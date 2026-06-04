using ContaGotas.Blazor.Components;
using ContaGotas;

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar o suporte para Componentes Razor Interativos (Server Mode)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. CONFIGURAÇÃO DA ARQUITETURA MVC NO CONTENTOR DE DI
// Regista o serviço de API como Singleton (uma única instância partilhada para a app)
builder.Services.AddSingleton<ICombustivelService, CombustivelApiService>();

// Regista o Model e o Controller como Scoped (uma instância isolada por cada utilizador/aba do navegador)
// O .NET vai ler os construtores automaticamente: quando o Controller pedir um Model, o DI injeta-o.
builder.Services.AddScoped<Model>();
builder.Services.AddScoped<Controller>();

var app = builder.Build();

// 3. Configuração do Pipeline de pedidos HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Mapeia o componente principal e ativa o modo interativo para os eventos funcionarem em tempo real
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();