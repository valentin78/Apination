using System.Collections.Generic;

namespace Sage50Connector.Models.Payloads
{
    public class Employee
    {
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public bool IsSalesRepresentative { get; }
        public string Email { get; }
        public string Name { get; }
        public string ID { get; }
    }
}