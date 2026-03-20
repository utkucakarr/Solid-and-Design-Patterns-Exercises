using FluentAssertions;
using LSP_Implementation.Interfaces;
using LSP_Implementation.Employees;

namespace LSP_Tests
{
    public class InternTests
    {
        private readonly Intern _intern = new("Mehmet", 10000);

        // ─── IEmployee ──────────────────────────────────────

        [Fact]
        public void CalculateSalary_ShouldReturnBaseSalary()
        {
            _intern.CalculateSalary().Should().Be(10000);
        }

        // ─── LSP Garantisi ──────────────────────────────────

        [Fact]
        public void Intern_ShouldBeAssignableTo_IEmployee()
        {
            _intern.Should().BeAssignableTo<IEmployee>();
        }

        [Fact]
        public void Intern_ShouldNotBeAssignableTo_IBonusEligible()
        {
            // Stajyer prim alamaz — tip sistemi bunu garanti eder
            _intern.Should().NotBeAssignableTo<IBonusEligible>();
        }

        [Fact]
        public void Intern_ShouldNotBeAssignableTo_IOvertimeEligible()
        {
            // Stajyer fazla mesai alamaz — tip sistemi bunu garanti eder
            _intern.Should().NotBeAssignableTo<IOvertimeEligible>();
        }

        [Fact]
        public void Intern_AsIEmployee_ShouldNotThrow()
        {
            // LSP: IEmployee yerine geçtiğinde exception yok!
            IEmployee employee = new Intern("Test", 5000);

            var act = () => employee.CalculateSalary();

            act.Should().NotThrow();
        }

        // ─── Guard Clause ────────────────────────────────────

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            var act = () => new Intern(" ", 10000);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNegativeSalary_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new Intern("Mehmet", -1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}