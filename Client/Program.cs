using job_board.Client;
using job_board.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddMudServices();
        //builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<TokenAuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthStateProvider>());

        /*if (!builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        }
        else
        {
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        }*/
        builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

        var baseAddress = builder.Configuration["Base_Address"];
        builder.Services.AddHttpClient("httpClient", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("httpClient"));

        await builder.Build().RunAsync();
    }
}