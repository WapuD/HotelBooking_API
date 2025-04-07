using HotelBooking_WEB.Data;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // ������������ ��� �������� ������ ������ � ������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // ����� ����� ������
    options.Cookie.HttpOnly = true; // ������������� cookie ������ ��� HTTP
    options.Cookie.IsEssential = true; // Cookie ���������� ��� ������ ����������
});

builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddMvcCore();
builder.Services.AddControllersWithViews();

builder.Services
    .AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7059/api"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
