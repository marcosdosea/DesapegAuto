using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DesapegAutoWeb.Controllers
{
    public class CompraController : Controller
    {
        private readonly IAnuncioService anuncioService;
        private readonly IVeiculoService veiculoService;
        private readonly IMarcaService marcaService;
        private readonly IModeloService modeloService;
        private readonly IConcessionariaService concessionariaService;
        private readonly IPessoaService pessoaService;
        private readonly IVendaService vendaService;
        public CompraController(
            IAnuncioService anuncioService,
            IVeiculoService veiculoService,
            IMarcaService marcaService,
            IModeloService modeloService,
            IConcessionariaService concessionariaService,
            IPessoaService pessoaService,
            IVendaService vendaService)
        {
            this.anuncioService = anuncioService;
            this.veiculoService = veiculoService;
            this.marcaService = marcaService;
            this.modeloService = modeloService;
            this.concessionariaService = concessionariaService;
            this.pessoaService = pessoaService;
            this.vendaService = vendaService;
        }

        public ActionResult Create(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null)
            {
                return NotFound();
            }

            if (string.Equals(anuncio.StatusAnuncio, "V", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Este veiculo ja foi vendido.";
                return RedirectToAction("Details", "Anuncio", new { id = anuncio.Id });
            }

            if (string.Equals(anuncio.StatusAnuncio, "P", StringComparison.OrdinalIgnoreCase) || anuncio.IdVenda != 0)
            {
                TempData["ErrorMessage"] = "Este veiculo esta com compra pendente no momento.";
                return RedirectToAction("Details", "Anuncio", new { id = anuncio.Id });
            }

            var veiculo = veiculoService.Get(anuncio.IdVeiculo);
            if (veiculo == null)
            {
                return NotFound();
            }

            var model = new CompraViewModel
            {
                IdAnuncio = anuncio.Id,
                ValorFinal = veiculo.Preco
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.Identity?.Name;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var pessoa = pessoaService.GetByEmail(email);
                    if (pessoa != null)
                    {
                        model.Nome = pessoa.Nome;
                        model.Email = pessoa.Email;
                        model.Cpf = pessoa.Cpf;
                        model.Telefone = pessoa.Telefone;
                    }
                    else
                    {
                        model.Email = email;
                    }
                }
            }

            PopulatePreview(anuncio, veiculo);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompraViewModel model)
        {
            model.Cpf = NormalizeDigits(model.Cpf);
            model.Telefone = NormalizeDigits(model.Telefone);

            ModelState.Remove(nameof(model.Cpf));
            ModelState.Remove(nameof(model.Telefone));
            TryValidateModel(model);

            var anuncio = anuncioService.Get(model.IdAnuncio);
            if (anuncio == null)
            {
                return NotFound();
            }

            var veiculo = veiculoService.Get(anuncio.IdVeiculo);
            if (veiculo == null)
            {
                return NotFound();
            }

            if (string.Equals(anuncio.StatusAnuncio, "V", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Este veiculo ja foi vendido.";
                return RedirectToAction("Details", "Anuncio", new { id = anuncio.Id });
            }

            if (string.Equals(anuncio.StatusAnuncio, "P", StringComparison.OrdinalIgnoreCase) || anuncio.IdVenda != 0)
            {
                TempData["ErrorMessage"] = "Este veiculo esta com compra pendente no momento.";
                return RedirectToAction("Details", "Anuncio", new { id = anuncio.Id });
            }

            if (!ModelState.IsValid)
            {
                PopulatePreview(anuncio, veiculo);
                return View(model);
            }

            try
            {
                var pessoa = pessoaService.GetByCpf(model.Cpf) ?? pessoaService.GetByEmail(model.Email);
                if (pessoa == null)
                {
                    pessoa = new Pessoa
                    {
                        Nome = model.Nome,
                        Email = model.Email,
                        Cpf = model.Cpf,
                        Telefone = model.Telefone
                    };
                    pessoaService.Create(pessoa);
                }

                var venda = new Venda
                {
                    IdConcessionaria = veiculo.IdConcessionaria,
                    IdPessoa = pessoa.Id,
                    DataVenda = DateTime.Today,
                    ValorFinal = model.ValorFinal,
                    FormaPagamento = model.FormaPagamento
                };

                vendaService.Create(venda);

                anuncio.IdVenda = venda.Id;
                anuncio.StatusAnuncio = "P"; // P = Pendente
                anuncioService.Edit(anuncio);

                TempData["SuccessMessage"] = "Solicitacao registrada. Aguarde confirmacao da concessionaria.";
                return RedirectToAction(nameof(Confirmacao));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                if (message.Contains("cpf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(model.Cpf), "CPF invalido. Digite somente 11 numeros.");
                }
                else if (message.Contains("telefone", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(model.Telefone), "Telefone invalido. Digite somente 10 ou 11 numeros.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nao foi possivel concluir a compra. Verifique os dados informados.");
                }
            }

            PopulatePreview(anuncio, veiculo);
            return View(model);
        }

        public ActionResult Confirmacao()
        {
            return View();
        }

        private void PopulatePreview(Anuncio anuncio, Veiculo veiculo)
        {
            var marca = marcaService.Get(veiculo.IdMarca);
            var modelo = modeloService.Get(veiculo.IdModelo);
            var concessionaria = concessionariaService.Get(veiculo.IdConcessionaria);

            ViewBag.NomeVeiculo = $"{marca?.Nome} {modelo?.Nome}".Trim();
            ViewBag.Ano = veiculo.Ano;
            ViewBag.Preco = veiculo.Preco;
            ViewBag.Cor = veiculo.Cor;
            ViewBag.Quilometragem = veiculo.Quilometragem;
            ViewBag.Concessionaria = concessionaria?.Nome ?? "Concessionaria";
            ViewBag.Descricao = string.IsNullOrWhiteSpace(anuncio.Descricao) ? "Sem descricao informada." : anuncio.Descricao;
        }

        private static string NormalizeDigits(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : Regex.Replace(value, "[^0-9]", string.Empty);
        }
    }
}
