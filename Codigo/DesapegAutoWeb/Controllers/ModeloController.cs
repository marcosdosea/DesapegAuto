using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DesapegAutoWeb.Controllers
{
    public class ModeloController : Controller
    {
        private readonly IModeloService modeloService;
        private readonly IMarcaService marcaService;
        private readonly IMapper mapper;

        public ModeloController(IModeloService modeloService, IMarcaService marcaService, IMapper mapper)
        {
            this.modeloService = modeloService;
            this.marcaService = marcaService;
            this.mapper = mapper;
        }

        // GET: Modelo
        public ActionResult Index()
        {
            var listaModelos = modeloService.GetAll();
            // Por enquanto, usando Modelo diretamente em vez de ModeloViewModel
            return View(listaModelos);
        }

        // GET: Modelo/Details/5
        public ActionResult Details(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }
            return View(modelo);
        }

        // GET: Modelo/Create
        public ActionResult Create()
        {
            // Por enquanto retornamos direto a view, mais tarde ajustaremos para incluir o dropdown de marcas
            return View();
        }

        // POST: Modelo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Modelo modelo)
        {
            if (ModelState.IsValid)
            {
                modeloService.Create(modelo);
                return RedirectToAction(nameof(Index));
            }
            return View(modelo);
        }

        // GET: Modelo/Edit/5
        public ActionResult Edit(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }
            return View(modelo);
        }

        // POST: Modelo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Modelo modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                modeloService.Edit(modelo);
                return RedirectToAction(nameof(Index));
            }
            return View(modelo);
        }

        // GET: Modelo/Delete/5
        public ActionResult Delete(int id)
        {
            var modelo = modeloService.Get(id);
            if (modelo == null)
            {
                return NotFound();
            }
            return View(modelo);
        }

        // POST: Modelo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            modeloService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // Additional Methods from IModeloService
        public ActionResult GetByMarca(int idMarca)
        {
            var modelos = modeloService.GetByMarca(idMarca);
            return View("Index", modelos);
        }

        public ActionResult GetByCategoria(string categoria)
        {
            var modelos = modeloService.GetByCategoria(categoria);
            return View("Index", modelos);
        }

        public ActionResult GetByNome(string nome)
        {
            var modelos = modeloService.GetByNome(nome);
            return View("Index", modelos);
        }
    }
}
