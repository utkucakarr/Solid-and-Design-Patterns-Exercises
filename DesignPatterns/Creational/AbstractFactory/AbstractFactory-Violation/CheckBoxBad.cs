namespace AbstractFactory_Violation
{
    public class CheckBoxBad
    {
        public string Theme { get; }
        public CheckBoxBad(string theme)
        {
            Theme = theme;
        }

        public string Render()
            => $"[CheckBox] Theme: {Theme} | Renk: {(Theme == "light" ? "Mavi" : "Mor")}";
    }
}
