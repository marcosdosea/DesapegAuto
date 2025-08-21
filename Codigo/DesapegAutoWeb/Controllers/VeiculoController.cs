using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DesapegAutoWeb.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly IVeiculoService veiculoService;
        private readonly IMapper mapper;

        public VeiculoController(IVeiculoService veiculoService, IMapper mapper)
        {
            this.veiculoService = veiculoService;
            this.mapper = mapper;
        }

        // GET: Veiculo
        public ActionResult Index()
        {
            var listaVeiculos = veiculoService.GetAll();
            var listaVeiculosViewModel = mapper.Map<IEnumerable<VeiculoViewModel>>(listaVeiculos);
            return View(listaVeiculosViewModel);
        }

        // GET: Veiculo/Details/5
        public ActionResult Details(int id)
        {
            var veiculo = veiculoService.Get(id);
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Veiculo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VeiculoViewModel veiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                veiculoService.Create(veiculo);
                return RedirectToAction(nameof(Index));
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Edit/5
        public ActionResult Edit(int id)
        {
            var veiculo = veiculoService.Get(id);
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VeiculoViewModel veiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                veiculoService.Edit(veiculo);
                return RedirectToAction(nameof(Index));
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Delete/5
        public ActionResult Delete(int id)
        {
            var veiculo = veiculoService.Get(id);
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VeiculoViewModel veiculoViewModel)
        {
            veiculoService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public RedirectToActionResult Edit(VeiculoViewModel veiculoViewModel)
        {
            throw new NotImplementedException();
        }
    }
}