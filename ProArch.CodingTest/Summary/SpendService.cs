using System.Linq;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Summary
{
    public class SpendService
    {
        private readonly ISupplierService _supplierService;
        private readonly YearAmountsQueryHandler _yearAmountsQueryHandler;

        public SpendService(ISupplierService supplierService, YearAmountsQueryHandler yearAmountsQueryHandler)
            => (_supplierService, _yearAmountsQueryHandler) =
                (supplierService, yearAmountsQueryHandler);

        public SpendSummary GetTotalSpend(int supplierId)
        {
            var supplier = _supplierService.GetById(supplierId);

            var yearAmounts = _yearAmountsQueryHandler.Handle(supplier);

            var years = yearAmounts
                .GroupBy(x => x.Year, (year, yearAmount)
                    => new SpendDetail { Year = year, TotalSpend = yearAmount.Sum(x => x.Amount) })
                .ToList();

            return new SpendSummary
            {
                Name = supplier.Name,
                Years = years
            };
        }
    }
}