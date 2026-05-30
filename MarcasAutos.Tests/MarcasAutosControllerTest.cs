using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarcasAutos.Api.Controllers;
using MarcasAutos.Api.Data;
using MarcasAutos.Api.Models;

namespace MarcasAutos.Tests;

public class MarcasAutosControllerTest
{
    [Fact]
    public async Task GetAll_ReturnsExpectedBrands()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        context.MarcasAutos.AddRange(
            new MarcaAuto { Nombre = "Toyota" },
            new MarcaAuto { Nombre = "KIA" },
            new MarcaAuto { Nombre = "Nissan" }
        );

        await context.SaveChangesAsync();

        var controller = new MarcasAutosController(context);
        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var brands = Assert.IsAssignableFrom<IEnumerable<MarcaAutoResponse>>(okResult.Value);

        Assert.Equal(3, brands.Count());
        Assert.Contains(brands, brand => brand.Nombre == "Toyota");
        Assert.Contains(brands, brand => brand.Nombre == "KIA");
        Assert.Contains(brands, brand => brand.Nombre == "Nissan");
    }
}
