using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DesapegAutoWeb.Controllers
{
    public class AnuncioController : Controller
    {
        private readonly IAnuncioService anuncioService;
        private readonly IVeiculoService veiculoService;
        private readonly IVendaService vendaService;
        private readonly IModeloService modeloService;
        private readonly IMarcaService marcaService;
        private readonly IMapper mapper;

        public AnuncioController(
            IAnuncioService anuncioService,
            IVeiculoService veiculoService,
            IVendaService vendaService,
            IModeloService modeloService,
            IMarcaService marcaService,
            IMapper mapper)
        {
            this.anuncioService = anuncioService;
            this.veiculoService = veiculoService;
            this.vendaService = vendaService;
            this.modeloService = modeloService;
            this.marcaService = marcaService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            var anuncios = anuncioService.GetAll();
            var model = mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();
            foreach (var anuncio in model)
            {
                var veiculo = veiculoService.Get(anuncio.IdVeiculo);
                if (veiculo != null)
                {
                    var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);

                    var marca = marcaService.Get(veiculo.IdMarca);
                    var modelo = modeloService.Get(veiculo.IdModelo);

                    if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                    if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;

                    anuncio.Veiculo = veiculoViewModel;
                }
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null) return NotFound();

            var model = mapper.Map<AnuncioViewModel>(anuncio);

            var veiculo = veiculoService.Get(anuncio.IdVeiculo);
            if (veiculo != null)
            {
                var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);

                var marca = marcaService.Get(veiculo.IdMarca);
                var modelo = modeloService.Get(veiculo.IdModelo);

                if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;

                model.Veiculo = veiculoViewModel;
            }

            anuncio.Visualizacoes++;
            anuncioService.Edit(anuncio);

            return View(model);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            var model = new AnuncioViewModel();
            PopulateViewBags();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(AnuncioViewModel anuncioViewModel)
        {
            if (ModelState.IsValid)
            {
                anuncioViewModel.Descricao ??= string.Empty;
                anuncioViewModel.Opcionais ??= string.Empty;
                var anuncio = mapper.Map<Anuncio>(anuncioViewModel);
                anuncioService.Create(anuncio);
                return RedirectToAction(nameof(Index));
            }

            PopulateViewBags(anuncioViewModel);
            return View(anuncioViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null) return NotFound();
            var model = mapper.Map<AnuncioViewModel>(anuncio);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, AnuncioViewModel anuncioViewModel)
        {
            anuncioService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateViewBags(AnuncioViewModel? model = null)
        {
            ViewBag.Veiculos = new SelectList(veiculoService.GetAll(), "Id", "Placa", model?.IdVeiculo);
            ViewBag.Vendas = new SelectList(vendaService.GetAll(), "Id", "Id", model?.IdVenda);
        }
    }
}
