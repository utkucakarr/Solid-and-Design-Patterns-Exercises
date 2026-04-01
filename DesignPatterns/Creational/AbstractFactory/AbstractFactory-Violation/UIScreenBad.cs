namespace AbstractFactory_Violation
{
    //  Client her bileşeni ayrı ayrı oluşturuyor
    //   Yanlış tema kombinasyonu mümkün!
    public class UIScreenBad
    {
        public void Render(string buttonTheme, string textBoxTheme, string checkBoxTheme)
        {
            var button = new ButtonBad(buttonTheme);
            var textBox = new TextBoxBad(textBoxTheme);
            var checkBox = new CheckBoxBad(checkBoxTheme);

            Console.WriteLine("Ekran render ediliyor...");
            Console.WriteLine(button.Render());
            Console.WriteLine(textBox.Render());
            Console.WriteLine(checkBox.Render());
        }
    }
}
