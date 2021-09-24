using ProArch.CodingTest.External;

namespace ProArch.CodingTest.ExternalInvoices
{
    public interface IExternalInvoiceServiceWrapper
    {
        ExternalInvoice[] GetInvoices(string supplierId);
    }
}