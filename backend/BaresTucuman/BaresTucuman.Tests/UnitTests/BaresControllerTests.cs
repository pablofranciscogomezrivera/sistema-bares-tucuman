using BaresTucuman.API.Application.DTOs;
using BaresTucuman.API.Controllers;
using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Enums;
using BaresTucuman.API.Domain.Interfaces;
using BaresTucuman.API.Infraestructure.Data;
using BaresTucuman.API.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute; 
using Xunit;

namespace BaresTucuman.Tests
{
    public class BaresControllerTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetBar_SiExiste_DeberiaDevolverOkConBar()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Bares.Add(new Bar { Id = 1, Nombre = "Test Bar", IsActive = true });
            await context.SaveChangesAsync();
            var provider = Substitute.For<IBarProvider>();
            var service = Substitute.For<IBarSyncService>();
            var controller = new BaresController(provider, context, service);

            // Act
            var resultado = await controller.GetBar(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var barDevuelto = Assert.IsType<Bar>(okResult.Value);
            Assert.Equal("Test Bar", barDevuelto.Nombre);
        }

        [Fact]
        public async Task CreateBar_SiValido_DeberiaGuardarYDevolverCreated()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var mockValidator = Substitute.For<IValidator<CrearBarDto>>();

            mockValidator
                .ValidateAsync(Arg.Any<CrearBarDto>(), default)
                .Returns(new ValidationResult());
            var provider = Substitute.For<IBarProvider>();

            var service = Substitute.For<IBarSyncService>();
            var controller = new BaresController(provider, context, service);
            var nuevoDto = new CrearBarDto
            {
                Nombre = "Nuevo Bar",
                Ubicacion = "Centro",
                Categoria = "Bar",
                CategoriaAMostrar = TipoBar.BaresClasicos
            };

            var resultado = await controller.CreateBar(nuevoDto, mockValidator);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(BaresController.GetBar), createdResult.ActionName);

            var barEnDb = await context.Bares.FirstOrDefaultAsync(b => b.Nombre == "Nuevo Bar");
            Assert.NotNull(barEnDb);
            Assert.True(barEnDb.IsActive);
        }

        [Fact]
        public async Task CreateBar_SiInvalido_DeberiaDevolverBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockValidator = Substitute.For<IValidator<CrearBarDto>>();

            var fallosSimulados = new ValidationResult(new[] { new ValidationFailure("Nombre", "Error falso de prueba") });

            mockValidator
                .ValidateAsync(Arg.Any<CrearBarDto>(), default)
                .Returns(fallosSimulados);
            var provider = Substitute.For<IBarProvider>();

            var service = Substitute.For<IBarSyncService>();
            var controller = new BaresController(provider, context, service);
            var dtoMalo = new CrearBarDto(); 

            // Act
            var resultado = await controller.CreateBar(dtoMalo, mockValidator);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado);

            var cantidadEnDb = await context.Bares.CountAsync();
            Assert.Equal(0, cantidadEnDb);
        }

        [Fact]
        public async Task UpdateBar_SiDatosSonValidosYBarExiste_DeberiaActualizarYDevolverNoContent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Bares.Add(new Bar
            {
                Id = 10,
                Nombre = "Nombre Viejo",
                Ubicacion = "Ubicacion Vieja",
                IsActive = true
            });
            await context.SaveChangesAsync();

            var mockProvider = Substitute.For<IBarProvider>();
            var mockSyncService = Substitute.For<IBarSyncService>();
            var mockValidator = Substitute.For<IValidator<ActualizarBarDto>>();

            mockValidator.ValidateAsync(Arg.Any<ActualizarBarDto>(), default)
                         .Returns(new ValidationResult());

            var controller = new BaresController(mockProvider, context, mockSyncService);

            var dto = new ActualizarBarDto
            {
                Nombre = "Nombre Nuevo",
                Ubicacion = "Ubicacion Nueva",
                CategoriaAMostrar = TipoBar.Restobares
            };

            // Act
            var resultado = await controller.UpdateBar(10, dto, mockValidator);

            // Assert
            Assert.IsType<NoContentResult>(resultado);

            var barModificado = await context.Bares.FindAsync(10);
            Assert.Equal("Nombre Nuevo", barModificado.Nombre);
            Assert.Equal("Ubicacion Nueva", barModificado.Ubicacion);
            Assert.Equal(TipoBar.Restobares, barModificado.CategoriaAMostrar);
        }

        [Fact]
        public async Task UpdateBar_SiDatosSonInvalidos_DeberiaDevolverBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockProvider = Substitute.For<IBarProvider>();
            var mockSyncService = Substitute.For<IBarSyncService>();
            var mockValidator = Substitute.For<IValidator<ActualizarBarDto>>();

            var fallos = new ValidationResult(new[] { new ValidationFailure("Nombre", "El nombre es obligatorio") });
            mockValidator.ValidateAsync(Arg.Any<ActualizarBarDto>(), default).Returns(fallos);

            var controller = new BaresController(mockProvider, context, mockSyncService);
            var dtoMalo = new ActualizarBarDto(); 

            // Act
            var resultado = await controller.UpdateBar(1, dtoMalo, mockValidator);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado);
        }

        [Fact]
        public async Task UpdateBar_SiBarNoExiste_DeberiaDevolverNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var mockProvider = Substitute.For<IBarProvider>();
            var mockSyncService = Substitute.For<IBarSyncService>();
            var mockValidator = Substitute.For<IValidator<ActualizarBarDto>>();

            mockValidator.ValidateAsync(Arg.Any<ActualizarBarDto>(), default).Returns(new ValidationResult());

            var controller = new BaresController(mockProvider, context, mockSyncService);
            var dtoValido = new ActualizarBarDto { Nombre = "Un buen nombre", Ubicacion = "Centro" };

            // Act
            var resultado = await controller.UpdateBar(99, dtoValido, mockValidator);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado);
        }
    }
}