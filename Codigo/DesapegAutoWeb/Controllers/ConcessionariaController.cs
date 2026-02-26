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
            if (c == null) return NotFound();
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
            // Strip non-digits from CNPJ and Telefone before validation
            if (!string.IsNullOrEmpty(vm.Cnpj))
                vm.Cnpj = System.Text.RegularExpressions.Regex.Replace(vm.Cnpj, @"\D", "");
            if (!string.IsNullOrEmpty(vm.Telefone))
                vm.Telefone = System.Text.RegularExpressions.Regex.Replace(vm.Telefone, @"\D", "");

            ModelState.Remove(nameof(vm.Cnpj));
            ModelState.Remove(nameof(vm.Telefone));
            TryValidateModel(vm);

            if (!ModelState.IsValid)
            {
                return RenderIndexWithForm(vm);
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
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(vm.Senha), error.Description);
                    }
                    return RenderIndexWithForm(vm);
                }

                await userManager.AddToRoleAsync(user, "Funcionario");
                await userManager.AddClaimAsync(user, new Claim("ConcessionariaId", c.Id.ToString()));
                TempData["SuccessMessage"] = "Concessionaria cadastrada com sucesso.";
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RenderIndexWithForm(vm);
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                if (message.Contains("cnpj", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(vm.Cnpj), "CNPJ invalido. Digite somente 14 numeros.");
                }
                else if (message.Contains("telefone", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(vm.Telefone), "Telefone invalido. Digite somente 10 ou 11 numeros.");
                }
                else if (message.Contains("senha", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(vm.Senha), "Senha invalida para o tamanho permitido.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao salvar. Verifique os dados informados.");
                }
                return RenderIndexWithForm(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var c = concessionariaService.Get(id);
            if (c == null) return NotFound();
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
            if (c == null) return NotFound();
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

        private ActionResult RenderIndexWithForm(ConcessionariaViewModel vm)
        {
            var lista = concessionariaService.GetAll();
            var indexVm = mapper.Map<List<ConcessionariaViewModel>>(lista);
            ViewBag.FormData = vm;
            return View("Index", indexVm);
        }
    }
}
