using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.DarkTheme
{
    public class DarkTextBox : ITextBox
    {
        public string Theme => "Dark";

        public string GetInput(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));
            return $"[Dark TextBox] Girilen değer: '{value}'";
        }

        public string Render()
            => "[Dark TextBox] Koyu gri arka plan | Mor kenarlık";
    }
}
