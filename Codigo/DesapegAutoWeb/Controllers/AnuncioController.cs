using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Core.Exceptions;
using System.Linq;

namespace DesapegAutoWeb.Controllers
{
    public class AnuncioController : Controller
    {
        private readonly IAnuncioService anuncioService;
        private readonly IVeiculoService veiculoService;
        private readonly IModeloService modeloService;
        private readonly IMarcaService marcaService;
        private readonly IConcessionariaService concessionariaService;
        private readonly IMapper mapper;

        public AnuncioController(
            IAnuncioService anuncioService,
            IVeiculoService veiculoService,
            IModeloService modeloService,
            IMarcaService marcaService,
            IConcessionariaService concessionariaService,
            IMapper mapper)
        {
            this.anuncioService = anuncioService;
            this.veiculoService = veiculoService;
            this.modeloService = modeloService;
            this.marcaService = marcaService;
            this.concessionariaService = concessionariaService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Index()
        {
            var anuncios = anuncioService.GetAll();
            var concessionariaId = GetConcessionariaIdFromUser();
            if (concessionariaId.HasValue)
            {
                anuncios = anuncios.Where(a =>
                {
                    var veiculo = veiculoService.Get(a.IdVeiculo);
                    return veiculo != null && veiculo.IdConcessionaria == concessionariaId.Value;
                });
            }
            var model = mapper.Map<IEnumerable<AnuncioViewModel>>(anuncios).ToList();
            foreach (var anuncio in model)
            {
                var veiculo = veiculoService.Get(anuncio.IdVeiculo);
                if (veiculo != null)
                {
                    var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);

                    var marca = marcaService.Get(veiculo.IdMarca);
                    var modelo = modeloService.Get(veiculo.IdModelo);

                    if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                    if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;

                    anuncio.Veiculo = veiculoViewModel;
                }
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null) return NotFound();

            var model = mapper.Map<AnuncioViewModel>(anuncio);

            var veiculo = veiculoService.Get(anuncio.IdVeiculo);
            if (veiculo != null)
            {
                var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);

                var marca = marcaService.Get(veiculo.IdMarca);
                var modelo = modeloService.Get(veiculo.IdModelo);

                if (marca != null) veiculoViewModel.NomeMarca = marca.Nome;
                if (modelo != null) veiculoViewModel.NomeModelo = modelo.Nome;

                model.Veiculo = veiculoViewModel;
            }

            anuncio.Visualizacoes++;
            anuncioService.Edit(anuncio);

            return View(model);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            var model = new AnuncioCreateViewModel();
            var concessionariaId = GetConcessionariaIdFromUser();
            if (concessionariaId.HasValue)
            {
                model.IdConcessionaria = concessionariaId.Value;
                SetConcessionariaDisplay(concessionariaId.Value);
            }
            PopulateCreateViewBags(model.IdMarca, model.IdModelo, model.IdConcessionaria);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(AnuncioCreateViewModel anuncioViewModel)
        {
            var concessionariaId = GetConcessionariaIdFromUser();
            if (concessionariaId.HasValue)
            {
                anuncioViewModel.IdConcessionaria = concessionariaId.Value;
                SetConcessionariaDisplay(concessionariaId.Value);
            }

            if (!ModelState.IsValid)
            {
                PopulateCreateViewBags(anuncioViewModel.IdMarca, anuncioViewModel.IdModelo, anuncioViewModel.IdConcessionaria);
                return View(anuncioViewModel);
            }

            try
            {
                var veiculo = new Veiculo
                {
                    IdConcessionaria = anuncioViewModel.IdConcessionaria,
                    IdMarca = anuncioViewModel.IdMarca,
                    IdModelo = anuncioViewModel.IdModelo,
                    Ano = anuncioViewModel.Ano,
                    Cor = anuncioViewModel.Cor,
                    Quilometragem = anuncioViewModel.Quilometragem,
                    Preco = anuncioViewModel.Preco,
                    Placa = anuncioViewModel.Placa
                };

                var veiculoId = veiculoService.Create(veiculo);
                var anuncio = new Anuncio
                {
                    IdVeiculo = veiculoId,
                    IdVenda = 0,
                    Descricao = anuncioViewModel.Descricao ?? string.Empty,
                    Opcionais = string.Join(", ", anuncioViewModel.OpcionaisSelecionados ?? new List<string>()),
                    StatusAnuncio = "D",
                    Visualizacoes = 0
                };

                anuncioService.Create(anuncio);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            PopulateCreateViewBags(anuncioViewModel.IdMarca, anuncioViewModel.IdModelo, anuncioViewModel.IdConcessionaria);
            return View(anuncioViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var anuncio = anuncioService.Get(id);
            if (anuncio == null) return NotFound();
            var model = mapper.Map<AnuncioViewModel>(anuncio);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, AnuncioViewModel anuncioViewModel)
        {
            anuncioService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateCreateViewBags(int? idMarca = null, int? idModelo = null, int? idConcessionaria = null)
        {
            ViewBag.Marcas = new SelectList(marcaService.GetAll(), "Id", "Nome", idMarca);
            ViewBag.Modelos = new SelectList(modeloService.GetAll(), "Id", "Nome", idModelo);
            ViewBag.Concessionarias = new SelectList(concessionariaService.GetAll(), "Id", "Nome", idConcessionaria);
        }

        private int? GetConcessionariaIdFromUser()
        {
            var claim = User.FindFirst("ConcessionariaId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }

        private void SetConcessionariaDisplay(int id)
        {
            var concessionaria = concessionariaService.Get(id);
            ViewBag.ConcessionariaNome = concessionaria?.Nome ?? $"#{id}";
        }
    }
}
