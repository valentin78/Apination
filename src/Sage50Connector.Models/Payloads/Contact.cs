using System.Collections.Generic;
using Sage.Peachtree.API;

namespace Sage50Connector.Models.Payloads
{
    public class Contact
    {
        public Contact()
        {
            Address = new Address();
        }
        public bool IsPrimaryContact { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public string Notes { get; set; }
        public NameToUseOnForms NameToUseOnForms { get; set; }
        public string Name { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public Address Address { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public Gender Gender { get; set; }
    }
}