using Microsoft.AspNetCore.Mvc;
using Core;
using AutoMapper;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace DesapegAutoWeb.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService categoriaService;
        private readonly IMapper mapper;

        public CategoriaController(ICategoriaService categoriaService, IMapper mapper)
        {
            this.categoriaService = categoriaService;
            this.mapper = mapper;
        }

        // GET: Categoria
        public ActionResult Index()
        {
            var listaCategorias = categoriaService.GetAll();
            var listaCategoriasViewModel = mapper.Map<IEnumerable<CategoriaViewModel>>(listaCategorias);
            return View(listaCategoriasViewModel);
        }

        // GET: Categoria/Details/5
        public ActionResult Details(int id)
        {
            var categoria = categoriaService.Get(id);
            var categoriaViewModel = mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        // GET: Categoria/Create
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(CategoriaViewModel categoriaViewModel)
        {
            if (ModelState.IsValid)
            {
                var categoria = mapper.Map<Categoria>(categoriaViewModel);
                categoriaService.Create(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaViewModel);
        }

        // GET: Categoria/Edit/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var categoria = categoriaService.Get(id);
            var categoriaViewModel = mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(CategoriaViewModel categoriaViewModel)
        {
            if (ModelState.IsValid)
            {
                var categoria = mapper.Map<Categoria>(categoriaViewModel);
                categoriaService.Edit(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaViewModel);
        }

        // GET: Categoria/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var categoria = categoriaService.Get(id);
            var categoriaViewModel = mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        // POST: Categoria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            categoriaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

