using AbstractFactory_Implementetion.Application;
using AbstractFactory_Implementetion.DarkTheme;
using AbstractFactory_Implementetion.Interfaces;
using AbstractFactory_Implementetion.LightTheme;
using FluentAssertions;
using Moq;

namespace AbstractFactory_Tests
{
    public class UIApplicationTests
    {
        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullFactory_ShouldThrowArgumentNullException()
        {
            var act = () => new UIApplication(null);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("factory");
        }

        // --- Light Tema Testleri ---

        [Fact]
        public void UIApplication_WithLightFactory_ShouldHaveLightTheme()
        {
            var app = new UIApplication(new LightUIFactory());

            app.GetThemeName().Should().Be("Light");
        }

        [Fact]
        public void UIApplication_WithLightFactory_RenderUI_ShouldNotThrow()
        {
            var app = new UIApplication(new LightUIFactory());

            var act = () => app.RenderUI();

            act.Should().NotThrow();
        }

        [Fact]
        public void UIApplication_WithLightFactory_SimulateInteraction_ShouldNotThrow()
        {
            var app = new UIApplication(new LightUIFactory());

            var act = () => app.SimulateInteraction();

            act.Should().NotThrow();
        }

        // --- Dark Tema Testleri ---

        [Fact]
        public void UIApplication_WithDarkFactory_ShouldHaveDarkTheme()
        {
            var app = new UIApplication(new DarkUIFactory());

            app.GetThemeName().Should().Be("Dark");
        }

        [Fact]
        public void UIApplication_WithDarkFactory_RenderUI_ShouldNotThrow()
        {
            var app = new UIApplication(new DarkUIFactory());

            var act = () => app.RenderUI();

            act.Should().NotThrow();
        }

        // --- Abstract Factory Garantisi ---

        [Fact]
        public void UIApplication_ShouldWork_WithAnyIUIFactory()
        {
            // Abstract Factory: herhangi bir IUIFactory implementasyonu çalışmalı
            var mockFactory = new Mock<IUIFactory>();
            var mockButton = new Mock<IButton>();
            var mockTextBox = new Mock<ITextBox>();
            var mockCheckBox = new Mock<ICheckBox>();

            mockFactory.Setup(f => f.ThemeName).Returns("MockTheme");
            mockFactory.Setup(f => f.CreateButton()).Returns(mockButton.Object);
            mockFactory.Setup(f => f.CreateTextBox()).Returns(mockTextBox.Object);
            mockFactory.Setup(f => f.CreateCheckBox()).Returns(mockCheckBox.Object);

            mockButton.Setup(b => b.Render()).Returns("Mock Button");
            mockTextBox.Setup(t => t.Render()).Returns("Mock TextBox");
            mockCheckBox.Setup(c => c.Render()).Returns("Mock CheckBox");
            mockButton.Setup(b => b.Click()).Returns("Mock Click");
            mockTextBox.Setup(t => t.GetInput(It.IsAny<string>())).Returns("Mock Input");
            mockCheckBox.Setup(c => c.Toggle(It.IsAny<bool>())).Returns("Mock Toggle");

            var app = new UIApplication(mockFactory.Object);

            app.GetThemeName().Should().Be("MockTheme");

            var renderAct = () => app.RenderUI();
            renderAct.Should().NotThrow();
        }

        [Fact]
        public void UIApplication_ShouldCallAllFactoryMethods_OnConstruction()
        {
            // Constructor'da tüm bileşenler oluşturuluyor
            var mockFactory = new Mock<IUIFactory>();
            var mockButton = new Mock<IButton>();
            var mockTextBox = new Mock<ITextBox>();
            var mockCheckBox = new Mock<ICheckBox>();

            mockFactory.Setup(f => f.CreateButton()).Returns(mockButton.Object);
            mockFactory.Setup(f => f.CreateTextBox()).Returns(mockTextBox.Object);
            mockFactory.Setup(f => f.CreateCheckBox()).Returns(mockCheckBox.Object);
            mockFactory.Setup(f => f.ThemeName).Returns("Test");

            _ = new UIApplication(mockFactory.Object);

            mockFactory.Verify(f => f.CreateButton(), Times.Once);
            mockFactory.Verify(f => f.CreateTextBox(), Times.Once);
            mockFactory.Verify(f => f.CreateCheckBox(), Times.Once);
        }
    }
}
