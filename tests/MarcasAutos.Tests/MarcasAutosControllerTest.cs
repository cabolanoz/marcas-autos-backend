using MarcasAutos.Api.Controllers;
using MarcasAutos.Application.Common;
using MarcasAutos.Application.MarcasAutos;
using MarcasAutos.Application.MarcasAutos.Events;
using MarcasAutos.Domain.MarcasAutos;
using MarcasAutos.Infrastructure.Persistence;
using MarcasAutos.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Tests;

public class MarcasAutosControllerTest
{
  [Fact]
  public async Task GetAll_ReturnsExpectedBrands()
  {
    await using var context = CreateContext();

    context.MarcasAutos.AddRange(
        new MarcaAuto("Toyota"),
        new MarcaAuto("KIA"),
        new MarcaAuto("Nissan")
    );

    await context.SaveChangesAsync();

    var controller = CreateController(context);

    var result = await controller.GetAll(CancellationToken.None);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var brands = Assert.IsAssignableFrom<IEnumerable<MarcaAutoResponse>>(okResult.Value).ToList();

    Assert.Equal(3, brands.Count);
    Assert.Contains(brands, brand => brand.Nombre == "Toyota");
    Assert.Contains(brands, brand => brand.Nombre == "KIA");
    Assert.Contains(brands, brand => brand.Nombre == "Nissan");
  }

  [Fact]
  public async Task GetAll_ReturnsEmptyList_WhenNoBrandsExist()
  {
    await using var context = CreateContext();

    var controller = CreateController(context);

    var result = await controller.GetAll(CancellationToken.None);

    var okResult = Assert.IsType<OkObjectResult>(result);
    var brands = Assert.IsAssignableFrom<IEnumerable<MarcaAutoResponse>>(okResult.Value);

    Assert.Empty(brands);
  }

  [Fact]
  public async Task Create_ReturnsCreatedBrand_AndPublishesEvent()
  {
    await using var context = CreateContext();

    var messageBus = new FakeMessageBus();
    var controller = CreateController(context, messageBus);

    var result = await controller.Create(
        new CreateMarcaAutoRequest("Mazda"),
        CancellationToken.None
    );

    var createdResult = Assert.IsType<CreatedResult>(result);
    var response = Assert.IsType<MarcaAutoResponse>(createdResult.Value);

    Assert.Equal("Mazda", response.Nombre);

    var brandInDatabase = await context.MarcasAutos.SingleAsync();
    Assert.Equal("Mazda", brandInDatabase.Nombre);

    var publishedEvent = Assert.Single(messageBus.PublishedMessages);
    var marcaAutoCreadaEvent = Assert.IsType<MarcaAutoCreadaEvent>(publishedEvent);

    Assert.Equal("Mazda", marcaAutoCreadaEvent.Nombre);
  }

  [Fact]
  public async Task Repository_GetByIdAsync_ReturnsBrand_WhenBrandExists()
  {
    await using var context = CreateContext();

    var marcaAuto = new MarcaAuto("Toyota");
    context.MarcasAutos.Add(marcaAuto);
    await context.SaveChangesAsync();

    var repository = new MarcaAutoRepository(context);

    var result = await repository.GetByIdAsync(marcaAuto.Id);

    Assert.NotNull(result);
    Assert.Equal("Toyota", result.Nombre);
  }

  [Fact]
  public async Task Repository_GetByIdAsync_ReturnsNull_WhenBrandDoesNotExist()
  {
    await using var context = CreateContext();

    var repository = new MarcaAutoRepository(context);

    var result = await repository.GetByIdAsync(999);

    Assert.Null(result);
  }

  [Fact]
  public async Task UnitOfWork_SaveChangesAsync_PersistsChanges()
  {
    await using var context = CreateContext();

    context.MarcasAutos.Add(new MarcaAuto("Hyundai"));

    var unitOfWork = new UnitOfWork(context);
    var affectedRows = await unitOfWork.SaveChangesAsync();

    Assert.Equal(1, affectedRows);
    Assert.Equal("Hyundai", context.MarcasAutos.Single().Nombre);
  }

  [Fact]
  public async Task GetMarcasAutosUseCase_ReturnsMappedResponses()
  {
    await using var context = CreateContext();

    context.MarcasAutos.AddRange(
        new MarcaAuto("Toyota"),
        new MarcaAuto("Honda")
    );

    await context.SaveChangesAsync();

    var repository = new MarcaAutoRepository(context);
    var useCase = new GetMarcasAutosUseCase(repository);

    var result = await useCase.ExecuteAsync();

    Assert.Equal(2, result.Count);
    Assert.Contains(result, brand => brand.Nombre == "Toyota");
    Assert.Contains(result, brand => brand.Nombre == "Honda");
  }

  [Fact]
  public async Task CreateMarcaAutoUseCase_CreatesBrand_AndPublishesEvent()
  {
    await using var context = CreateContext();

    var repository = new MarcaAutoRepository(context);
    var unitOfWork = new UnitOfWork(context);
    var messageBus = new FakeMessageBus();

    var useCase = new CreateMarcaAutoUseCase(repository, unitOfWork, messageBus);

    var result = await useCase.ExecuteAsync(new CreateMarcaAutoRequest("Suzuki"));

    Assert.Equal("Suzuki", result.Nombre);
    Assert.Equal("Suzuki", context.MarcasAutos.Single().Nombre);

    var publishedEvent = Assert.Single(messageBus.PublishedMessages);
    var marcaAutoCreadaEvent = Assert.IsType<MarcaAutoCreadaEvent>(publishedEvent);

    Assert.Equal("Suzuki", marcaAutoCreadaEvent.Nombre);
  }

  [Fact]
  public void MarcaAuto_TrimsName()
  {
    var marcaAuto = new MarcaAuto("  Toyota  ");

    Assert.Equal("Toyota", marcaAuto.Nombre);
  }

  [Fact]
  public void MarcaAuto_Throws_WhenNameIsEmpty()
  {
    Assert.Throws<ArgumentException>(() => new MarcaAuto(""));
  }

  [Fact]
  public void MarcaAuto_Throws_WhenNameIsWhiteSpace()
  {
    Assert.Throws<ArgumentException>(() => new MarcaAuto("   "));
  }

  [Fact]
  public void MarcaAuto_UpdatesName()
  {
    var marcaAuto = new MarcaAuto("Toyota");

    marcaAuto.ActualizarNombre("Honda");

    Assert.Equal("Honda", marcaAuto.Nombre);
  }

  [Fact]
  public void MarcaAuto_TrimsUpdatedName()
  {
    var marcaAuto = new MarcaAuto("Toyota");

    marcaAuto.ActualizarNombre("  Honda  ");

    Assert.Equal("Honda", marcaAuto.Nombre);
  }

  [Fact]
  public void MarcaAuto_Throws_WhenUpdatedNameIsEmpty()
  {
    var marcaAuto = new MarcaAuto("Toyota");

    Assert.Throws<ArgumentException>(() => marcaAuto.ActualizarNombre(""));
  }

  [Fact]
  public void MarcaAuto_Throws_WhenUpdatedNameIsWhiteSpace()
  {
    var marcaAuto = new MarcaAuto("Toyota");

    Assert.Throws<ArgumentException>(() => marcaAuto.ActualizarNombre("   "));
  }

  private static AppDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    return new AppDbContext(options);
  }

  private static MarcasAutosController CreateController(
      AppDbContext context,
      FakeMessageBus? messageBus = null
  )
  {
    var repository = new MarcaAutoRepository(context);
    var unitOfWork = new UnitOfWork(context);

    var getUseCase = new GetMarcasAutosUseCase(repository);
    var createUseCase = new CreateMarcaAutoUseCase(
        repository,
        unitOfWork,
        messageBus ?? new FakeMessageBus()
    );

    return new MarcasAutosController(getUseCase, createUseCase);
  }

  private sealed class FakeMessageBus : IMessageBus
  {
    public List<object> PublishedMessages { get; } = new();

    public Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
      PublishedMessages.Add(message);

      return Task.CompletedTask;
    }
  }
}
