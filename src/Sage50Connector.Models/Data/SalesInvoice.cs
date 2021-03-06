﻿using System;
using System.Collections.Generic;

namespace Sage50Connector.Models.Data
{
    public class SalesInvoice
    {
        public string ReferenceNumber { get; set; }
        public string InternalNote { get; set; }
        public string TermsDescription { get; set; }
        public string StatementNote { get; set; }
        public string ShipVia { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ShipDate { get; set; }
        public bool PrintCustomerNoteAfterLineItems { get; set; }
        public decimal FreightAmount { get; set; }
        public bool DropShip { get; set; }
        public DateTime? DiscountDate { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime? DateDue { get; set; }
        public string CustomerPurchaseOrderNumber { get; set; }
        public string CustomerNote { get; set; }
        public Account FreightAccount { get; set; }
        public NameAndAddress ShipToAddress { get; set; }

        public Customer Customer { get; set; }

        public List<SalesInvoiceLine> SalesLines { get; set; }
        
        //public decimal Amount { get; set; }
        //public List<SalesInvoiceSalesLine> ApplyToSalesLines { get; set; }
        //readonly
        //public decimal AmountDue { get; set; }

        // TODO: factory not support create() 
        //public SalesTaxCode SalesTaxCode { get; set; }
        //public List<SalesInvoiceLine> ApplyToSalesOrderLines { get; set; }
        //public List<SalesInvoiceLine> ApplyToProposalLines { get; set; }
        //public Employee SalesRepresentativeReference { get; set; }
        //public List<SalesInvoiceRetainageLine> WithholdRetainageLines { get; set; }
    }
}