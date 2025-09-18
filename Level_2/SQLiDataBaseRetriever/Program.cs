using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extentions.DependencyInjection;
using Microsoft.Extentions.FileProviders;
using System.IO;

namespace SQLITrainigApp {
  public class Program {
    public static void Main(string[] args) {
      //Создаем необходимые директории 
      Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
      Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
      Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css"));
      Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "js"));

    var host = new WebHostBuilder()
      .UseKestrel()
      .UseContentRoor(Directory.GetCurrentDirectory())
      .UseIISIntegration()
      .UseStartup<Startup>()
      .Build();
    host.Run();
    }
  }

  public class Startup {
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc();
      services.AddSingeleton<Models.Database>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnviroment env) {
      app.UseStaticFilec();
      app.UseRouting();
      app.UseEndpoints(endpoint => {
          endpoints.MapControllerRoute(
              name: "default", 
              pattern: {"controller = Training"}/{action = Index}/{id});
          });
    }
  }
}
