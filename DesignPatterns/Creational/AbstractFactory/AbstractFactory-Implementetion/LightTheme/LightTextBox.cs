using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.LightTheme
{
    public class LightTextBox : ITextBox
    {
        public string Theme => "Light";

        public string Render()
            => "[Light TextBox] Beyaz arka plan | Açık gri kenarlık";

        public string GetInput(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            return $"[Light TextBox] Girilen değer: '{value}'";
        }
    }
}
