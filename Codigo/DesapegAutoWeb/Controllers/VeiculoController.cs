using AutoMapper;
using Core;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace DesapegAutoWeb.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly IVeiculoService veiculoService;
        private readonly IMarcaService marcaService;
        private readonly IModeloService modeloService;
        private readonly IMapper mapper;

        public VeiculoController(IVeiculoService veiculoService, IMarcaService marcaService, IModeloService modeloService, IMapper mapper)
        {
            this.veiculoService = veiculoService;
            this.marcaService = marcaService;
            this.modeloService = modeloService;
            this.mapper = mapper;
        }

        // GET: Veiculo
        public ActionResult Index()
        {
            var listaVeiculos = veiculoService.GetAll();
            var listaVeiculosViewModel = mapper.Map<List<VeiculoViewModel>>(listaVeiculos);

            var marcas = marcaService.GetAll().ToDictionary(m => m.Id, m => m.Nome);
            var modelos = modeloService.GetAll().ToDictionary(m => m.Id, m => m.Nome);

            foreach (var veiculo in listaVeiculosViewModel)
            {
                if (veiculo.IdMarca > 0 && marcas.TryGetValue(veiculo.IdMarca, out var marcaNome))
                {
                    veiculo.NomeMarca = marcaNome;
                }
                else
                {
                    veiculo.NomeMarca = "Marca Indefinida";
                }

                if (veiculo.IdModelo > 0 && modelos.TryGetValue(veiculo.IdModelo, out var modeloNome))
                {
                    veiculo.NomeModelo = modeloNome;
                }
                else
                {
                    veiculo.NomeModelo = "Modelo Indefinido";
                }
            }

            return View(listaVeiculosViewModel);
        }

        // GET: Veiculo/Details/5
        public ActionResult Details(int id)
        {
            var veiculo = veiculoService.Get(id);
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            if (veiculoViewModel != null)
            {
                var marca = marcaService.Get(veiculoViewModel.IdMarca);
                var modelo = modeloService.Get(veiculoViewModel.IdModelo);
                if (marca != null)
                {
                    veiculoViewModel.NomeMarca = marca.Nome;
                }
                if (modelo != null)
                {
                    veiculoViewModel.NomeModelo = modelo.Nome;
                }
            }
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
                var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                veiculoService.Create(veiculo);
                return RedirectToAction(nameof(Index));
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Edit/5
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var veiculo = veiculoService.Get(id);
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
                var veiculo = mapper.Map<Veiculo>(veiculoViewModel);
                veiculoService.Edit(veiculo);
                return RedirectToAction(nameof(Index));
            }
            return View(veiculoViewModel);
        }

        // GET: Veiculo/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var veiculo = veiculoService.Get(id);
            var veiculoViewModel = mapper.Map<VeiculoViewModel>(veiculo);
            return View(veiculoViewModel);
        }

        // POST: Veiculo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, VeiculoViewModel veiculoViewModel)
        {
            veiculoService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public RedirectToActionResult Edit(VeiculoViewModel veiculoViewModel)
        {
            throw new NotImplementedException();
        }
    }
}