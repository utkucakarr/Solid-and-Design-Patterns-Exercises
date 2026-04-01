namespace AbstractFactory_Implementetion.Interfaces
{
    // Abstract Factory — birbiriyle uyumlu UI bileşenleri ailesi üretiyor
    public interface IUIFactory
    {
        string ThemeName { get; }
        IButton CreateButton();
        ITextBox CreateTextBox();
        ICheckBox CreateCheckBox();
    }
}
