using FluentAssertions;
using LSP_Implementation.Interfaces;
using LSP_Implementation.Employees;

namespace LSP_Tests
{
    public class ContractorTests
    {
        private readonly Contractor _contractor = new("Ayşe", 20000);

        // ─── IEmployee ──────────────────────────────────────

        [Fact]
        public void CalculateSalary_ShouldReturnBaseSalary()
        {
            _contractor.CalculateSalary().Should().Be(20000);
        }

        // ─── IBonusEligible ─────────────────────────────────

        [Fact]
        public void CalculateBonus_ShouldReturnTenPercent()
        {
            _contractor.CalculateBonus().Should().Be(2000); // 20000 * 0.10
        }

        // ─── LSP Garantisi ──────────────────────────────────

        [Fact]
        public void Contractor_ShouldBeAssignableTo_IEmployee()
        {
            _contractor.Should().BeAssignableTo<IEmployee>();
        }

        [Fact]
        public void Contractor_ShouldBeAssignableTo_IBonusEligible()
        {
            _contractor.Should().BeAssignableTo<IBonusEligible>();
        }

        [Fact]
        public void Contractor_ShouldNotBeAssignableTo_IOvertimeEligible()
        {
            // Sözleşmeli fazla mesai alamaz — tip sistemi bunu garanti eder
            _contractor.Should().NotBeAssignableTo<IOvertimeEligible>();
        }

        [Fact]
        public void Contractor_AsIEmployee_ShouldNotThrow()
        {
            IEmployee employee = new Contractor("Test", 15000);

            var act = () => employee.CalculateSalary();

            act.Should().NotThrow();
        }

        // ─── Guard Clause ────────────────────────────────────

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            var act = () => new Contractor(" ", 20000);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNegativeSalary_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new Contractor("Ayşe", -1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}