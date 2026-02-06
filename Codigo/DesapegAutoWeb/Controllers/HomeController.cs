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
            // Redirect unauthenticated users to register/login flow.
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            // Dealership users start from the ads dashboard.
            if (User.IsInRole("Funcionario"))
            {
                return RedirectToAction("Index", "Anuncio");
            }

            // Load a few ads for home preview.
            var anuncios = _anuncioService.GetAll().Take(3);
            var model = _mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();
            
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
            
            return View(model);
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
