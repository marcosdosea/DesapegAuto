using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesapegAutoWeb.Controllers
{
    public class AnuncioController : Controller
    {
        private readonly IAnuncioService anuncioService;
        private readonly IVeiculoService veiculoService;
        private readonly IVendaService vendaService;
        private readonly IMapper mapper;

        public AnuncioController(
            IAnuncioService anuncioService,
            IVeiculoService veiculoService,
            IVendaService vendaService,
            IMapper mapper)
        {
            this.anuncioService = anuncioService;
            this.veiculoService = veiculoService;
            this.vendaService = vendaService;
            this.mapper = mapper;
        }

        // GET: Anuncio
        public ActionResult Index()
        {
            var anuncios = anuncioService.GetAll();
            var model = mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();
            foreach (var anuncio in model)
            {
                var veiculo = veiculoService.Get(anuncio.IdVeiculo);
                if (veiculo != null)
                    anuncio.Veiculo = mapper.Map<VeiculoViewModel>(veiculo);
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null) return NotFound();
            var model = mapper.Map<AnuncioViewModel>(anuncio);
            return View(model);
        }

        // GET: Anuncio/Create
        public ActionResult Create()
        {
            var model = new AnuncioViewModel();
            PopulateViewBags();
            return View(model);
        }

        // POST: Anuncio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AnuncioViewModel anuncioViewModel)
        {
            if (ModelState.IsValid)
            {
                var anuncio = mapper.Map<Anuncio>(anuncioViewModel);
                anuncioService.Create(anuncio);
                return RedirectToAction(nameof(Index));
            }

            PopulateViewBags(anuncioViewModel);
            return View(anuncioViewModel);
        }

        // GET: Anuncio/Delete/5
        public ActionResult Delete(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null) return NotFound();
            var model = mapper.Map<AnuncioViewModel>(anuncio);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
