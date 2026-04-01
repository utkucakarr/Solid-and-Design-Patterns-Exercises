namespace AbstractFactory_Violation
{
    public class TextBoxBad
    {
        public string Theme { get; }
        public TextBoxBad(string theme)
        {
            Theme = theme;
        }

        public string Render()
             => $"[TextBox] Theme: {Theme} | Renk: {(Theme == "light" ? "Açık gri" : "Koyu gri")}";
    }
}
