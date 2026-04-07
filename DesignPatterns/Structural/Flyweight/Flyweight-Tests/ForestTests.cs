using FluentAssertions;
using Flyweight_Implementation.Forests;
using Flyweight_Implementation.Models;

namespace Flyweight_Tests
{
    public class ForestTests
    {

        // Constructure Teslteri ---

        private readonly Forest _sut = new();

        // --- PlantTree Testleri ---

        [Fact]
        public void PlantTree_ShouldIncreaseTreeCount()
        {
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 10, 20, 5);

            _sut.TreeCount.Should().Be(1);
        }

        [Fact]
        public void PlantTree_MultipleTimes_ShouldIncreaseTreeCount()
        {
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 10, 20, 5);
            _sut.PlantTree("Pine", "Mavi", "pine_texture", 30, 40, 7);
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 50, 60, 6);

            _sut.TreeCount.Should().Be(3);
        }

        // --- Flyweight Garantisi ---

        [Fact]
        public void PlantTree_SameType_ShouldNotIncreaseUniqueTypeCount()
        {
            // 3 aynı tipte ağaç = 1 TreeType nesnesi
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 10, 20, 5);
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 30, 40, 7);
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 50, 60, 6);

            _sut.UniqueTreeTypes.Should().Be(1);
        }

        [Fact]
        public void PlantTree_DifferentTypes_ShouldIncreaseUniqueTypeCount()
        {
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 10, 20, 5);
            _sut.PlantTree("Pine", "Mavi", "pine_texture", 30, 40, 7);
            _sut.PlantTree("Birch", "Sarı", "birch_texture", 50, 60, 6);

            _sut.UniqueTreeTypes.Should().Be(3);
        }

        [Fact]
        public void PlantTree_1000SameType_ShouldHaveOnly1UniqueType()
        {
            // Flyweight'in asıl gücü — 1000 ağaç = 1 TreeType!
            for (int i = 0; i < 1000; i++)
                _sut.PlantTree("Oak", "Yeşil", "oak_texture", i * 10, i * 20, 5);

            _sut.TreeCount.Should().Be(1000);
            _sut.UniqueTreeTypes.Should().Be(1);
        }

        [Fact]
        public void PlantTree_1000MixedTypes_ShouldHaveOnly3UniqueTypes()
        {
            for (int i = 0; i < 1000; i++)
            {
                _sut.PlantTree("Oak", "Yeşil", "oak_texture", i * 10, i, 5);
                _sut.PlantTree("Pine", "Mavi", "pine_texture", i * 10, i, 7);
                _sut.PlantTree("Birch", "Sarı", "birch_texture", i * 10, i, 6);
            }

            _sut.TreeCount.Should().Be(3000);
            _sut.UniqueTreeTypes.Should().Be(3);
        }

        // --- Guard Clause Testleri ---

        [Fact]
        public void PlantTree_WithNegativeX_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.PlantTree("Oak", "Yeşil", "oak_texture", -1, 20, 5);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void PlantTree_WithZeroSize_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.PlantTree("Oak", "Yeşil", "oak_texture", 10, 20, 0);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // --- Render Testleri ---

        [Fact]
        public void Render_ShouldNotThrow()
        {
            _sut.PlantTree("Oak", "Yeşil", "oak_texture", 10, 20, 5);
            _sut.PlantTree("Pine", "Mavi", "pine_texture", 30, 40, 7);

            var act = () => _sut.Render();

            act.Should().NotThrow();
        }
    }
}