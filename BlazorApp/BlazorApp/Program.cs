using BlazorApp.Client;
using BlazorApp.Client.Components;
using BlazorApp.Client.Pages;
using BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<BlazorEventHelper>();
builder.Services.AddScoped<IEnvironmentService, WebFormsEnvironmentService>();
builder.Services.AddScoped<StateStore>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
  Console.WriteLine("CORS middleware is running.");
  options.AddPolicy("AllowLocalhost",
            builder =>
            {
              builder.AllowAnyOrigin().AllowAnyHeader()
                   .AllowAnyMethod();
            });
});

builder.Services.AddControllers();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseWebAssemblyDebugging();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

app.Run();
