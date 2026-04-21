using Memento_Implementation.Models;

namespace Memento_Implementation.Interfaces
{
    public interface IDocumentEditor
    {
        // Editör — kullanıcı aksiyonlarını orkestre eder
        DocumentResult SetTitle(string title);
        DocumentResult SetContent(string content);
        DocumentResult AddTag(string tag);
        DocumentResult RemoveTag(string tag);
        DocumentResult Undo();
        DocumentResult Redo();
        DocumentResult GetCurrentState();
    }
}
