using MarcasAutos.Application.Common;
using MarcasAutos.Application.MarcasAutos.Events;
using MarcasAutos.Domain.MarcasAutos;

namespace MarcasAutos.Application.MarcasAutos;

public class CreateMarcaAutoUseCase
{
  private readonly IMarcaAutoRepository _repository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMessageBus _messageBus;

  public CreateMarcaAutoUseCase(
      IMarcaAutoRepository repository,
      IUnitOfWork unitOfWork,
      IMessageBus messageBus
  )
  {
    _repository = repository;
    _unitOfWork = unitOfWork;
    _messageBus = messageBus;
  }

  public async Task<MarcaAutoResponse> ExecuteAsync(
      CreateMarcaAutoRequest request,
      CancellationToken cancellationToken = default
  )
  {
    var marcaAuto = new MarcaAuto(request.Nombre);

    await _repository.AddAsync(marcaAuto, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    await _messageBus.PublishAsync(
        new MarcaAutoCreadaEvent(marcaAuto.Id, marcaAuto.Nombre),
        cancellationToken
    );

    return new MarcaAutoResponse(marcaAuto.Id, marcaAuto.Nombre);
  }
}
