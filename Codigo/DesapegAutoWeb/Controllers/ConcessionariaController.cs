using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;

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
            if (ModelState.IsValid)
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Create(c);
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
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
            if (id != vm.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Edit(c);
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
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
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }
    }
}
