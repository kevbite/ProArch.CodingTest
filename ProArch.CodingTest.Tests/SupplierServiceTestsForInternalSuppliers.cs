using FluentAssertions;
using Xunit;

namespace ProArch.CodingTest.Tests
{
    public class SupplierServiceTestsForInternalSuppliers
    {
        private readonly Harness _harness = new();

        [Fact]
        public void ShouldReturnSummaryOfASingleInvoice()
        {
            var supplier = _harness.AddInternalSupplier();
            _harness.AddExternalInvoice(supplier, 100, 2021);

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

        [Fact]
        public void ShouldReturnSummaryOfOnlySuppliedSupplier()
        {
            var supplier1 = _harness.AddInternalSupplier();
            var supplier2 = _harness.AddInternalSupplier();
            _harness.AddInternalInvoice(supplier1, 50, 2000);
            _harness.AddInternalInvoice(supplier2, 150, 2021);

            var spendService = _harness.CreateSpendService();
            var spendSummary = spendService.GetTotalSpend(supplier2.Id);

            spendSummary.Should().BeEquivalentTo(new
            {
                Name = supplier2.Name,
                Years = new[]
                {
                    new
                    {
                        Year = 2021,
                        TotalSpend = 150
                    }
                }
            });
        }

        [Fact]
        public void ShouldReturnSummaryOfYearsSummed()
        {
            var supplier1 = _harness.AddInternalSupplier();
            _harness.AddInternalInvoice(supplier1, 10, 2000);
            _harness.AddInternalInvoice(supplier1, 20, 2000);
            _harness.AddInternalInvoice(supplier1, 30, 2001);
            _harness.AddInternalInvoice(supplier1, 40, 2001);
            _harness.AddInternalInvoice(supplier1, 50, 2002);

            var spendService = _harness.CreateSpendService();
            var spendSummary = spendService.GetTotalSpend(supplier1.Id);

            spendSummary.Should().BeEquivalentTo(new
            {
                Name = supplier1.Name,
                Years = new[]
                {
                    new
                    {
                        Year = 2000,
                        TotalSpend = 30
                    },
                    new
                    {
                        Year = 2001,
                        TotalSpend = 70
                    },
                    new
                    {
                        Year = 2002,
                        TotalSpend = 50
                    }
                }
            });
        }
    }
}