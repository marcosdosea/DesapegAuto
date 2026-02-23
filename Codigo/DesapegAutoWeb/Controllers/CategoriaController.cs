using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesapegAutoWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService categoriaService;
        private readonly IMapper mapper;

        public CategoriaController(ICategoriaService categoriaService, IMapper mapper)
        {
            this.categoriaService = categoriaService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            var listaCategorias = categoriaService.GetAll();
            var listaCategoriasViewModel = mapper.Map<IEnumerable<CategoriaViewModel>>(listaCategorias);
            return View(listaCategoriasViewModel);
        }

        public ActionResult Details(int id)
        {
            var categoria = categoriaService.Get(id);
            if (categoria == null) return NotFound();
            var categoriaViewModel = mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(CategoriaViewModel categoriaViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Preencha os campos obrigatorios para cadastrar a categoria.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var categoria = mapper.Map<Categoria>(categoriaViewModel);
                categoriaService.Create(categoria);
                TempData["SuccessMessage"] = "Categoria cadastrada com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel salvar a categoria devido a uma inconsistencia no banco de dados.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var categoria = categoriaService.Get(id);
            if (categoria == null) return NotFound();
            var categoriaViewModel = mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(CategoriaViewModel categoriaViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaViewModel);
            }

            try
            {
                var categoria = mapper.Map<Categoria>(categoriaViewModel);
                categoriaService.Edit(categoria);
                TempData["SuccessMessage"] = "Categoria atualizada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(categoriaViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var categoria = categoriaService.Get(id);
            if (categoria == null) return NotFound();
            var categoriaViewModel = mapper.Map<CategoriaViewModel>(categoria);
            return View(categoriaViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                categoriaService.Delete(id);
                TempData["SuccessMessage"] = "Categoria removida com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel remover a categoria porque ela esta em uso.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
