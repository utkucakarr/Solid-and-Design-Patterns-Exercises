namespace Singleton_Violation.Logging
{
    public class UserService
    {
        // Başka bir Logger instance'ı — aynı dosyaya iki ayrı nesne yazıyor!
        private readonly Logger _logger = new("app.log");

        public void RegisterUser(string userName)
            => _logger.Log($"[UserService] Kullanıcı kaydedildi: {userName}");
    }
}
