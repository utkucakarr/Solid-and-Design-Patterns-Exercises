using SRP_Implementation.Interfeces;
using SRP_Implementation.Models;
using System.Text;

namespace SRP_Implementation.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly IOrderLogger _logger;

        public OrderService(
            IOrderRepository repository,
            INotificationService notificationService,
            IOrderLogger logger)
        {
            _repository = repository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public void ProcessOrder(Order order)
        {
            _repository.Save(order);
            _notificationService.SendConfirmation(order);
            _logger.LogOrderCreated(order);
        }
    }
}
