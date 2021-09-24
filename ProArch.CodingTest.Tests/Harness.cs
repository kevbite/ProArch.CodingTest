using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoFixture;
using ProArch.CodingTest.External;
using ProArch.CodingTest.ExternalInvoices;
using ProArch.CodingTest.Invoices;
using ProArch.CodingTest.Summary;
using ProArch.CodingTest.Suppliers;

namespace ProArch.CodingTest.Tests
{
    public class Harness : ISupplierService, IInvoiceRepository, IExternalInvoiceServiceWrapper, IFailoverInvoiceService
    {
        private readonly List<Supplier> _suppliers = new();
        private readonly List<Invoice> _internalInvoices = new();
        private readonly List<(int supplierId, ExternalInvoice invoice)> _externalInvoices = new();
        private readonly Dictionary<int, FailoverInvoiceCollection> _failoverlInvoices = new();
        private readonly Fixture _fixture = new();
        private readonly Queue<Action> _externalInvoiceGetInvoicesActions = new();
        private int _externalInvoiceServiceCallCount = 0;
        public int ExternalInvoiceServiceCallCount => _externalInvoiceServiceCallCount;
        Supplier ISupplierService.GetById(int id) => _suppliers.SingleOrDefault(x => x.Id == id);

        IQueryable<Invoice> IInvoiceRepository.Get() => _internalInvoices.AsQueryable();

        ExternalInvoice[] IExternalInvoiceServiceWrapper.GetInvoices(string supplierId)
        {
            Interlocked.Increment(ref _externalInvoiceServiceCallCount);
            
            if (_externalInvoiceGetInvoicesActions.TryDequeue(out var action))
                action.Invoke();

            return _externalInvoices.Where(x => x.supplierId.ToString() == supplierId)
                .Select(x => x.invoice)
                .ToArray();
        }
        
        FailoverInvoiceCollection IFailoverInvoiceService.GetInvoices(int supplierId) => _failoverlInvoices[supplierId];
        
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
            var externalInvoiceServiceResilienceDecorator = new ExternalInvoiceServiceResilienceDecorator(this, this);

            var yearAmountsQueryHandler = new YearAmountsQueryHandler(this, externalInvoiceServiceResilienceDecorator);

            return new SpendService(this, yearAmountsQueryHandler);
        }

        public void AddExternalInvoiceAction(Action action)
        {
            _externalInvoiceGetInvoicesActions.Enqueue(action);
        }

        public void AddExternalInvoice(Supplier supplier, int amount, int year)
        {
            var invoice = _fixture.Build<ExternalInvoice>()
                .With(x => x.TotalAmount, amount)
                .With(x => x.Year, year)
                .Create();

            _externalInvoices.Add((supplier.Id, invoice));
        }

        public FailoverInvoiceCollection AddFailoverInvoiceCollection(Supplier supplier, DateTime timestamp, params ExternalInvoice[] invoices)
        {
            var collection = new FailoverInvoiceCollection
            {
                Timestamp = timestamp,
                Invoices = invoices
            };
            _failoverlInvoices.Add(supplier.Id, collection);

            return collection;
        }

    }
}