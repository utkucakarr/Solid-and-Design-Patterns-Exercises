using Command_Implementation.Interfaces;
using Command_Implementation.Models;

namespace Command_Implementation.Invoker
{
    public class SmartHomeController
    {
        // Invoker yalnızca ICommand bilir — cihazları tanımıyor, loosely coupled
        private readonly Stack<ICommand> _history = new();
        private readonly Stack<ICommand> _redoStack = new();
        private readonly Queue<ICommand> _queue = new();
        private readonly List<string> _log = new();

        public CommandResult Execute(ICommand command)
        {
            ArgumentNullException.ThrowIfNull(command, nameof(command));

            var result = command.Execute();

            if (result.IsSuccess)
            {
                // Komut geçmişe ekleniyor - undo mümkün
                _history.Push(command);
                _redoStack.Clear();
                _log.Add($"[EXECUTE] {command.Description}");
            }

            return result;
        }

        public CommandResult Undo()
        {
            if (_history.Count == 0)
                return CommandResult.Fail("Geri alınacak komut bulunamadı.");

            var command = _history.Pop();
            var result = command.Undo();

            if (result.IsSuccess)
            {
                // Geri alınan komut redo stack'e taşınıyor
                _redoStack.Push(command);
                _log.Add($"[UNDO] {command.Description}");
            }

            return result;
        }

        public CommandResult Redo()
        {
            if (_redoStack.Count == 0)
                return CommandResult.Fail("Yeniden yapılacak komut bulunamadı.");

            var command = _redoStack.Pop();
            var result = command.Execute();

            if (result.IsSuccess)
            {
                _history.Push(command);
                _log.Add($"[REDO] {command.Description}");
            }

            return result;
        }

        // Kuyruk desteği — zamanlayıcı ile entegre edilebilir
        public void Enqueue(ICommand command)
        {
            ArgumentNullException.ThrowIfNull(command, nameof(command));
            _queue.Enqueue(command);
        }

        public IReadOnlyList<CommandResult> RunQueue()
        {
            var results = new List<CommandResult>();
            while (_queue.Count > 0)
                results.Add(Execute(_queue.Dequeue()));
            return results.AsReadOnly();
        }

        public IReadOnlyList<string> GetLog() => _log.AsReadOnly();
        public int HistoryCount => _history.Count;
        public int RedoCount => _redoStack.Count;
    }
}
