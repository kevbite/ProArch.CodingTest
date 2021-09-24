using System;
using FluentAssertions;
using ProArch.CodingTest.External;
using ProArch.CodingTest.Summary;
using Xunit;

namespace ProArch.CodingTest.Tests
{
    public class SupplierServiceTestsForExternalSuppliersResilience
    {
        private readonly Harness _harness = new();

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ShouldReturnInvoicesUpTo3ConsecutiveErrors(int errorCount)
        {
            var supplier = _harness.AddExternalSupplier();
            _harness.AddExternalInvoice(supplier, 100, 2021);
            for (var i = 0; i < errorCount; i++)
            {
                _harness.AddExternalInvoiceAction(() => throw new TimeoutException("Boom!"));
            }

            var spendService = _harness.CreateSpendService();
            SpendSummary spendSummary = null;
            for (var i = 0; i < errorCount; i++)
            {
                spendSummary = spendService.GetTotalSpend(supplier.Id);
            }


            spendSummary.Should().BeEquivalentTo(new
            {
                Name = supplier.Name,
                Years = new[]
                {
                    new
                    {
                        Year = 2021,
                        TotalSpend = 100
                    }
                }
            });
        }

        [Fact]
        public void ShouldReturnInvoicesFromFailoverInvoicesAfter3Errors()
        {
            var supplier = _harness.AddExternalSupplier();
            _harness.AddFailoverInvoiceCollection(supplier, DateTime.Today, new ExternalInvoice
                {
                    TotalAmount = 5,
                    Year = 2020
                }, new ExternalInvoice
                {
                    TotalAmount = 6,
                    Year = 2021
                },
                new ExternalInvoice
                {
                    TotalAmount = 22,
                    Year = 2020
                });
            _harness.AddExternalInvoiceAction(() => throw new TimeoutException("Boom!"));
            _harness.AddExternalInvoiceAction(() => throw new TimeoutException("Boom!"));
            _harness.AddExternalInvoiceAction(() => throw new TimeoutException("Boom!"));
            _harness.AddExternalInvoiceAction(() => throw new TimeoutException("Boom!"));

            var spendService = _harness.CreateSpendService();
            var spendSummary = spendService.GetTotalSpend(supplier.Id);

            spendSummary.Should().BeEquivalentTo(new
            {
                Name = supplier.Name,
                Years = new[]
                {
                    new
                    {
                        Year = 2020,
                        TotalSpend = 27
                    },
                    new
                    {
                        Year = 2021,
                        TotalSpend = 6
                    }
                }
            });
        }
    }
}