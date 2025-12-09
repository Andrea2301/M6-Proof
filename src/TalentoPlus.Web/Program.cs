using TalentoPlus.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// HTTP CONTEXT ACCESSOR (si lo necesitas)
builder.Services.AddHttpContextAccessor();

// HTTP CLIENT
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5206";
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// SERVICIO API
builder.Services.AddScoped<IApiService, ApiService>();

// MVC
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();