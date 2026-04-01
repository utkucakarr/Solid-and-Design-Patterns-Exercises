namespace AbstractFactory_Implementetion.Interfaces
{
    public interface ITextBox
    {
        public string Theme { get; }
        string Render();
        string GetInput(string value);
    }
}
