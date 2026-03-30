using Singleton_Implementation.Interfaces;

namespace Singleton_Implementation.Services
{
    public class UserService
    {
        private readonly IAppLogger _logger;
        public UserService(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void RegisterUser(string userName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userName, nameof(userName));

            _logger.Info($"[UserService] Kullanıcı kaydedildi -> Kullanıcı Adı: {userName}");
        }

        public void LoginFailed(string userName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userName, nameof(userName));

            _logger.Error($"[UserService] Başarısız giriş denemesi -> {userName}");
        }
    }
}
