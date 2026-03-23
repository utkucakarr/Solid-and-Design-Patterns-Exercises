using LSP_Implementation.Interfaces;
using LSP_Implementation.Models;
using LSP_Implementation.Employees;
using LSP_Violation;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║      LSP İHLALİ — CANLI DEMO         ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badEmployees = new List<Employee>
{
    new LSP_Violation.FullTimeEmployee("Ahmet", 30000),
    new LSP_Violation.Contractor("Ayşe", 20000),
    new LSP_Violation.Intern("Mehmet", 10000)
};

Console.WriteLine("─── Maaşlar Hesaplanıyor ───");
foreach (var emp in badEmployees)
    Console.WriteLine($"  {emp.Name}: {emp.CalculateSalary()} TL");

Console.WriteLine();
Console.WriteLine("─── Primler Hesaplanıyor ───");
Console.WriteLine("    Listede Intern var...\n");

try
{
    foreach (var emp in badEmployees)
        Console.WriteLine($"  {emp.Name} primi: {emp.CalculateBonus()} TL");
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"\n  RUNTIME HATASI: {ex.Message}");
    Console.WriteLine("     Ahmet primi hesaplandı.");
    Console.WriteLine("     Ayşe primi hesaplandı.");
    Console.WriteLine("     Mehmet'e gelince program ÇÖKTÜ!\n");
}

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║      LSP ÇÖZÜMÜ — DOĞRU YAKLAŞIM     ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var fullTime = new LSP_Implementation.Employees.FullTimeEmployee("Ahmet", 30000);
var contractor = new LSP_Implementation.Employees.Contractor("Ayşe", 20000);
var intern = new LSP_Implementation.Employees.Intern("Mehmet", 10000);

// Tüm çalışanlar — sadece maaş hesabı, güvenli!
var allEmployees = new List<IEmployee> { fullTime, contractor, intern };

Console.WriteLine("─── Tüm Maaşlar (Güvenli) ───");
foreach (var emp in allEmployees)
{
    var result = PayrollResult.Create(emp.Name, emp.CalculateSalary());
    Console.WriteLine($"   {result.EmployeeName}: {result.TotalPayment} TL");
}

Console.WriteLine();

// Sadece prim alabilenler — Intern giremez, compiler engeller!
var bonusEligibles = new List<IBonusEligible> { fullTime, contractor };

Console.WriteLine("─── Primler (Güvenli) ───");
Console.WriteLine("   Intern listeye giremez — compiler engeller!\n");

foreach (var emp in bonusEligibles)
{
    var employee = (IEmployee)emp;
    var result = PayrollResult.Create(employee.Name, employee.CalculateSalary(), emp.CalculateBonus());
    Console.WriteLine($"   {result.EmployeeName} -> Maaş: {result.Salary} TL | " +
                      $"Prim: {result.Bonus} TL | Toplam: {result.TotalPayment} TL");
}

Console.WriteLine();

// Sadece fazla mesai alabilenler
var overtimeEligibles = new List<IOvertimeEligible> { fullTime };

Console.WriteLine("─── Fazla Mesai (Güvenli) ───");
Console.WriteLine("   Sadece tam zamanlı çalışanlar bu listeye girebilir!\n");

foreach (var emp in overtimeEligibles)
{
    var employee = (IEmployee)emp;
    var result = PayrollResult.Create(employee.Name, employee.CalculateSalary(), overtime: emp.CalculateOvertime());
    Console.WriteLine($"   {result.EmployeeName} -> Maaş: {result.Salary} TL | " +
                      $"Fazla Mesai: {result.Overtime} TL | Toplam: {result.TotalPayment} TL");
}

Console.WriteLine("\n─── SONUÇ ───");
Console.WriteLine("   Bad  -> Runtime'da patlıyor. Intern prim listesine girebiliyor.");
Console.WriteLine("   Good -> Compiler koruyor. Her çalışan yalnızca hakkı olan listeye giriyor.\n");