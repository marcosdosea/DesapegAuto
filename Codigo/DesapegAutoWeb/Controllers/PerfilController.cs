using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DesapegAutoWeb.Areas.Identity.Data;

namespace DesapegAutoWeb.Controllers
{
    /// <summary>
    /// Exemplo de controller protegido por autenticação
    /// Todas as actions deste controller requerem que o usuário esteja logado
    /// </summary>
    [Authorize]
  public class PerfilController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PerfilController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
      _userManager = userManager;
     _signInManager = signInManager;
        }

        /// <summary>
     /// Página de perfil do usuário - Requer autenticação
     /// </summary>
        public async Task<IActionResult> Index()
        {
         // Obter o usuário atual
            var user = await _userManager.GetUserAsync(User);
     
     if (user == null)
      {
      return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
       }

            // Criar um modelo para a view com informações do usuário
            var model = new PerfilViewModel
      {
           Email = user.Email!,
           UserName = user.UserName!,
         PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
      PhoneNumberConfirmed = user.PhoneNumberConfirmed,
              TwoFactorEnabled = user.TwoFactorEnabled
       };

            return View(model);
        }

        /// <summary>
  /// Exemplo de action que permite acesso anônimo mesmo dentro de um controller protegido
     /// </summary>
        [AllowAnonymous]
        public IActionResult Publico()
  {
      return View();
        }
    }

    /// <summary>
    /// ViewModel para exibir informações do perfil do usuário
    /// </summary>
    public class PerfilViewModel
    {
        public string Email { get; set; } = string.Empty;
      public string UserName { get; set; } = string.Empty;
     public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
     public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
