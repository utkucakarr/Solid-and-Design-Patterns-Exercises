using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.LightTheme
{
    // Light tema ailesi — hepsi birbiriyle uyumlu!
    public class LightUIFactory : IUIFactory
    {
        public string ThemeName => "Light";

        public IButton CreateButton() => new LightButton();

        public ICheckBox CreateCheckBox() => new LightCheckBox();

        public ITextBox CreateTextBox() => new LightTextBox();
    }
}