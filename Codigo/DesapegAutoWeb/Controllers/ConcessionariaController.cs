using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesapegAutoWeb.Controllers
{
    public class ConcessionariaController : Controller
    {
        private readonly IConcessionariaService concessionariaService;
        private readonly IMapper mapper;

        public ConcessionariaController(IConcessionariaService concessionariaService, IMapper mapper)
        {
            this.concessionariaService = concessionariaService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            var lista = concessionariaService.GetAll();
            var vm = mapper.Map<List<ConcessionariaViewModel>>(lista);
            return View(vm);
        }

        public ActionResult Details(int id)
        {
            var c = concessionariaService.Get(id);
            var vm = mapper.Map<ConcessionariaViewModel>(c);
            return View(vm);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ConcessionariaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Preencha os campos obrigatorios para cadastrar a concessionaria.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Create(c);
                TempData["SuccessMessage"] = "Concessionaria cadastrada com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel salvar a concessionaria devido a uma inconsistencia no banco de dados.";
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Edit(int id)
        {
            var c = concessionariaService.Get(id);
            var vm = mapper.Map<ConcessionariaViewModel>(c);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ConcessionariaViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Edit(c);
                TempData["SuccessMessage"] = "Concessionaria atualizada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel atualizar a concessionaria devido a uma inconsistencia no banco de dados.");
                return View(vm);
            }
        }

        public ActionResult Delete(int id)
        {
            var c = concessionariaService.Get(id);
            var vm = mapper.Map<ConcessionariaViewModel>(c);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ConcessionariaViewModel vm)
        {
            try
            {
                concessionariaService.Delete(id);
                TempData["SuccessMessage"] = "Concessionaria removida com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel remover a concessionaria porque ela esta em uso.");
                return View(vm);
            }
        }
    }
}
