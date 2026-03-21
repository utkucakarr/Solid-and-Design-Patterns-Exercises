namespace ISP_Violation
{
    public interface IPrinter
    {
        void Print(string document);
        void Scan(string document);
        void Fax(string document);
    }
}
