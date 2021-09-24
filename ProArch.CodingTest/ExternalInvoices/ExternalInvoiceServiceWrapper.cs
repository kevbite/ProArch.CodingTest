using ProArch.CodingTest.External;

namespace ProArch.CodingTest.ExternalInvoices
{
    public class ExternalInvoiceServiceWrapper : IExternalInvoiceServiceWrapper
    {
        public ExternalInvoice[] GetInvoices(string supplierId) 
            => ExternalInvoiceService.GetInvoices(supplierId);
    }
}