using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.Application
{
    // Client — hangi tema kullanıldığından habersiz!
    public class UIApplication
    {
        private readonly IUIFactory _factory;
        private readonly IButton _button;
        private readonly ICheckBox _checkBox;
        private readonly ITextBox _textBox;

        public UIApplication(IUIFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _button = factory.CreateButton();
            _checkBox = factory.CreateCheckBox();
            _textBox = factory.CreateTextBox();
        }

        // Tüm bileşenler aynı temadan — tutarlı UI garantisi!
        public void RenderUI()
        {
            Console.WriteLine($"\n─── {_factory.ThemeName} Tema UI Render Ediliyor ───\n");
            Console.WriteLine(_button.Render());
            Console.WriteLine(_textBox.Render());
            Console.WriteLine(_checkBox.Render());
        }

        public void SimulateInteraction()
        {
            Console.WriteLine($"\n─── {_factory.ThemeName} Tema Etkileşim Simülasyonu ───\n");
            Console.WriteLine(_button.Click());
            Console.WriteLine(_textBox.GetInput("Utku Çakar"));
            Console.WriteLine(_checkBox.Toggle(true));
        }

        public string GetThemeName() => _factory.ThemeName;
    }
}