using Command_Implementation.Interfaces;
using Command_Implementation.Models;

namespace Command_Implementation.Commands
{
    public class MacroCommand : ICommand
    {
        private readonly IReadOnlyList<ICommand> _commands;

        public string Description { get; }

        public MacroCommand(string description, IEnumerable<ICommand> commands)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));
            Description = description;
            _commands = commands.ToList().AsReadOnly();
        }

        // Alt komutlar sırayla çalıştırılıyor
        public CommandResult Execute()
        {
            var results = new List<string>();

            foreach (var command in _commands)
            {
                var result = command.Execute();
                results.Add(result.Message);
            }

            return CommandResult.Success(string.Join(" | ", results), "Makro", Description);
        }

        // Undo işlemi ters sırada yapılıyor — bileşik komutlarda kritik
        public CommandResult Undo()
        {
            var results = new List<string>();

            foreach (var command in _commands.Reverse())
            {
                var result = command.Undo();
                results.Add(result.Message);
            }

            return CommandResult.Success(string.Join(" | ", results), "Makro", $"Undo:{Description}");
        }
    }
}
