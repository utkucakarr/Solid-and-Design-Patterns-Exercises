namespace Memento_Implementation.Models
{
    public sealed class DocumentResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string? Title { get; }
        public string? Content { get; }
        public IReadOnlyList<string>? Tags { get; }
        public bool CanUndo { get; }
        public bool CanRedo { get; }

        private DocumentResult(
            bool isSuccess,
            string message,
            string? title,
            string? content,
            IReadOnlyList<string>? tags,
            bool canUndo,
            bool canRedo)
        {
            IsSuccess = isSuccess;
            Message = message;
            Title = title;
            Content = content;
            Tags = tags;
            CanUndo = canUndo;
            CanRedo = canRedo;
        }

        public static DocumentResult Success(
            string message,
            string title,
            string content,
            IReadOnlyList<string> tags,
            bool canUndo,
            bool canRedo) =>
            new(
                isSuccess: true,
                message: message,
                title: title,
                content: content,
                tags: tags,
                canUndo: canUndo,
                canRedo: canRedo
            );

        public static DocumentResult Fail(string reason) =>
            new(
                isSuccess: false,
                message: reason,
                title: null,
                content: null,
                tags: null,
                canUndo: false,
                canRedo: false
            );
    }
}
