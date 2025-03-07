using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebSystemOne.Models;

namespace WebSystemOne.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly IUserStore<ApplicationUserModel> _userStore;
        private readonly IUserEmailStore<ApplicationUserModel> _emailStore;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUserModel> userManager,
            IUserStore<ApplicationUserModel> userStore,
            SignInManager<ApplicationUserModel> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле \"Электронная почта\" обязательно для заполнения.")]
            [EmailAddress(ErrorMessage = "Некорректный формат электронной почты.")]
            [Display(Name = "Электронная почта")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Поле \"Пароль\" обязательно для заполнения.")]
            [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} и не более {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Поле \"Имя\" обязательно для заполнения.")]
            [StringLength(50, ErrorMessage = "Имя должно содержать от {2} до {1} символов.", MinimumLength = 2)]
            [RegularExpression(@"^[A-Za-zА-Яа-яЁё\s\-]+$", ErrorMessage = "Имя может содержать только буквы, пробелы и дефисы.")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Поле \"Фамилия\" обязательно для заполнения.")]
            [StringLength(50, ErrorMessage = "Фамилия должна содержать от {2} до {1} символов.", MinimumLength = 2)]
            [RegularExpression(@"^[A-Za-zА-Яа-яЁё\s\-]+$", ErrorMessage = "Фамилия может содержать только буквы, пробелы и дефисы.")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Поле \"Отчество\" обязательно для заполнения.")]
            [StringLength(50, ErrorMessage = "Отчество должно содержать от {2} до {1} символов.", MinimumLength = 2)]
            [RegularExpression(@"^[A-Za-zА-Яа-яЁё\s\-]+$", ErrorMessage = "Отчество может содержать только буквы, пробелы и дефисы.")]
            [Display(Name = "Отчество")]
            public string MiddleName { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.MiddleName = Input.MiddleName;

                user.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь создал новую учетную запись с паролем.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }


        private ApplicationUserModel CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUserModel>();
            }
            catch
            {
                throw new InvalidOperationException($"Невозможно создать экземпляр '{nameof(ApplicationUserModel)}'. " +
                    $"Убедитесь, что '{nameof(ApplicationUserModel)}' не является абстрактным классом и имеет конструктор без параметров, или переопределите страницу регистрации в /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUserModel> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Текущий магазин пользователей не поддерживает электронную почту.");
            }
            return (IUserEmailStore<ApplicationUserModel>)_userStore;
        }
    }
}
