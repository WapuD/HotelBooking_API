using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using HotelBooking_WEB.Data.Service;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using HotelBooking_API.Data.Other;
using System.Linq.Expressions;

namespace HotelBooking_WEB.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;
        private readonly IApiClient _apiClient;
        private readonly IEmailService _emailService;

        [BindProperty]
        public UpdateUserDto User { get; set; }

        [BindProperty]
        public ChangePasswordDto ChangePassword { get; set; }
        public string PasswordChangeMessage { get; set; }
        public bool PasswordChangeSuccess { get; set; }


        public ProfileModel(ILogger<ProfileModel> logger, IApiClient apiClient, IEmailService emailService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _emailService = emailService;
        }

        public async Task OnGet()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _apiClient.GetUser(userId);
            User = new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName,
                Phone = user.Phone,
                CompanyId = user.CompanyId
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _apiClient.UpdateUser(User);
                TempData["DataChangeSuccess"] = true;
                TempData["DataChangeMessage"] = "Пароль успешно изменён.";
/*                await _emailService.SendEmailAsync(HttpContext.Session.GetString("UserEmail").ToString(), "Личный кабинет", "Линые данные вашего профиля были успешно изменены");*/
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var user = await _apiClient.GetUser(userId);

                if (!PasswordHasher.VerifyPassword(ChangePassword.OldPassword, user.PasswordHash))
                {
                    ModelState.AddModelError("ChangePassword.OldPassword", "Старый пароль неверен");
                    PasswordChangeSuccess = false;
                    return Page();
                }

                user.PasswordHash = PasswordHasher.HashPassword(ChangePassword.NewPassword);
                var newUserPassword = new UpdateUserPasswordDto
                {
                    UserId = user.Id,
                    PasswordHash = user.PasswordHash
                };
                var flag = await _apiClient.UpdateUserPassword(newUserPassword);

                // Чтобы модальное окно закрылось после успешной смены, можно передать флаг в ViewData или TempData
                TempData["ClosePasswordModal"] = true;
                TempData["PasswordChangeSuccess"] = true;
                TempData["PasswordChangeMessage"] = "Пароль успешно изменён.";

                User = new UpdateUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    CompanyId = user.CompanyId
                };

                return Page();
            }
            catch
            {
                PasswordChangeMessage = "Проверьте корректность введённых данных.";
                PasswordChangeSuccess = false;
                return Page();
            }
        }

    }
}
