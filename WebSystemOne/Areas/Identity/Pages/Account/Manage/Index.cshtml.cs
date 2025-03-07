// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebSystemOne.Models;

namespace WebSystemOne.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;

        public IndexModel(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Логин пользователя (не редактируется)
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        // Редактируемые данные: ФИО
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле \"Имя\" обязательно для заполнения.")]
            [Display(Name = "Имя")]
            [StringLength(50, ErrorMessage = "Имя должно содержать от {2} до {1} символов.", MinimumLength = 2)]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Поле \"Фамилия\" обязательно для заполнения.")]
            [Display(Name = "Фамилия")]
            [StringLength(50, ErrorMessage = "Фамилия должна содержать от {2} до {1} символов.", MinimumLength = 2)]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Поле \"Отчество\" обязательно для заполнения.")]
            [Display(Name = "Отчество")]
            [StringLength(50, ErrorMessage = "Отчество должно содержать от {2} до {1} символов.", MinimumLength = 2)]
            public string MiddleName { get; set; }
        }

        private async Task LoadAsync(ApplicationUserModel user)
        {
            Username = await _userManager.GetUserNameAsync(user);

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Не удалось загрузить пользователя с ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Не удалось загрузить пользователя с ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Обновляем ФИО
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.MiddleName = Input.MiddleName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "Ошибка при обновлении профиля.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Ваш профиль успешно обновлен.";
            return RedirectToPage();
        }
    }
}
