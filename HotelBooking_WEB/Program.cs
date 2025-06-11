using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Diagnostics;
using Refit;
using System.Text.Json.Serialization;
using System.Text.Json;
using HotelBooking_WEB.Data.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // Используется для хранения данных сессии в памяти
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Время жизни сессии
    options.Cookie.HttpOnly = true; // Устанавливаем cookie только для HTTP
    options.Cookie.IsEssential = true; // Cookie необходимы для работы приложения
});

builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddMvcCore();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services
    .AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:44353/api"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = error.Error.Message,
                stackTrace = error.Error.StackTrace
            }));
        }
    });
});

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
