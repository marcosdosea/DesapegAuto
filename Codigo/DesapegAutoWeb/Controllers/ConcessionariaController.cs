using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Service;
using DesapegAutoWeb.Models;
using DesapegAutoWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace DesapegAutoWeb.Controllers
{
    [Authorize]
    public class ConcessionariaController : Controller
    {
        private readonly IConcessionariaService concessionariaService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public ConcessionariaController(
            IConcessionariaService concessionariaService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            this.concessionariaService = concessionariaService;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var lista = concessionariaService.GetAll();
            var vm = mapper.Map<List<ConcessionariaViewModel>>(lista);
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            var c = concessionariaService.Get(id);
            var vm = mapper.Map<ConcessionariaViewModel>(c);
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(ConcessionariaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Preencha os campos obrigatorios para cadastrar a concessionaria.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Create(c);

                var user = new ApplicationUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, vm.Senha);
                if (!result.Succeeded)
                {
                    concessionariaService.Delete(c.Id);
                    TempData["ErrorMessage"] = string.Join(" ", result.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Index));
                }

                await userManager.AddToRoleAsync(user, "Funcionario");
                await userManager.AddClaimAsync(user, new Claim("ConcessionariaId", c.Id.ToString()));
                TempData["SuccessMessage"] = "Concessionaria cadastrada com sucesso.";
            }
            catch (ServiceException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nao foi possivel salvar a concessionaria devido a uma inconsistencia no banco de dados. Verifique se a senha tem no maximo 8 caracteres e se todos os campos obrigatorios existem no banco.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var c = concessionariaService.Get(id);
            var vm = mapper.Map<ConcessionariaViewModel>(c);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, ConcessionariaViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Edit(c);
                TempData["SuccessMessage"] = "Concessionaria atualizada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel atualizar a concessionaria devido a uma inconsistencia no banco de dados.");
                return View(vm);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var c = concessionariaService.Get(id);
            var vm = mapper.Map<ConcessionariaViewModel>(c);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, ConcessionariaViewModel vm)
        {
            try
            {
                concessionariaService.Delete(id);
                TempData["SuccessMessage"] = "Concessionaria removida com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel remover a concessionaria porque ela esta em uso.");
                return View(vm);
            }
        }

        [Authorize(Roles = "Funcionario")]
        public ActionResult MeuCadastro()
        {
            var concessionariaId = GetConcessionariaIdFromUser();
            if (!concessionariaId.HasValue)
            {
                return Forbid();
            }

            var c = concessionariaService.Get(concessionariaId.Value);
            if (c == null)
            {
                return NotFound();
            }

            var vm = mapper.Map<ConcessionariaViewModel>(c);
            ViewBag.SelfEdit = true;
            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario")]
        public ActionResult MeuCadastro(ConcessionariaViewModel vm)
        {
            var concessionariaId = GetConcessionariaIdFromUser();
            if (!concessionariaId.HasValue || concessionariaId.Value != vm.Id)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.SelfEdit = true;
                return View("Edit", vm);
            }

            try
            {
                var c = mapper.Map<Concessionaria>(vm);
                concessionariaService.Edit(c);
                TempData["SuccessMessage"] = "Concessionaria atualizada com sucesso.";
                return RedirectToAction("Index", "Anuncio");
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.SelfEdit = true;
                return View("Edit", vm);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nao foi possivel atualizar a concessionaria devido a uma inconsistencia no banco de dados.");
                ViewBag.SelfEdit = true;
                return View("Edit", vm);
            }
        }

        private int? GetConcessionariaIdFromUser()
        {
            var claim = User.FindFirst("ConcessionariaId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}
