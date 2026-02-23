using System.Diagnostics;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Core.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace DesapegAutoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnuncioService _anuncioService;
        private readonly IVeiculoService _veiculoService;
        private readonly IModeloService _modeloService;
        private readonly IMarcaService _marcaService;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IAnuncioService anuncioService,
            IVeiculoService veiculoService,
            IModeloService modeloService,
            IMarcaService marcaService,
            IMapper mapper)
        {
            _logger = logger;
            _anuncioService = anuncioService;
            _veiculoService = veiculoService;
            _modeloService = modeloService;
            _marcaService = marcaService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            // Dealership users start from the ads dashboard.
            if (User.IsInRole("Funcionario"))
            {
                return RedirectToAction("Index", "Anuncio");
            }

            var allAnuncios = _anuncioService.GetAll().ToList();
            var allMarcas   = _marcaService.GetAll().ToList();
            var allModelos  = _modeloService.GetAll().ToList();

            // Pre-cache vehicles to avoid O(N×M) repeated service calls.
            var vehicleCache = new Dictionary<int, Core.Veiculo>();
            foreach (var a in allAnuncios)
            {
                if (!vehicleCache.ContainsKey(a.IdVeiculo))
                {
                    var v = _veiculoService.Get(a.IdVeiculo);
                    if (v != null) vehicleCache[a.IdVeiculo] = v;
                }
            }

            // Helper: enrich an AnuncioViewModel with vehicle details from cache.
            void Enrich(AnuncioViewModel av)
            {
                if (!vehicleCache.TryGetValue(av.IdVeiculo, out var veiculo)) return;
                var vm     = _mapper.Map<VeiculoViewModel>(veiculo);
                var marca  = allMarcas.FirstOrDefault(m => m.Id == veiculo.IdMarca);
                var modelo = allModelos.FirstOrDefault(m => m.Id == veiculo.IdModelo);
                if (marca  != null) vm.NomeMarca  = marca.Nome;
                if (modelo != null) vm.NomeModelo = modelo.Nome;
                av.Veiculo = vm;
            }

            // ── Destaques (3 ads) ──────────────────────────────────────────
            var destaques = _mapper.Map<List<AnuncioViewModel>>(allAnuncios.Take(3));
            destaques.ForEach(Enrich);

            // ── Mais Recentes (6 ads ordered by date desc) ────────────────
            var recentes = _mapper.Map<List<AnuncioViewModel>>(
                allAnuncios.OrderByDescending(a => a.DataPublicacao).Take(6));
            recentes.ForEach(Enrich);

            // ── Por Marca – top 8 brands, up to 4 ads each ───────────────
            var porMarca = new List<MarcaGroupViewModel>();
            foreach (var marca in allMarcas)
            {
                var marcaAnuncios = allAnuncios
                    .Where(a => vehicleCache.TryGetValue(a.IdVeiculo, out var v) && v.IdMarca == marca.Id)
                    .ToList();

                if (!marcaAnuncios.Any()) continue;

                var vms = _mapper.Map<List<AnuncioViewModel>>(marcaAnuncios.Take(4));
                vms.ForEach(Enrich);

                porMarca.Add(new MarcaGroupViewModel
                {
                    IdMarca       = marca.Id,
                    NomeMarca     = marca.Nome,
                    TotalAnuncios = marcaAnuncios.Count,
                    Anuncios      = vms,
                });
            }
            porMarca = porMarca.OrderByDescending(m => m.TotalAnuncios).Take(8).ToList();

            // ── Por Categoria – all categories, up to 4 ads each ─────────
            var catMap = new Dictionary<string, List<Core.Anuncio>>(StringComparer.OrdinalIgnoreCase);
            foreach (var anuncio in allAnuncios)
            {
                if (!vehicleCache.TryGetValue(anuncio.IdVeiculo, out var v)) continue;
                var modelo = allModelos.FirstOrDefault(m => m.Id == v.IdModelo);
                var cat = modelo?.Categoria;
                if (string.IsNullOrWhiteSpace(cat)) cat = "Outro";
                if (!catMap.ContainsKey(cat)) catMap[cat] = new List<Core.Anuncio>();
                catMap[cat].Add(anuncio);
            }

            var porCategoria = catMap
                .Select(kv =>
                {
                    var vms = _mapper.Map<List<AnuncioViewModel>>(kv.Value.Take(4));
                    vms.ForEach(Enrich);
                    return new CategoriaGroupViewModel
                    {
                        Categoria     = kv.Key,
                        TotalAnuncios = kv.Value.Count,
                        Anuncios      = vms,
                    };
                })
                .OrderByDescending(c => c.TotalAnuncios)
                .ToList();

            // ── Por Faixa de Preço – 5 buckets, up to 4 ads each ─────────
            var faixas = new[]
            {
                new { Label = "Até R$ 30k",        Min = (decimal?)null,     Max = (decimal?)30_000m  },
                new { Label = "R$ 30k – R$ 60k",   Min = (decimal?)30_000m,  Max = (decimal?)60_000m  },
                new { Label = "R$ 60k – R$ 100k",  Min = (decimal?)60_000m,  Max = (decimal?)100_000m },
                new { Label = "R$ 100k – R$ 200k", Min = (decimal?)100_000m, Max = (decimal?)200_000m },
                new { Label = "Acima de R$ 200k",  Min = (decimal?)200_000m, Max = (decimal?)null     },
            };

            var porFaixa = new List<FaixaPrecoViewModel>();
            foreach (var f in faixas)
            {
                var faixaAnuncios = allAnuncios
                    .Where(a =>
                    {
                        if (!vehicleCache.TryGetValue(a.IdVeiculo, out var v)) return false;
                        if (f.Min.HasValue && v.Preco < f.Min.Value)  return false;
                        if (f.Max.HasValue && v.Preco >= f.Max.Value) return false;
                        return true;
                    })
                    .ToList();

                var vms = _mapper.Map<List<AnuncioViewModel>>(faixaAnuncios.Take(4));
                vms.ForEach(Enrich);

                porFaixa.Add(new FaixaPrecoViewModel
                {
                    Label         = f.Label,
                    PrecoMin      = f.Min,
                    PrecoMax      = f.Max,
                    TotalAnuncios = faixaAnuncios.Count,
                    Anuncios      = vms,
                });
            }
            // Only show price ranges that actually have ads.
            porFaixa = porFaixa.Where(f => f.TotalAnuncios > 0).ToList();

            var homeModel = new HomeViewModel
            {
                Destaques     = destaques,
                PorMarca      = porMarca,
                PorCategoria  = porCategoria,
                PorFaixaPreco = porFaixa,
                MaisRecentes  = recentes,
            };

            return View(homeModel);
        }

        public IActionResult Search(string? termo, decimal? precoMin, decimal? precoMax, 
            int? anoMin, int? anoMax, int? kmMin, int? kmMax, string? localizacao, 
            List<string>? opcionais)
        {
            var anuncios = _anuncioService.GetAll();
            var model = _mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();
            
            if (!string.IsNullOrWhiteSpace(termo))
            {
                termo = termo.ToLower();
                model = model.Where(a => 
                {
                    if (a.Veiculo == null) return false;
                    var veiculo = _veiculoService.Get(a.IdVeiculo);
                    if (veiculo != null)
                    {
                        var marca = _marcaService.Get(veiculo.IdMarca);
                        var modelo = _modeloService.Get(veiculo.IdModelo);
                        return (marca?.Nome?.ToLower().Contains(termo) ?? false) ||
                               (modelo?.Nome?.ToLower().Contains(termo) ?? false);
                    }
                    return false;
                }).ToList();
            }
            
            foreach (var anuncio in model)
            {
                var veiculo = _veiculoService.Get(anuncio.IdVeiculo);
                if (veiculo != null)
                {
                    var veiculoViewModel = _mapper.Map<VeiculoViewModel>(veiculo);
                    
                    var marca = _marcaService.Get(veiculo.IdMarca);
                    var modelo = _modeloService.Get(veiculo.IdModelo);
                    
                    if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                    if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;
                    
                    anuncio.Veiculo = veiculoViewModel;
                }
            }
            
            if (precoMin.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Preco >= precoMin.Value).ToList();
            }
            if (precoMax.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Preco <= precoMax.Value).ToList();
            }
            
            if (anoMin.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Ano >= anoMin.Value).ToList();
            }
            if (anoMax.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Ano <= anoMax.Value).ToList();
            }
            
            if (kmMin.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Quilometragem >= kmMin.Value).ToList();
            }
            if (kmMax.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Quilometragem <= kmMax.Value).ToList();
            }
            
            if (opcionais != null && opcionais.Any())
            {
                model = model.Where(a => 
                {
                    if (string.IsNullOrWhiteSpace(a.Opcionais)) return false;
                    return opcionais.All(o => a.Opcionais.ToLower().Contains(o.ToLower()));
                }).ToList();
            }
            
            ViewBag.Termo = termo;
            ViewBag.PrecoMin = precoMin;
            ViewBag.PrecoMax = precoMax;
            ViewBag.AnoMin = anoMin;
            ViewBag.AnoMax = anoMax;
            ViewBag.KmMin = kmMin;
            ViewBag.KmMax = kmMax;
            ViewBag.Localizacao = localizacao;
            ViewBag.Opcionais = opcionais;
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
