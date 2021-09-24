using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ProArch.CodingTest.External;
using ProArch.CodingTest.ExternalInvoices;
using ProArch.CodingTest.Invoices;
using ProArch.CodingTest.Summary;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Tests
{
    public class Harness : ISupplierService, IInvoiceRepository, IExternalInvoiceServiceWrapper
    {
        private readonly List<Supplier> _suppliers = new();
        private readonly List<Invoice> _internalInvoices = new();
        private readonly List<(int supplierId, ExternalInvoice invoice)> _externalInvoices = new();
        private readonly Fixture _fixture = new();

        Supplier ISupplierService.GetById(int id) => _suppliers.SingleOrDefault(x => x.Id == id);

        IQueryable<Invoice> IInvoiceRepository.Get() => _internalInvoices.AsQueryable();

        ExternalInvoice[] IExternalInvoiceServiceWrapper.GetInvoices(string supplierId)
            => _externalInvoices.Where(x => x.supplierId.ToString() == supplierId)
                .Select(x => x.invoice)
                .ToArray();

        public void AddInternalInvoice(Supplier supplier, decimal amount, int year)
        {
            var invoice = _fixture.Build<Invoice>()
                .With(x => x.SupplierId, supplier.Id)
                .With(x => x.Amount, amount)
                .Create();

            invoice.InvoiceDate = invoice.InvoiceDate.ChangeYear(year);
            _internalInvoices.Add(invoice);
        }

        public Supplier AddInternalSupplier() => AddSupplier(false);
        public Supplier AddExternalSupplier() => AddSupplier(true);

        private Supplier AddSupplier(bool isExternal)
        {
            var supplier = _fixture.Build<Supplier>()
                .With(x => x.IsExternal, isExternal)
                .Create();

            _suppliers.Add(supplier);

            return supplier;
        }

        public SpendService CreateSpendService()
        {
            return new SpendService(this, this, this);
        }

        public void AddExternalInvoice(Supplier supplier, int amount, int year)
        {
            var invoice = _fixture.Build<ExternalInvoice>()
                .With(x => x.TotalAmount, amount)
                .With(x => x.Year, year)
                .Create();

            _externalInvoices.Add((supplier.Id, invoice));
        }
    }
}