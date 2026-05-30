using MarcasAutos.Application.MarcasAutos;
using Microsoft.Extensions.DependencyInjection;

namespace MarcasAutos.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<GetMarcasAutosUseCase>();
    services.AddScoped<CreateMarcaAutoUseCase>();

    return services;
  }
}
