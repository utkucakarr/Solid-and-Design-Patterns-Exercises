using AbstractFactory_Implementetion.Interfaces;

namespace AbstractFactory_Implementetion.DarkTheme
{
    public class DarkCheckBox : ICheckBox
    {
        public string Theme => "Dark";

        public string Render()
            => "[Dark CheckBox] Koyu arka plan | Mor işaret rengi";

        public string Toggle(bool isChecked)
             => $"[Dark CheckBox] {(isChecked ? "✓ İşaretlendi" : "☐ İşaret kaldırıldı")}";
    }
}
