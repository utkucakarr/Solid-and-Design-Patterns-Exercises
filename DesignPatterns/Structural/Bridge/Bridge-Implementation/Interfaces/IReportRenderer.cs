namespace Bridge_Implementation.Interfaces
{
    // İmplementor - format hiyerarşisinin sözleşmesi
    // PDF, Excel, CSV bu interface'i implement ediyor.
    public interface IReportRenderer
    {
        string RenderName { get; }
        string Render(string title, string content, Dictionary<string, string> metadata);
    }
}
