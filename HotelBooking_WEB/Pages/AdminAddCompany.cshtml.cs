using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;


public class AddCompanyModel : PageModel
{
    public IFormFile LogoFile { get; set; }
    private readonly IApiClient _apiClient;

    public AddCompanyModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty]
    public CompanyCreateDto NewCompany { get; set; } = new CompanyCreateDto();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (LogoFile != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CompanyImg");
            Directory.CreateDirectory(uploadsFolder);

            string safeCompanyName = MakeFilenameSafe(NewCompany.Name);

            var existingFiles = Directory.GetFiles(uploadsFolder, $"{safeCompanyName}_*.png");

            int nextNumber = 1;
            if (existingFiles.Length > 0)
            {
                var numbers = existingFiles.Select(f =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(f);
                    var parts = fileName.Split('_');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int n))
                        return n;
                    return 0;
                }).Where(n => n > 0).ToList();

                if (numbers.Any())
                    nextNumber = numbers.Max() + 1;
            }

            var fileName = $"{safeCompanyName}_{nextNumber}.png";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Сохраняем файл как PNG с помощью ImageSharp
            using (var image = await Image.LoadAsync(LogoFile.OpenReadStream()))
            {
                await image.SaveAsync(filePath, new PngEncoder());
            }

            NewCompany.LogoUrl = $"/CompanyImg/{fileName}";
        }


        try
        {
            var result = await _apiClient.CreateCompany(NewCompany);
            TempData["SuccessMessage"] = true;
            TempData["SuccessMessage"] = "Компания успешно добавлена.";
            if (result)
                return RedirectToPage("/AdminBookings");
            ModelState.AddModelError("", "Ошибка при добавлении компании.");
        }
        catch (Refit.ApiException apiEx) when (apiEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var content = await apiEx.GetContentAsAsync<Dictionary<string, List<string>>>();
            if (content != null && content.TryGetValue("Errors", out var errors))
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ошибка валидации при добавлении компании.");
            }
        }
        return Page();

    }

    private string MakeFilenameSafe(string input)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            input = input.Replace(c, '_');
        }
        return input.Replace(" ", "_");
    }

}
