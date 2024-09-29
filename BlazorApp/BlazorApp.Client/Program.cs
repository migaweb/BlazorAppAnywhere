using BlazorApp.Client;
using BlazorApp.Client.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<BlazorEventHelper>();
builder.Services.AddScoped<IEnvironmentService, WebFormsEnvironmentService>();
builder.Services.AddScoped<StateStore>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7124/") });

builder.RootComponents.RegisterCustomElement<Greetings>("my-greeting");
builder.RootComponents.RegisterCustomElement<AnotherComponent>("blazor-environment");

await builder.Build().RunAsync();
