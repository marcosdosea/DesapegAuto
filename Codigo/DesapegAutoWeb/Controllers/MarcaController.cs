using Microsoft.AspNetCore.Mvc;
using Core;
using AutoMapper;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace DesapegAutoWeb.Controllers
{
    public class MarcaController : Controller
    {
        private readonly IMarcaService marcaService;
        private readonly IMapper mapper;    

        public MarcaController(IMarcaService marcaService, IMapper mapper)
        {
            this.marcaService = marcaService;
            this.mapper = mapper;
        }

        // GET: Marca

        public ActionResult Index()
        {
            var listaMarcas = marcaService.GetAll();
            var listaMarcasViewModel = mapper.Map<IEnumerable<MarcaViewModel>>(listaMarcas);
            return View(listaMarcasViewModel);
        }


        // GET: Marca/Details/5 

        public ActionResult Details(int id)
        {
            var marca = marcaService.Get(id);
            var marcaViewModel = mapper.Map<MarcaViewModel>(marca);
            return View(marcaViewModel);
        }

        // GET: Marca/Create
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return View();
        }


        // POST: Marca/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(MarcaViewModel marcaViewModel)
        {
            if (ModelState.IsValid)
            {
                var marca = mapper.Map<Marca>(marcaViewModel);
                marcaService.Create(marca);
                return RedirectToAction(nameof(Index));
            }
            return View(marcaViewModel);
        }

        // GET: Marca/Edit/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var marca = marcaService.Get(id);
            var marcaViewModel = mapper.Map<MarcaViewModel>(marca);
            return View(marcaViewModel);
        }

        // POST: Marca/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(MarcaViewModel marcaViewModel)
        {
            if (ModelState.IsValid)
            {
                var marca = mapper.Map<Marca>(marcaViewModel);
                marcaService.Edit(marca);
                return RedirectToAction(nameof(Index));
            }
            return View(marcaViewModel);
        }

        // GET: Marca/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var marca = marcaService.Get(id);
            var marcaViewModel = mapper.Map<MarcaViewModel>(marca);
            return View(marcaViewModel);
        }

        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            marcaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
