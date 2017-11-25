using System;

namespace Sage50Connector.Models.Data
{
    public abstract class Transaction
    {
        public DateTime Date { get; set; }
        public string ReferenceNumber { get; set; }
        public Account Account { get; set; }
    }
}