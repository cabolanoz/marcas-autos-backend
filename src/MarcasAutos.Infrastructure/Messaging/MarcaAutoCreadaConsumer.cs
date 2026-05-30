using MarcasAutos.Application.MarcasAutos.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MarcasAutos.Infrastructure.Messaging;

public class MarcaAutoCreadaConsumer : IConsumer<MarcaAutoCreadaEvent>
{
  private readonly ILogger<MarcaAutoCreadaConsumer> _logger;

  public MarcaAutoCreadaConsumer(ILogger<MarcaAutoCreadaConsumer> logger)
  {
    _logger = logger;
  }

  public Task Consume(ConsumeContext<MarcaAutoCreadaEvent> context)
  {
    _logger.LogInformation(
        "Marca de auto creada: {Id} - {Nombre}",
        context.Message.Id,
        context.Message.Nombre
    );

    return Task.CompletedTask;
  }
}
