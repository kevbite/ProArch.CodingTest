namespace ProArch.CodingTest.Invoices
{
    public interface IFailoverInvoiceService
    {
        FailoverInvoiceCollection GetInvoices(int supplierId);
    }
}