using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.DarkTheme
{
    public class DarkButton : IButton
    {
        public string Theme => "Dark";

        public string Render()
            => "[Dark Button] Siyah arka plan | Mor kenarlık | Beyaz yazı";

        public string Click()
            => "[Dark Button] Tıklandı — koyu tema efekti";
    }
}
