using System.Collections.Generic;
using System.Linq;
using ProArch.CodingTest.ExternalInvoices;
using ProArch.CodingTest.Invoices;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Summary
{
    public class SpendService
    {
        private readonly ISupplierService _supplierService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IExternalInvoiceServiceWrapper _externalInvoiceService;

        public SpendService(ISupplierService supplierService, IInvoiceRepository invoiceRepository,
            IExternalInvoiceServiceWrapper externalInvoiceService)
            => (_supplierService, _invoiceRepository, _externalInvoiceService) =
                (supplierService, invoiceRepository, externalInvoiceService);

        public SpendSummary GetTotalSpend(int supplierId)
        {
            var supplier = _supplierService.GetById(supplierId);
            
            if (supplier.IsExternal)
            {
                var years = _externalInvoiceService.GetInvoices(supplier.Id.ToString())
                    .GroupBy(x => x.Year, (year, invoices)
                        => new SpendDetail { Year = year, TotalSpend = invoices.Sum(x => x.TotalAmount) })
                    .ToList();
                
                return new SpendSummary
                {
                    Name = supplier.Name,
                    Years = years
                };
            }
            else
            {
                var years = _invoiceRepository.Get()
                    .Where(x => x.SupplierId == supplierId)
                    .GroupBy(x => x.InvoiceDate.Year,
                        (year, invoices) =>
                            new SpendDetail { Year = year, TotalSpend = invoices.Sum(x => x.Amount) })
                    .ToList();

                return new SpendSummary
                {
                    Name = supplier.Name,
                    Years = years
                };
            }
        }
    }
}