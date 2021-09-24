namespace ProArch.CodingTest.Suppliers
{
    public interface ISupplierService
    {
        Supplier GetById(int id);
    }

    public class SupplierService : ISupplierService
    {
        public Supplier GetById(int id)
        {
            return new Supplier();
        }
    }
}
