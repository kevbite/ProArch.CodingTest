using System;
using Polly;
using Polly.Retry;
using ProArch.CodingTest.External;

namespace ProArch.CodingTest.ExternalInvoices
{
    public class ExternalInvoiceServiceResilienceDecorator : IExternalInvoiceServiceWrapper
    {
        private readonly IExternalInvoiceServiceWrapper _inner;

        private static readonly RetryPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .Retry(3);

        public ExternalInvoiceServiceResilienceDecorator(IExternalInvoiceServiceWrapper inner) => _inner = inner;

        public ExternalInvoice[] GetInvoices(string supplierId)
        {
            return RetryPolicy.Execute(() => _inner.GetInvoices(supplierId));
        }
    }
    
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