namespace AbstractFactory_Implementetion.Interfaces
{
    public interface IButton
    {
        public string Theme { get; }
        string Render();
        string Click();
    }
}
