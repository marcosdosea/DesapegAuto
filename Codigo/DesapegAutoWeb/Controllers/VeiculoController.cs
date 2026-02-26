using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DesapegAutoWeb.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly IVeiculoService veiculoService;
        private readonly IAnuncioService anuncioService;
        private readonly IMarcaService marcaService;
        private readonly IModeloService modeloService;
        private readonly IMapper mapper;

        public VeiculoController(
            IVeiculoService veiculoService,
            IAnuncioService anuncioService,
            IMarcaService marcaService,
            IModeloService modeloService,
            IMapper mapper)
        {
            this.veiculoService = veiculoService;
            this.anuncioService = anuncioService;
            this.marcaService = marcaService;
            this.modeloService = modeloService;
            this.mapper = mapper;
        }

        // GET: Veiculo
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Index()
        {
            var listaVeiculos = veiculoService.GetAll();
            var listaVeiculosViewModel = mapper.Map<IEnumerable<VeiculoViewModel>>(listaVeiculos).ToList();

            var anunciosPorVeiculo = anuncioService.GetAll()
                .GroupBy(a => a.IdVeiculo)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var veiculo in listaVeiculosViewModel)
            {
                var marca = marcaService.Get(veiculo.IdMarca);
                var modelo = modeloService.Get(veiculo.IdModelo);

                veiculo.NomeMarca = marca?.Nome ?? "-";
                veiculo.NomeModelo = modelo?.Nome ?? "-";

                if (anunciosPorVeiculo.TryGetValue(veiculo.Id, out var anuncio))
                {
                    veiculo.Status = MapStatus(anuncio.StatusAnuncio);
                }
                else
                {
                    veiculo.Status = "Sem anúncio";
                }
            }

            return View(listaVeiculosViewModel);
        }

        // GET: Veiculo/Details/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Details(int id)
        {
            var veiculo = veiculoService.Get(id);
            if (veiculo == null) return NotFound();
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);

            var marca = marcaService.Get(veiculo.IdMarca);
            var modelo = modeloService.Get(veiculo.IdModelo);
            veiculoViewModel.NomeMarca = marca?.Nome ?? "-";
            veiculoViewModel.NomeModelo = modelo?.Nome ?? "-";

            var anuncio = anuncioService.GetAll().FirstOrDefault(a => a.IdVeiculo == veiculo.Id);
            veiculoViewModel.Status = anuncio != null ? MapStatus(anuncio.StatusAnuncio) : "Sem anúncio";

            return View(veiculoViewModel);
        }

        // GET: Veiculo/Create
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Veiculo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(VeiculoViewModel veiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                    veiculoService.Create(veiculo);
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Edit/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var veiculo = veiculoService.Get(id);
            if (veiculo == null) return NotFound();
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id, VeiculoViewModel veiculoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                    veiculoService.Edit(veiculo);
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Delete/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Delete(int id)
        {
            var veiculo = veiculoService.Get(id);
            if (veiculo == null) return NotFound();
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Delete(int id, VeiculoViewModel veiculoViewModel)
        {
            try
            {
                var anuncio = anuncioService.GetAll().FirstOrDefault(a => a.IdVeiculo == id);
                if (anuncio != null)
                {
                    ModelState.AddModelError(string.Empty, "Nao e possivel remover este veiculo porque existe um anuncio vinculado. Remova o anuncio primeiro.");
                    var veiculo = veiculoService.Get(id);
                    if (veiculo != null)
                    {
                        veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
                    }
                    return View(veiculoViewModel);
                }
                veiculoService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel remover o veiculo porque ele possui registros vinculados.");
                var veiculo = veiculoService.Get(id);
                if (veiculo != null)
                {
                    veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
                }
                return View(veiculoViewModel);
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var veiculo = veiculoService.Get(id);
                if (veiculo != null)
                {
                    veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
                }
                return View(veiculoViewModel);
            }
        }

        private static string MapStatus(string? statusAnuncio)
        {
            return statusAnuncio switch
            {
                "V" => "Vendido",
                "P" => "Pendente",
                "I" => "Indisponível",
                _ => "Disponível"
            };
        }
    }
}
