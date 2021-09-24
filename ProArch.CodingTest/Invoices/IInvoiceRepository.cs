using System.Linq;

namespace ProArch.CodingTest.Invoices
{
    public interface IInvoiceRepository
    {
        IQueryable<Invoice> Get();
    }
}