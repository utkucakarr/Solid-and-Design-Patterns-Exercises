namespace Iterator_Implementation.Models
{
    public sealed class PlaylistResult
    {
        public bool IsSuccess { get; private init; }
        public string Message { get; private init; } = string.Empty;
        public IReadOnlyList<string> PlayedSongs { get; private init; } = Array.Empty<string>();
        public int TotalDurationSeconds { get; private init; }
        public string TracingCode { get; private init; } = string.Empty;

        private PlaylistResult(){ }

        public static PlaylistResult Success(
            IReadOnlyList<string> playedSongs,
            int totalDurationSeconds,
            string message = "Playlist başarıyla çalındı.",
            string tracingCode = null)
            => new() 
            {        
                IsSuccess = true,
                Message = message,
                PlayedSongs = playedSongs,
                TotalDurationSeconds = totalDurationSeconds,
                TracingCode = tracingCode ?? Guid.NewGuid().ToString("N")[..8].ToUpper()
            };

        public static PlaylistResult Fail(string reason) =>
            new()
            {
                IsSuccess = false,
                Message = reason,
            };
    }
}
