using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace DesapegAutoWeb.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly IVeiculoService veiculoService;
        private readonly IAnuncioService anuncioService;
        private readonly IMapper mapper;

        public VeiculoController(IVeiculoService veiculoService, IAnuncioService anuncioService, IMapper mapper)
        {
            this.veiculoService = veiculoService;
            this.anuncioService = anuncioService;
            this.mapper = mapper;
        }

        // GET: Veiculo
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Index()
        {
            var listaVeiculos = veiculoService.GetAll();
            var listaVeiculosViewModel = mapper.Map<IEnumerable<VeiculoViewModel>>(listaVeiculos);
            return View(listaVeiculosViewModel);
        }

        // GET: Veiculo/Details/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Details(int id)
        {
            var veiculo = veiculoService.Get(id);
            if (veiculo == null) return NotFound();
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Create
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Veiculo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(VeiculoViewModel veiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                    veiculoService.Create(veiculo);
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Edit/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var veiculo = veiculoService.Get(id);
            if (veiculo == null) return NotFound();
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id, VeiculoViewModel veiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                    veiculoService.Edit(veiculo);
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Delete/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Delete(int id)
        {
            var veiculo = veiculoService.Get(id);
            if (veiculo == null) return NotFound();
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Delete(int id, VeiculoViewModel veiculoViewModel)
        {
            try
            {
                var anuncio = anuncioService.GetAll().FirstOrDefault(a => a.IdVeiculo == id);
                if (anuncio != null && anuncio.IdVenda != 0)
                {
                    ModelState.AddModelError(string.Empty, "Nao e possivel remover. Veiculo esta em uma venda pendente ou concluida.");
                    var veiculo = veiculoService.Get(id);
                    if (veiculo != null)
                    {
                        veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
                    }
                    return View(veiculoViewModel);
                }
                veiculoService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var veiculo = veiculoService.Get(id);
                if (veiculo != null)
                {
                    veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
                }
                return View(veiculoViewModel);
            }
        }
    }
}
