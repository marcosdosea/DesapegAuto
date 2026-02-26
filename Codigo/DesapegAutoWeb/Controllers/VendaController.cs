using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System;
using System.Linq;

namespace DesapegAutoWeb.Controllers
{
    public class VendaController : Controller
    {
        private readonly IVendaService vendaService;
        private readonly IConcessionariaService concessionariaService;
        private readonly IPessoaService pessoaService;
        private readonly IVeiculoService? veiculoService;
        private readonly IModeloService? modeloService;
        private readonly IMarcaService? marcaService;
        private readonly IAnuncioService anuncioService;
        private readonly IMapper mapper;

        public VendaController(
            IVendaService vendaService,
            IConcessionariaService concessionariaService,
            IPessoaService pessoaService,
            IMapper mapper,
            IAnuncioService anuncioService,
            IVeiculoService? veiculoService = null,
            IModeloService? modeloService = null,
            IMarcaService? marcaService = null)
        {
            this.vendaService = vendaService;
            this.concessionariaService = concessionariaService;
            this.pessoaService = pessoaService;
            this.veiculoService = veiculoService;
            this.modeloService = modeloService;
            this.marcaService = marcaService;
            this.anuncioService = anuncioService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Index()
        {
            var listaVendas = vendaService.GetAllDTO();
            return View(listaVendas);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Details(int id)
        {
            var venda = vendaService.Get(id);
            if (venda == null)
            {
                return NotFound();
            }
            var vendaViewModel = mapper.Map<VendaViewModel>(venda);

            var concessionaria = concessionariaService.Get(venda.IdConcessionaria);
            var pessoa = pessoaService.Get(venda.IdPessoa);

            ViewBag.NomeConcessionaria = concessionaria?.Nome ?? "N/A";
            ViewBag.NomePessoa = pessoa?.Nome ?? "N/A";

            return View(vendaViewModel);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(int? idVeiculo = null)
        {
            PopulateViewBags();

            if (!idVeiculo.HasValue)
            {
                TempData["ErrorMessage"] = "Selecione um veiculo para iniciar a venda.";
                return RedirectToAction("Index", "Anuncio");
            }

            var anuncio = anuncioService.GetAll().FirstOrDefault(a => a.IdVeiculo == idVeiculo.Value);
            if (anuncio == null)
            {
                TempData["ErrorMessage"] = "Anuncio nao encontrado para este veiculo.";
                return RedirectToAction("Index", "Anuncio");
            }

            if (anuncio.IdVenda != 0 ||
                string.Equals(anuncio.StatusAnuncio, "V", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(anuncio.StatusAnuncio, "P", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Este veiculo nao esta disponivel para nova venda.";
                return RedirectToAction("Index", "Anuncio");
            }

            if (idVeiculo.HasValue && veiculoService != null)
            {
                var veiculo = veiculoService.Get(idVeiculo.Value);
                if (veiculo != null)
                {
                    var modelo = modeloService?.Get(veiculo.IdModelo);
                    var marca = marcaService?.Get(veiculo.IdMarca);

                    ViewBag.VeiculoPreview = new
                    {
                        Id = veiculo.Id,
                        Nome = $"{marca?.Nome} {modelo?.Nome}".Trim(),
                        Ano = veiculo.Ano,
                        Preco = veiculo.Preco.ToString("C0", new CultureInfo("pt-BR")),
                        Cor = veiculo.Cor,
                        Quilometragem = veiculo.Quilometragem
                    };
                }
            }

            var model = new VendaViewModel
            {
                IdVeiculo = idVeiculo.Value,
                IdConcessionaria = veiculoService?.Get(idVeiculo.Value)?.IdConcessionaria ?? 0,
                DataVenda = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(VendaViewModel vendaViewModel)
        {
            var anuncio = anuncioService.GetAll().FirstOrDefault(a => a.IdVeiculo == vendaViewModel.IdVeiculo);
            if (anuncio == null)
            {
                TempData["ErrorMessage"] = "Anuncio nao encontrado para este veiculo.";
                return RedirectToAction("Index", "Anuncio");
            }

            if (anuncio.IdVenda != 0 ||
                string.Equals(anuncio.StatusAnuncio, "V", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(anuncio.StatusAnuncio, "P", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Este veiculo nao esta disponivel para nova venda.";
                return RedirectToAction("Index", "Anuncio");
            }

            var veiculo = veiculoService?.Get(vendaViewModel.IdVeiculo);
            if (veiculo != null && vendaViewModel.IdConcessionaria == 0)
            {
                vendaViewModel.IdConcessionaria = veiculo.IdConcessionaria;
                ModelState.Remove(nameof(vendaViewModel.IdConcessionaria));
                TryValidateModel(vendaViewModel);
            }

            if (ModelState.IsValid)
            {
                var venda = mapper.Map<Venda>(vendaViewModel);
                vendaService.Create(venda);
                anuncio.IdVenda = venda.Id;
                anuncio.StatusAnuncio = "V";
                anuncioService.Edit(anuncio);
                return RedirectToAction(nameof(Index));
            }

            PopulateViewBags(vendaViewModel);
            if (veiculoService != null)
            {
                var veiculoPreview = veiculoService.Get(vendaViewModel.IdVeiculo);
                if (veiculoPreview != null)
                {
                    var modelo = modeloService?.Get(veiculoPreview.IdModelo);
                    var marca = marcaService?.Get(veiculoPreview.IdMarca);
                    ViewBag.VeiculoPreview = new
                    {
                        Id = veiculoPreview.Id,
                        Nome = $"{marca?.Nome} {modelo?.Nome}".Trim(),
                        Ano = veiculoPreview.Ano,
                        Preco = veiculoPreview.Preco.ToString("C0", new CultureInfo("pt-BR")),
                        Cor = veiculoPreview.Cor,
                        Quilometragem = veiculoPreview.Quilometragem
                    };
                }
            }
            return View(vendaViewModel);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var venda = vendaService.Get(id);
            if (venda == null)
            {
                return NotFound();
            }
            var vendaViewModel = mapper.Map<VendaViewModel>(venda);

            PopulateViewBags(vendaViewModel);
            return View(vendaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var venda = vendaService.Get(id);
            if (venda == null)
            {
                return NotFound();
            }
            var vendaViewModel = mapper.Map<VendaViewModel>(venda);
            return View(vendaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Confirmar(int id)
        {
            var venda = vendaService.Get(id);
            if (venda == null)
            {
                return NotFound();
            }

            var anuncio = anuncioService.GetAll().FirstOrDefault(a => a.IdVenda == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            anuncio.StatusAnuncio = "V";
            anuncioService.Edit(anuncio);
            TempData["SuccessMessage"] = "Venda confirmada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateViewBags(VendaViewModel? model = null)
        {
            ViewBag.IdConcessionaria = new SelectList(concessionariaService.GetAll(), "Id", "Nome", model?.IdConcessionaria);
            ViewBag.IdPessoa = new SelectList(pessoaService.GetAll(), "Id", "Nome", model?.IdPessoa);
        }
    }
}
