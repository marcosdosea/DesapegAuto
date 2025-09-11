using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace DesapegAutoWeb.Controllers
{
    public class ModeloController : Controller
    {
        private readonly IModeloService modeloService;
        private readonly IMarcaService marcaService;
        private readonly IMapper mapper;

        public ModeloController(IModeloService modeloService, IMarcaService marcaService, IMapper mapper)
        {
            this.modeloService = modeloService;
            this.marcaService = marcaService;
            this.mapper = mapper;
        }

        // GET: Modelo
        public ActionResult Index()
        {
            var listaModelos = modeloService.GetAll();
            var listaModelosVm = mapper.Map<IEnumerable<ModeloViewModel>>(listaModelos);
            return View(listaModelosVm);
        }

        // GET: Modelo/Details/5
        public ActionResult Details(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }
            var vm = mapper.Map<ModeloViewModel>(modelo);
            return View(vm);
        }

        // GET: Modelo/Create
        public ActionResult Create()
        {
            var vm = new ModeloViewModel();
            // Optionally populate vm.Marcas from marcaService here
            return View(vm);
        }

        // POST: Modelo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModeloViewModel modeloViewModel)
        {
            if (ModelState.IsValid)
            {
                var modelo = mapper.Map<Modelo>(modeloViewModel);
                modeloService.Create(modelo);
                return RedirectToAction(nameof(Index));
            }
            return View(modeloViewModel);
        }

        // GET: Modelo/Edit/5
        public ActionResult Edit(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }
            var vm = mapper.Map<ModeloViewModel>(modelo);
            return View(vm);
        }

        // POST: Modelo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ModeloViewModel modeloViewModel)
        {
            if (id != modeloViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var modelo = mapper.Map<Modelo>(modeloViewModel);
                modeloService.Edit(modelo);
                return RedirectToAction(nameof(Index));
            }
            return View(modeloViewModel);
        }

        // GET: Modelo/Delete/5
        public ActionResult Delete(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }
            var vm = mapper.Map<ModeloViewModel>(modelo);
            return View(vm);
        }

        // POST: Modelo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            modeloService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // Additional Methods from IModeloService
        public ActionResult GetByMarca(int idMarca)
        {
            var modelosDto = modeloService.GetByMarca(idMarca);
            // Map DTO -> ViewModel
            var vms = modelosDto.Select(m => new ModeloViewModel
            {
                Id = m.Id,
                Nome = m.Nome ?? string.Empty,
                Versoes = m.Versoes,
                IdMarca = m.IdMarca,
                Categoria = string.Empty
            });
            return View("Index", vms);
        }

        public ActionResult GetByCategoria(string categoria)
        {
            var modelosDto = modeloService.GetByCategoria(categoria);
            var vms = modelosDto.Select(m => new ModeloViewModel
            {
                Id = m.Id,
                Nome = m.Nome ?? string.Empty,
                Versoes = m.Versoes,
                IdMarca = m.IdMarca,
                Categoria = categoria
            });
            return View("Index", vms);
        }

        public ActionResult GetByNome(string nome)
        {
            var modelosDto = modeloService.GetByNome(nome);
            var vms = modelosDto.Select(m => new ModeloViewModel
            {
                Id = m.Id,
                Nome = m.Nome ?? string.Empty,
                Versoes = m.Versoes,
                IdMarca = m.IdMarca,
                Categoria = string.Empty
            });
            return View("Index", vms);
        }
    }
}
