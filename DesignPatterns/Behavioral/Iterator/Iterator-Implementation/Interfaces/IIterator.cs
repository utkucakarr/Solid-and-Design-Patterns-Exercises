namespace Iterator_Implementation.Interfaces
{
    // GoF Iterator arayüzü — traversal sözleşmesi
    public interface IIterator<T>
    {
        bool HasNext();  // Sonraki eleman var mı?
        T Next();        // Sonraki elemanı döndür, imleci ilerlet
        void Reset();    // Başa dön
    }
}
