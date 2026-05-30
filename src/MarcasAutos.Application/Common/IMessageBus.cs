namespace MarcasAutos.Application.Common;

public interface IMessageBus
{
  Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
      where T : class;
}
