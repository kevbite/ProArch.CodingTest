using FluentAssertions;
using Xunit;

namespace ProArch.CodingTest.Tests
{
    public class SupplierServiceTestsForInternalSuppliers
    {
        private readonly Harness _harness = new();

        [Fact]
        public void ShouldReturnSummaryOfASingleInvoiceForInternalSupplier()
        {
            var supplier = _harness.AddInternalSupplier();
            _harness.AddInternalInvoice(supplier, 100, 2021);

            var spendService = _harness.CreateSpendService();
            var spendSummary = spendService.GetTotalSpend(supplier.Id);

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
    }
}