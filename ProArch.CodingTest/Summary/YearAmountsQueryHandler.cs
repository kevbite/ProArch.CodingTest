using System.Linq;
using ProArch.CodingTest.ExternalInvoices;
using ProArch.CodingTest.Invoices;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Summary
{
    public class YearAmountsQueryHandler
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IExternalInvoiceServiceWrapper _externalInvoiceService;

        public YearAmountsQueryHandler(IInvoiceRepository invoiceRepository,
            IExternalInvoiceServiceWrapper externalInvoiceService)
            => (_invoiceRepository, _externalInvoiceService) =
                (invoiceRepository, externalInvoiceService);

        public IQueryable<YearAmount> Handle(Supplier supplier)
        {
            if (supplier.IsExternal)
            {
                return _externalInvoiceService.GetInvoices(supplier.Id.ToString())
                    .AsQueryable()
                    .Select(x => new YearAmount(x.Year, x.TotalAmount));
            }

            return _invoiceRepository.Get()
                .Where(x => x.SupplierId == supplier.Id)
                .Select(x => new YearAmount(x.InvoiceDate.Year, x.Amount));
        }

        public class YearAmount
        {
            public int Year { get; }
            public decimal Amount { get; }

            public YearAmount(int year, decimal amount)
            {
                Year = year;
                Amount = amount;
            }
        }
    }
}