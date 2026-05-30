using MarcasAutos.Application.MarcasAutos;
using Microsoft.AspNetCore.Mvc;

namespace MarcasAutos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasAutosController : ControllerBase
{
  private readonly GetMarcasAutosUseCase _getMarcasAutosUseCase;
  private readonly CreateMarcaAutoUseCase _createMarcaAutoUseCase;

  public MarcasAutosController(
      GetMarcasAutosUseCase getMarcasAutosUseCase,
      CreateMarcaAutoUseCase createMarcaAutoUseCase
  )
  {
    _getMarcasAutosUseCase = getMarcasAutosUseCase;
    _createMarcaAutoUseCase = createMarcaAutoUseCase;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
  {
    var marcas = await _getMarcasAutosUseCase.ExecuteAsync(cancellationToken);

    return Ok(marcas);
  }

  [HttpPost]
  public async Task<IActionResult> Create(
      [FromBody] CreateMarcaAutoRequest request,
      CancellationToken cancellationToken
  )
  {
    var marca = await _createMarcaAutoUseCase.ExecuteAsync(request, cancellationToken);

    return Created($"/api/MarcasAutos/{marca.Id}", marca);
  }
}
