using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ProArch.CodingTest.Invoices;
using ProArch.CodingTest.Summary;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Tests
{
    public class Harness : ISupplierService, IInvoiceRepository
    {
        private List<Supplier> _suppliers = new();
        private List<Invoice> _internalInvoices = new();
        private readonly Fixture _fixture = new();

        Supplier ISupplierService.GetById(int id)
        {
            return _suppliers.SingleOrDefault(x => x.Id == id);
        }

        IQueryable<Invoice> IInvoiceRepository.Get()
        {
            return _internalInvoices.AsQueryable();
        }

        public Invoice AddInternalInvoice(Supplier supplier, decimal amount, int year)
        {
            var invoice = _fixture.Build<Invoice>()
                .With(x => x.SupplierId, supplier.Id)
                .With(x => x.Amount, amount)
                .Do(x => x.InvoiceDate = x.InvoiceDate.ChangeYear(year))
                .Create();
            
            _internalInvoices.Add(invoice);

            return invoice;
        }
        
        public Supplier AddInternalSupplier()
        {
            var supplier = _fixture.Build<Supplier>()
                .With(x => x.IsExternal, false)
                .Create();

            _suppliers.Add(supplier);
            
            return supplier;
        }

        public SpendService CreateSpendService()
        {
            return new SpendService(this, this);
        }
    }
}