using FluentAssertions;
using Flyweight_Implementation.Factory;

namespace Flyweight_Tests
{
    public class TreeFactoryTests
    {
        private readonly TreeFactory _sut = new();

        // --- Flyweight Garantisi ---

        [Fact]
        public void GetTreeType_ShouldReturnSameInstance_ForSameParameters()
        {
            // Flyweight garantisi — aynı parametreler = aynı instance
            var type1 = _sut.GetTreeType("Oak", "Yeşil", "oak_texture");
            var type2 = _sut.GetTreeType("Oak", "Yeşil", "oak_texture");

            type1.Should().BeSameAs(type2);
        }

        [Fact]
        public void GetTreeType_ShouldReturnDifferentInstance_ForDifferentParameters()
        {
            var oakType = _sut.GetTreeType("Oak", "Yeşil", "oak_texture");
            var pineType = _sut.GetTreeType("Pine", "Mavi", "pine_texture");

            oakType.Should().NotBeSameAs(pineType);
        }

        [Fact]
        public void GetTreeType_ShouldNotIncreaseCount_ForSameParameters()
        {
            // Aynı parametreler defalarca çağrılsa da cache sayısı artmıyor
            _sut.GetTreeType("Oak", "Yeşil", "oak_texture");
            _sut.GetTreeType("Oak", "Yeşil", "oak_texture");
            _sut.GetTreeType("Oak", "Yeşil", "oak_texture");

            _sut.UniqueTreeTypeCount.Should().Be(1);
        }

        [Fact]
        public void GetTreeType_ShouldIncreaseCount_ForDifferentParameters()
        {
            _sut.GetTreeType("Oak", "Yeşil", "oak_texture");
            _sut.GetTreeType("Pine", "Mavi", "pine_texture");
            _sut.GetTreeType("Birch", "Sarı", "birch_texture");

            _sut.UniqueTreeTypeCount.Should().Be(3);
        }

        // --- Guard Clause Testleri ---

        [Fact]
        public void GetTreeType_WithEmptyName_ShouldThrowArgumentException()
        {
            var act = () => _sut.GetTreeType(" ", "Yeşil", "texture");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetTreeType_WithEmptyColor_ShouldThrowArgumentException()
        {
            var act = () => _sut.GetTreeType("Oak", " ", "texture");

            act.Should().Throw<ArgumentException>();
        }

        // --- Cache İstatistikleri ---

        [Fact]
        public void UniqueTreeTypeCount_ShouldStartAtZero()
        {
            _sut.UniqueTreeTypeCount.Should().Be(0);
        }

        [Fact]
        public void GetCachedTypeNames_ShouldContainRegisteredTypes()
        {
            _sut.GetTreeType("Oak", "Yeşil", "oak_texture");
            _sut.GetTreeType("Pine", "Mavi", "pine_texture");

            var names = _sut.GetCachedTypeNames();

            names.Should().HaveCount(2);
        }
    }
}
