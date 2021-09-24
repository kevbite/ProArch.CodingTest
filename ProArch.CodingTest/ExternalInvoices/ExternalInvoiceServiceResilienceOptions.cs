using System;

namespace ProArch.CodingTest.ExternalInvoices
{
    public class ExternalInvoiceServiceResilienceOptions
    {
        public ExternalInvoiceServiceResilienceOptions(TimeSpan circuitBreakDuration)
        {
            CircuitBreakDuration = circuitBreakDuration;
        }

        public TimeSpan CircuitBreakDuration { get; }

        public static ExternalInvoiceServiceResilienceOptions Default { get; } =
            new ExternalInvoiceServiceResilienceOptions(TimeSpan.FromMinutes(1));
    }
}