using ProArch.CodingTest.External;

namespace ProArch.CodingTest.ExternalInvoices
{
    public interface IExternalInvoiceServiceWrapper
    {
        ExternalInvoice[] GetInvoices(string supplierId);
    }

    public class ExternalInvoiceServiceWrapper : IExternalInvoiceServiceWrapper
    {
        public ExternalInvoice[] GetInvoices(string supplierId) 
            => ExternalInvoiceService.GetInvoices(supplierId);
    }
}