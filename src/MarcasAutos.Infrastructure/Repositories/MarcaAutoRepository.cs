using MarcasAutos.Domain.MarcasAutos;
using MarcasAutos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Infrastructure.Repositories;

public class MarcaAutoRepository : IMarcaAutoRepository
{
  private readonly AppDbContext _context;

  public MarcaAutoRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<IReadOnlyList<MarcaAuto>> GetAllAsync(
      CancellationToken cancellationToken = default
  )
  {
    return await _context.MarcasAutos
        .OrderBy(marca => marca.Id)
        .ToListAsync(cancellationToken);
  }

  public async Task<MarcaAuto?> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default
  )
  {
    return await _context.MarcasAutos
        .FirstOrDefaultAsync(marca => marca.Id == id, cancellationToken);
  }

  public async Task AddAsync(
      MarcaAuto marcaAuto,
      CancellationToken cancellationToken = default
  )
  {
    await _context.MarcasAutos.AddAsync(marcaAuto, cancellationToken);
  }
}
