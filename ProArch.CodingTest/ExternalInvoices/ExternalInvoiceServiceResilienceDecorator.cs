using System;
using System.Linq;
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
        private readonly IFailoverInvoiceService _failoverInvoiceService;

        private readonly RetryPolicy _retryPolicy = Policy
            .Handle<Exception>()
            .Retry(3);

        private readonly CircuitBreakerPolicy _circuitBreakerPolicy;
        
        public ExternalInvoiceServiceResilienceDecorator(IExternalInvoiceServiceWrapper inner, IFailoverInvoiceService failoverInvoiceService, ExternalInvoiceServiceResilienceOptions options)
        {
            (_inner, _failoverInvoiceService) = (inner, failoverInvoiceService);
            
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(1, options.CircuitBreakDuration);
        }

        public ExternalInvoice[] GetInvoices(string supplierId)
        {
            var executeAndCapture = _circuitBreakerPolicy.ExecuteAndCapture(() => _retryPolicy.Execute(() => _inner.GetInvoices(supplierId)));

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