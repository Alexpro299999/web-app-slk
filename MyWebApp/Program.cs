using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, x => x.UseVector())); // Включаем поддержку векторов

//builder.Services.AddScoped<IEmbeddingGenerator, MockEmbeddingGenerator>(); 

builder.Services.AddScoped<IOpenAiService, OpenAiService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddRazorPages();

var app = builder.Build();

var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    context.Database.Migrate();

    //SeedData.Initialize(services);

    try
    {
        var service = services.GetRequiredService<EquipmentService>();
        await service.GenerateEmbeddingsForEmptyAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Внимание: Ошибка генерации векторов (Ollama работает?): {ex.Message}");
    }
}

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