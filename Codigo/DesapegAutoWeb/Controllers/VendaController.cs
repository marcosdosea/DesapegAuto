using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesapegAutoWeb.Controllers
{
    public class VendaController : Controller
    {
        private readonly IVendaService vendaService;
        private readonly IConcessionariaService concessionariaService;
        private readonly IPessoaService pessoaService; 
        private readonly IMapper mapper;

        public VendaController(
            IVendaService vendaService,
            IConcessionariaService concessionariaService,
            IPessoaService pessoaService,
            IMapper mapper)
        {
            this.vendaService = vendaService;
            this.concessionariaService = concessionariaService;
            this.pessoaService = pessoaService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            var listaVendas = vendaService.GetAllDTO();
            return View(listaVendas);
        }

        public ActionResult Details(int id)
        {
            var venda = vendaService.Get(id);
            var vendaViewModel = mapper.Map<VendaViewModel>(venda);

            ViewBag.NomeConcessionaria = concessionariaService.Get(venda.IdConcessionaria)?.Nome;
            ViewBag.NomePessoa = pessoaService.Get(venda.IdPessoa)?.Nome;

            return View(vendaViewModel);
        }

        public ActionResult Create()
        {
            PopulateViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendaViewModel vendaViewModel)
        {
            if (ModelState.IsValid)
            {
                var venda = mapper.Map<Venda>(vendaViewModel);
                vendaService.Create(venda);
                return RedirectToAction(nameof(Index));
            }

            PopulateViewBags(vendaViewModel);
            return View(vendaViewModel);
        }

        public ActionResult Edit(int id)
        {
            var venda = vendaService.Get(id);
            var vendaViewModel = mapper.Map<VendaViewModel>(venda);

            PopulateViewBags(vendaViewModel);
            return View(vendaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VendaViewModel vendaViewModel)
        {
            if (ModelState.IsValid)
            {
                var venda = mapper.Map<Venda>(vendaViewModel);
                vendaService.Edit(venda);
                return RedirectToAction(nameof(Index));
            }

            PopulateViewBags(vendaViewModel);
            return View(vendaViewModel);
        }

        public ActionResult Delete(int id)
        {
            var venda = vendaService.Get(id);
            var vendaViewModel = mapper.Map<VendaViewModel>(venda);
            return View(vendaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VendaViewModel vendaViewModel)
        {
            try
            {
                vendaService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vendaViewModel);
            }
        }

        private void PopulateViewBags(VendaViewModel? model = null)
        {
            ViewBag.IdConcessionaria = new SelectList(concessionariaService.GetAll(), "Id", "Nome", model?.IdConcessionaria);
            ViewBag.IdPessoa = new SelectList(pessoaService.GetAll(), "Id", "Nome", model?.IdPessoa);
        }
    }
}