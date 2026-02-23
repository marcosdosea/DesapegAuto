using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesapegAutoWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VersaoController : Controller
    {
        private readonly IVersaoService versaoService;
        private readonly IModeloService modeloService;
        private readonly IMapper mapper;

        public VersaoController(IVersaoService versaoService, IModeloService modeloService, IMapper mapper)
        {
            this.versaoService = versaoService;
            this.modeloService = modeloService;
            this.mapper = mapper;
        }

        // GET: VersaoController
        public ActionResult Index()
        {
            var listaVersoes = versaoService.GetAll();
            var listaVersoesViewModel = mapper.Map<List<VersaoViewModel>>(listaVersoes);
            return View(listaVersoesViewModel);
        }

        // GET: VersaoController/Details/5
        public ActionResult Details(int id)
        {
            var versao = versaoService.Get(id);
            if (versao == null) return NotFound();
            var versaoViewModel = mapper.Map<VersaoViewModel>(versao);
            return View(versaoViewModel);
        }

        // GET: VersaoController/Create
        public ActionResult Create()
        {
            // Carrega os modelos para preencher o dropdown (lista de seleção)
            ViewBag.IdModelo = new SelectList(modeloService.GetAll(), "Id", "Nome");
            return View();
        }

        // POST: VersaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VersaoViewModel versaoViewModel)
        {
            if (ModelState.IsValid)
            {
                var versao = mapper.Map<Versao>(versaoViewModel);
                versaoService.Create(versao);
                return RedirectToAction(nameof(Index));
            }
            // Se o modelo for inválido, recarrega o dropdown e retorna para a view
            ViewBag.IdModelo = new SelectList(modeloService.GetAll(), "Id", "Nome", versaoViewModel.IdModelo);
            return View(versaoViewModel);
        }

        // GET: VersaoController/Edit/5
        public ActionResult Edit(int id)
        {
            var versao = versaoService.Get(id);
            if (versao == null)
            {
                return NotFound();
            }
            var versaoViewModel = mapper.Map<VersaoViewModel>(versao);

            // Carrega os modelos para o dropdown, já selecionando o modelo atual da versão
            ViewBag.IdModelo = new SelectList(modeloService.GetAll(), "Id", "Nome", versao.IdModelo);
            return View(versaoViewModel);
        }

        // POST: VersaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VersaoViewModel versaoViewModel)
        {
            if (ModelState.IsValid)
            {
                var versao = mapper.Map<Versao>(versaoViewModel);
                versaoService.Edit(versao);
                return RedirectToAction(nameof(Index));
            }
            // Se o modelo for inválido, recarrega o dropdown
            ViewBag.IdModelo = new SelectList(modeloService.GetAll(), "Id", "Nome", versaoViewModel.IdModelo);
            return View(versaoViewModel);
        }

        // GET: VersaoController/Delete/5
        public ActionResult Delete(int id)
        {
            var versao = versaoService.Get(id);
            if (versao == null) return NotFound();
            var versaoViewModel = mapper.Map<VersaoViewModel>(versao);
            return View(versaoViewModel);
        }

        // POST: VersaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VersaoViewModel versaoViewModel)
        {
            try
            {
                versaoService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                // Mostra o erro na tela caso o service lance uma exceção
                ModelState.AddModelError(string.Empty, ex.Message);
                var versao = versaoService.Get(id);
                if (versao != null)
                {
                    var vvm = mapper.Map<VersaoViewModel>(versao);
                    return View(vvm);
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}