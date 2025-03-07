// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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
            [Required]
            [EmailAddress]
            [Display(Name = "Электронная почта")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} и не более {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Имя")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required]
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
                // Если требуется, можно установить EmailConfirmed = true сразу
                user.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь создал новую учетную запись с паролем.");

                    // Авторизуем пользователя сразу после регистрации
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Если произошла ошибка, отобразим форму заново
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
