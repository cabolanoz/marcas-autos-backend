using MarcasAutos.Domain.MarcasAutos;

namespace MarcasAutos.Application.MarcasAutos;

public class GetMarcasAutosUseCase
{
  private readonly IMarcaAutoRepository _repository;

  public GetMarcasAutosUseCase(IMarcaAutoRepository repository)
  {
    _repository = repository;
  }

  public async Task<IReadOnlyList<MarcaAutoResponse>> ExecuteAsync(
      CancellationToken cancellationToken = default
  )
  {
    var marcas = await _repository.GetAllAsync(cancellationToken);

    return marcas
        .Select(marca => new MarcaAutoResponse(marca.Id, marca.Nombre))
        .ToList();
  }
}
