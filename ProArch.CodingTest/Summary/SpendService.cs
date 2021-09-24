using System.Collections.Generic;
using System.Linq;
using ProArch.CodingTest.Invoices;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Summary
{
    public class SpendService
    {
        private readonly ISupplierService _supplierService;
        private readonly IInvoiceRepository _invoiceRepository;

        public SpendService(ISupplierService supplierService, IInvoiceRepository invoiceRepository)
            => (_supplierService, _invoiceRepository) = (supplierService, invoiceRepository);

        public SpendSummary GetTotalSpend(int supplierId)
        {
            var supplier = _supplierService.GetById(supplierId);
            if (!supplier.IsExternal)
            {
                var totalSpend = _invoiceRepository.Get().First();

                return new SpendSummary
                {
                    Name = supplier.Name, Years = new List<SpendDetail>
                    {
                        new SpendDetail { Year = totalSpend.InvoiceDate.Year, TotalSpend = totalSpend.Amount }
                    }
                };
            }

            return null;
        }
    }
}