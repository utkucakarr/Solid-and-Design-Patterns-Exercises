using AbstractFactory_Implementetion.Interfaces;
using AbstractFactory_Implementetion.LightTheme;
using FluentAssertions;

namespace AbstractFactory_Tests
{
    public class LightUIFactoryTests
    {
        private readonly LightUIFactory _lightFactory = new();

        [Fact]
        public void CreateButton_ShouldReturnLightButton()
        {
            var button = _lightFactory.CreateButton();

            button.Should().BeOfType<LightButton>();
        }

        [Fact]
        public void CreateTextBox_ShouldReturnLightTextBox()
        {
            var textBox = _lightFactory.CreateTextBox();

            textBox.Should().BeOfType<LightTextBox>();
            textBox.Should().BeAssignableTo<LightTextBox>();
        }

        [Fact]
        public void CreateCheckBox_ShouldReturnLightCheckBox()
        {
            var checkBox = _lightFactory.CreateCheckBox();
            checkBox.Should().BeOfType<LightCheckBox>();
        }

        [Fact]
        public void CreateButton_ShouldReturnIButton()
        {
            var button = _lightFactory.CreateButton();

            button.Should().BeAssignableTo<IButton>();
        }

        [Fact]
        public void CreateTextBox_ShouldReturnITextBox()
        {
            var textBox = _lightFactory.CreateTextBox();

            textBox.Should().BeAssignableTo<ITextBox>();
        }

        [Fact]
        public void CreateCheckBox_ShouldReturnICheckBox()
        {
            var checkBox = _lightFactory.CreateCheckBox();

            checkBox.Should().BeAssignableTo<ICheckBox>();
        }

        // --- Tema Tutarlılığı ---

        [Fact]
        public void AllComponents_ShouldHaveSameTheme()
        {
            var button = _lightFactory.CreateButton();
            var textBox = _lightFactory.CreateTextBox();
            var checkBox = _lightFactory.CreateCheckBox();

            button.Theme.Should().Be("Light");
            textBox.Theme.Should().Be("Light");
            checkBox.Theme.Should().Be("Light");
        }

        [Fact]
        public void ThemeName_ShouldBeLight()
        {
            _lightFactory.ThemeName.Should().Be("Light");
        }

        // --- Render Testleri ---

        [Fact]
        public void Button_Render_ShouldContainLightThemeInfo()
        {
            var button = _lightFactory.CreateButton();

            button.Render().Should().Contain("Light");
        }

        [Fact]
        public void TextBox_GetInput_WithEmptyValue_ShouldThrowArgumentException()
        {
            var textBox = _lightFactory.CreateTextBox();

            var act = () => textBox.GetInput(" ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TextBox_GetInput_WithValidValue_ShouldReturnFormattedString()
        {
            var textBox = _lightFactory.CreateTextBox();

            var result = textBox.GetInput("Utku");

            result.Should().Contain("Utku");
        }

        [Fact]
        public void CheckBox_Toggle_WhenChecked_ShouldIndicateCheckedState()
        {
            var checkBox = _lightFactory.CreateCheckBox();

            var result = checkBox.Toggle(true);

            result.Should().Contain("İşaretlendi");
        }
    }
}