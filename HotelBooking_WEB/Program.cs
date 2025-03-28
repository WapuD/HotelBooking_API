using HotelBooking_WEB.Data;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddMvcCore();
builder.Services.AddControllersWithViews();

builder.Services
    .AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7059/api"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
