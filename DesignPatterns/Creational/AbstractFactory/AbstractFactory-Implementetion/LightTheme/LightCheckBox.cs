using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.LightTheme
{
    public class LightCheckBox : ICheckBox
    {
        public string Theme => "Light";

        public string Render()
            => "[Light CheckBox] Beyaz arka plan | Mavi işaret rengi";

        public string Toggle(bool isChecked)
            => $"[Light CheckBox] {(isChecked ? "✓ İşaretlendi" : "☐ İşaret kaldırıldı")}";

    }
}
