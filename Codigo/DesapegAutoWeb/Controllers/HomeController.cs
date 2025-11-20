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
            // Se o usuário não estiver autenticado, não redireciona mais
            // if (!User.Identity?.IsAuthenticated ?? true)
            // {
            //     return RedirectToPage("/Account/Register", new { area = "Identity" });
            // }

            // Carregar alguns anúncios para exibir na página inicial
            var anuncios = _anuncioService.GetAll().Take(3);
            var model = _mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();

            foreach (var anuncio in model)
            {
                var veiculo = _veiculoService.Get(anuncio.IdVeiculo);
                if (veiculo != null)
                {
                    var veiculoViewModel = _mapper.Map<VeiculoViewModel>(veiculo);

                    // Buscar marca e modelo
                    var marca = _marcaService.Get(veiculo.IdMarca);
                    var modelo = _modeloService.Get(veiculo.IdModelo);

                    if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                    if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;

                    anuncio.Veiculo = veiculoViewModel;
                }
            }

            return View(model);
        }

        public IActionResult Search(string? termo, decimal? precoMin, decimal? precoMax,
            int? anoMin, int? anoMax, int? kmMin, int? kmMax, string? localizacao,
            List<string>? opcionais)
        {
            var anuncios = _anuncioService.GetAll();
            var model = _mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(termo))
            {
                var termoLower = termo.ToLower();
                model = model.Where(a =>
                {
                    // Removed premature check: if (a.Veiculo == null) return false;
                    var veiculo = _veiculoService.Get(a.IdVeiculo);
                    if (veiculo != null)
                    {
                        var marca = _marcaService.Get(veiculo.IdMarca);
                        var modelo = _modeloService.Get(veiculo.IdModelo);
                        return (marca?.Nome?.ToLower().Contains(termoLower) ?? false) ||
                               (modelo?.Nome?.ToLower().Contains(termoLower) ?? false);
                    }
                    return false;
                }).ToList();
            }

            // Carregar dados do veículo para cada anúncio
            foreach (var anuncio in model)
            {
                var veiculo = _veiculoService.Get(anuncio.IdVeiculo);
                if (veiculo != null)
                {
                    var veiculoViewModel = _mapper.Map<VeiculoViewModel>(veiculo);

                    // Buscar marca e modelo
                    var marca = _marcaService.Get(veiculo.IdMarca);
                    var modelo = _modeloService.Get(veiculo.IdModelo);

                    if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                    if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;

                    anuncio.Veiculo = veiculoViewModel;
                }
            }

            // Aplicar filtros de preço
            if (precoMin.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Preco >= precoMin.Value).ToList();
            }
            if (precoMax.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Preco <= precoMax.Value).ToList();
            }

            // Aplicar filtros de ano
            if (anoMin.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Ano >= anoMin.Value).ToList();
            }
            if (anoMax.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Ano <= anoMax.Value).ToList();
            }

            // Aplicar filtros de quilometragem
            if (kmMin.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Quilometragem >= kmMin.Value).ToList();
            }
            if (kmMax.HasValue)
            {
                model = model.Where(a => a.Veiculo?.Quilometragem <= kmMax.Value).ToList();
            }

            // Aplicar filtros de opcionais
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
