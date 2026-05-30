using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarcasAutos.Api.Data;

namespace MarcasAutos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasAutosController : ControllerBase
{
  private readonly AppDbContext _context;

  public MarcasAutosController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var brands = await _context.MarcasAutos
      .OrderBy(m => m.Id)
      .Select(m => new MarcaAutoResponse(m.Id, m.Nombre))
      .ToListAsync();

    return Ok(brands);
  }
}

public record MarcaAutoResponse(int Id, string Nombre);