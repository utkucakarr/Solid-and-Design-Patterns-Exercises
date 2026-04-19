using Command_Implementation.Models;

namespace Command_Implementation.Interfaces
{
    public interface ICommand
    {
        string Description { get; }

        CommandResult Execute();
        CommandResult Undo();
    }
}
