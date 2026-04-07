using Adapter_Implementation.Models;
using System.Globalization;

namespace Adapter_Implementation.Interfaces
{
    // Sistemin kendi interface'i - tüm ödeme işlemleri bu sözleşmeyle
    public interface IPaymentProcessor
    {
        string ProviderName { get; }
        PaymentResult ProcessPayment(decimal amount, string currency);
        PaymentResult Refund(string transactionId, decimal amount);
    }
}
