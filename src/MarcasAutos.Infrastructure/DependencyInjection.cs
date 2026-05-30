using MarcasAutos.Application.Common;
using MarcasAutos.Domain.MarcasAutos;
using MarcasAutos.Infrastructure.Messaging;
using MarcasAutos.Infrastructure.Persistence;
using MarcasAutos.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarcasAutos.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration
  )
  {
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    services.AddScoped<IMarcaAutoRepository, MarcaAutoRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IMessageBus, MassTransitMessageBus>();

    services.AddMassTransit(config =>
    {
      config.AddConsumer<MarcaAutoCreadaConsumer>();

      config.UsingInMemory((context, cfg) =>
          {
            cfg.ConfigureEndpoints(context);
          });
    });

    return services;
  }
}
