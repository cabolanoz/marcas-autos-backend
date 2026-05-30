namespace MarcasAutos.Domain.MarcasAutos;

public interface IMarcaAutoRepository
{
  Task<IReadOnlyList<MarcaAuto>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<MarcaAuto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

  Task AddAsync(MarcaAuto marcaAuto, CancellationToken cancellationToken = default);
}
