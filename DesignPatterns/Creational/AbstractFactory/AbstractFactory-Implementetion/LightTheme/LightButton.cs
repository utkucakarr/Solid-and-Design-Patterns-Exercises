using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.LightTheme
{
    public class LightButton : IButton
    {
        public string Theme => "Light";

        public string Render()
            => "[Light Button] Beyaz arka plan | Gri kenarlık | Siyah yazı";

        public string Click()
            => "[Light Button] Tıklandı — açık tema efekti";
    }
}
