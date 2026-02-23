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
    public class MarcaController : Controller
    {
        private readonly IMarcaService marcaService;
        private readonly IMapper mapper;

        public MarcaController(IMarcaService marcaService, IMapper mapper)
        {
            this.marcaService = marcaService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            var listaMarcas = marcaService.GetAll();
            var listaMarcasViewModel = mapper.Map<IEnumerable<MarcaViewModel>>(listaMarcas);
            return View(listaMarcasViewModel);
        }

        public ActionResult Details(int id)
        {
            var marca = marcaService.Get(id);
            if (marca == null) return NotFound();
            var marcaViewModel = mapper.Map<MarcaViewModel>(marca);
            return View(marcaViewModel);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(MarcaViewModel marcaViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Preencha os campos obrigatorios para cadastrar a marca.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var marca = mapper.Map<Marca>(marcaViewModel);
                marcaService.Create(marca);
                TempData["SuccessMessage"] = "Marca cadastrada com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel salvar a marca devido a uma inconsistencia no banco de dados.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var marca = marcaService.Get(id);
            if (marca == null) return NotFound();
            var marcaViewModel = mapper.Map<MarcaViewModel>(marca);
            return View(marcaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(MarcaViewModel marcaViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(marcaViewModel);
            }

            try
            {
                var marca = mapper.Map<Marca>(marcaViewModel);
                marcaService.Edit(marca);
                TempData["SuccessMessage"] = "Marca atualizada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(marcaViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var marca = marcaService.Get(id);
            if (marca == null) return NotFound();
            var marcaViewModel = mapper.Map<MarcaViewModel>(marca);
            return View(marcaViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                marcaService.Delete(id);
                TempData["SuccessMessage"] = "Marca removida com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel remover a marca porque ela esta em uso.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
