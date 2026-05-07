using BaresTucuman.API.Application.Helpers;
using BaresTucuman.API.Domain.Enums;
using Xunit;

namespace BaresTucuman.Tests
{
    public class BarHelperTests
    {
        [Theory]
        [InlineData("Irlanda Bar", "Bar Irlanda Tucumán", true)]
        [InlineData("Peñón del Águila", "Peñón del Águila Barrio Sur", true)]
        [InlineData("La Pizzada", "El Molino", false)]
        public void NombresSonSimilares_DeberiaEvaluarDuplicadosCorrectamente(string nombre1, string nombre2, bool esperado)
        {
            // Act
            var resultado = BarHelper.NombresSonSimilares(nombre1, nombre2);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("Cervecería Artesanal", TipoBar.Cervecerias)]
        [InlineData("Pub & Boliche", TipoBar.Pubs)]
        [InlineData("Cafetería Centro", TipoBar.Cafeterias)]
        [InlineData("Pizzería", TipoBar.BaresClasicos)] // Fallback por defecto
        public void MapearCategoria_DeberiaAsignarEnumCorrecto(string categoriaCruda, TipoBar enumEsperado)
        {
            // Act
            var resultado = BarHelper.MapearCategoria(categoriaCruda);

            // Assert
            Assert.Equal(enumEsperado, resultado);
        }
    }
}