using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

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
 private readonly IMapper mapper;

        public VendaController(
 IVendaService vendaService,
       IConcessionariaService concessionariaService,
   IPessoaService pessoaService,
IMapper mapper,
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

    return View();
}

        [HttpPost]
        [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Funcionario")]
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

private void PopulateViewBags(VendaViewModel? model = null)
   {
    ViewBag.IdConcessionaria = new SelectList(concessionariaService.GetAll(), "Id", "Nome", model?.IdConcessionaria);
            ViewBag.IdPessoa = new SelectList(pessoaService.GetAll(), "Id", "Nome", model?.IdPessoa);
        }
    }
}
