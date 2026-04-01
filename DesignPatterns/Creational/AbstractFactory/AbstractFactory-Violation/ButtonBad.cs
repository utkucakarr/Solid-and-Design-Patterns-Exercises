namespace AbstractFactory_Violation
{
    //   Abstract Factory ihlali — ürün aileleri karışık,
    //   tutarsız UI oluşturulabilir
    public class ButtonBad
    {
        public string Theme { get; }
        public ButtonBad(string theme)
        {
            Theme = theme;
        }

        public string Render()
            => $"[Button] Theme: {Theme} | Renk: {(Theme == "light" ? "Beyaz" : "Siyah")}";
    }
}
