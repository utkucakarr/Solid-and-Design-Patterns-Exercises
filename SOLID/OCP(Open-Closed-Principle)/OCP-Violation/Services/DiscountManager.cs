using OCP_Violation.Enums;

namespace OCP_Violation.Services
{
    public class DiscountManager
    {
        // DİKKAT: Yeni bir müşteri tipi (örn: Student) geldiğinde bu metodun içine girip 
        // yeni bir 'case' eklemek zorundayız. Bu OCP'ye aykırıdır!
        public decimal CalculateDiscount(decimal amount, CustomerType customerType)
        {
            switch (customerType)
            {
                case CustomerType.Standard:
                    return amount * 0.05m;
                case CustomerType.Premium:
                    return amount * 0.10m;
                case CustomerType.VIP:
                    return amount * 0.20m;
                default:
                    throw new ArgumentException("Invalid customer type");
            }
        }
    }
}
