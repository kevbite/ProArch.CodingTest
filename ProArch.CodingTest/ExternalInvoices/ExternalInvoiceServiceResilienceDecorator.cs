using System;
using System.Linq;
using System.Runtime.Serialization;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using ProArch.CodingTest.External;
using ProArch.CodingTest.Invoices;

namespace ProArch.CodingTest.ExternalInvoices
{
    public class ExternalInvoiceServiceResilienceDecorator : IExternalInvoiceServiceWrapper
    {
        private readonly IExternalInvoiceServiceWrapper _inner;
        private IFailoverInvoiceService _failoverInvoiceService;

        private readonly RetryPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .Retry(3);
        
        private readonly CircuitBreakerPolicy CircuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(1, TimeSpan.FromMinutes(1));


        public ExternalInvoiceServiceResilienceDecorator(IExternalInvoiceServiceWrapper inner, IFailoverInvoiceService failoverInvoiceService) => (_inner, _failoverInvoiceService) = (inner, failoverInvoiceService);

        public ExternalInvoice[] GetInvoices(string supplierId)
        {
            var executeAndCapture = CircuitBreakerPolicy.ExecuteAndCapture(() => RetryPolicy.Execute(() => _inner.GetInvoices(supplierId)));

            if (executeAndCapture.Outcome == OutcomeType.Failure)
            {
                var invoiceCollection = _failoverInvoiceService.GetInvoices(int.Parse(supplierId));
                if (invoiceCollection.Timestamp <= DateTime.Today.AddDays(-28))
                {
                    throw new FailoverInvoicesOutOfDateException(invoiceCollection.Timestamp,
                        executeAndCapture.FinalException);
                }
                
                return invoiceCollection
                    .Invoices.Select(x => new ExternalInvoice
                    {
                        Year = x.Year,
                        TotalAmount = x.TotalAmount
                    })
                    .ToArray();
            }

            return executeAndCapture.Result;
        }
    }
}