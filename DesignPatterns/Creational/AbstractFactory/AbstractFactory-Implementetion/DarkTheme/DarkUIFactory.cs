using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.DarkTheme
{
    public class DarkUIFactory : IUIFactory
    {
        public string ThemeName => "Dark";

        public IButton CreateButton() => new DarkButton();

        public ICheckBox CreateCheckBox() => new DarkCheckBox();

        public ITextBox CreateTextBox() => new DarkTextBox();
    }
}
