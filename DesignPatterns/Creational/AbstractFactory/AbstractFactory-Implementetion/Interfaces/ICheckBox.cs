namespace AbstractFactory_Implementetion.Interfaces
{
    public interface ICheckBox
    {
        public string Theme { get; }
        string Render();
        string Toggle(bool isChecked);
    }
}
