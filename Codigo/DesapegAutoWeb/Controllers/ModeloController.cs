using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DesapegAutoWeb.Controllers
{
    public class ModeloController : Controller
    {
        private readonly IModeloService modeloService;
        private readonly IMarcaService marcaService;
        private readonly ICategoriaService categoriaService;
        private readonly IMapper mapper;

        public ModeloController(
            IModeloService modeloService,
            IMarcaService marcaService,
            ICategoriaService categoriaService,
            IMapper mapper)
        {
            this.modeloService = modeloService;
            this.marcaService = marcaService;
            this.categoriaService = categoriaService;
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            var listaModelos = modeloService.GetAll();
            var listaModelosVm = mapper.Map<IEnumerable<ModeloViewModel>>(listaModelos);
            return RenderIndex(listaModelosVm);
        }

        public ActionResult Details(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }

            var vm = mapper.Map<ModeloViewModel>(modelo);
            return View(vm);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Create(ModeloViewModel modeloViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Preencha os campos obrigatorios para cadastrar o modelo.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var modelo = mapper.Map<Modelo>(modeloViewModel);
                modeloService.Create(modelo);
                TempData["SuccessMessage"] = "Modelo cadastrado com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel salvar o modelo devido a uma inconsistencia no banco de dados.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }

            var vm = mapper.Map<ModeloViewModel>(modelo);
            PopulateCombos();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public ActionResult Edit(int id, ModeloViewModel modeloViewModel)
        {
            if (id != modeloViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateCombos();
                return View(modeloViewModel);
            }

            try
            {
                var modelo = mapper.Map<Modelo>(modeloViewModel);
                modeloService.Edit(modelo);
                TempData["SuccessMessage"] = "Modelo atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateCombos();
                return View(modeloViewModel);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel atualizar o modelo devido a uma inconsistencia no banco de dados.");
                PopulateCombos();
                return View(modeloViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }

            var vm = mapper.Map<ModeloViewModel>(modelo);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                modeloService.Delete(id);
                TempData["SuccessMessage"] = "Modelo removido com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel remover o modelo porque ele esta em uso.";
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult GetByMarca(int idMarca)
        {
            var modelosDto = modeloService.GetByMarca(idMarca);
            var vms = modelosDto.Select(m => new ModeloViewModel
            {
                Id = m.Id,
                Nome = m.Nome ?? string.Empty,
                Versoes = m.Versoes,
                IdMarca = m.IdMarca,
                IdCategoria = m.IdCategoria
            });

            return RenderIndex(vms);
        }

        public ActionResult GetByCategoria(string categoria)
        {
            var modelosDto = modeloService.GetByCategoria(categoria);
            var vms = modelosDto.Select(m => new ModeloViewModel
            {
                Id = m.Id,
                Nome = m.Nome ?? string.Empty,
                Versoes = m.Versoes,
                IdMarca = m.IdMarca,
                IdCategoria = m.IdCategoria,
                Categoria = categoria
            });

            return RenderIndex(vms);
        }

        public ActionResult GetByNome(string nome)
        {
            var modelosDto = modeloService.GetByNome(nome);
            var vms = modelosDto.Select(m => new ModeloViewModel
            {
                Id = m.Id,
                Nome = m.Nome ?? string.Empty,
                Versoes = m.Versoes,
                IdMarca = m.IdMarca,
                IdCategoria = m.IdCategoria
            });

            return RenderIndex(vms);
        }

        private ActionResult RenderIndex(IEnumerable<ModeloViewModel> modelos)
        {
            var lista = modelos.ToList();
            var marcas = marcaService.GetAll().ToDictionary(m => m.Id, m => m.Nome);
            var categorias = categoriaService.GetAll().ToDictionary(c => c.Id, c => c.Nome);

            foreach (var item in lista)
            {
                if (marcas.TryGetValue(item.IdMarca, out var nomeMarca))
                {
                    item.NomeMarca = nomeMarca;
                }

                if (categorias.TryGetValue(item.IdCategoria, out var nomeCategoria))
                {
                    item.NomeCategoria = nomeCategoria;
                    if (string.IsNullOrWhiteSpace(item.Categoria))
                    {
                        item.Categoria = nomeCategoria;
                    }
                }
            }

            PopulateCombos();
            return View("Index", lista);
        }

        private void PopulateCombos()
        {
            ViewBag.Marcas = marcaService.GetAll()
                .OrderBy(m => m.Nome)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Nome
                })
                .ToList();

            ViewBag.Categorias = categoriaService.GetAll()
                .OrderBy(c => c.Nome)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome
                })
                .ToList();
        }
    }
}
