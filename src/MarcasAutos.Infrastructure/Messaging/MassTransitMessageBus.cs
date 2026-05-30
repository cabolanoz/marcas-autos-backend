using MarcasAutos.Application.Common;
using MassTransit;

namespace MarcasAutos.Infrastructure.Messaging;

public class MassTransitMessageBus : IMessageBus
{
  private readonly IPublishEndpoint _publishEndpoint;

  public MassTransitMessageBus(IPublishEndpoint publishEndpoint)
  {
    _publishEndpoint = publishEndpoint;
  }

  public Task PublishAsync<T>(
      T message,
      CancellationToken cancellationToken = default
  )
      where T : class
  {
    return _publishEndpoint.Publish(message, cancellationToken);
  }
}
