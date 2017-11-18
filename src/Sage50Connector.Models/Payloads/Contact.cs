using System.Collections.Generic;

namespace Sage50Connector.Models.Payloads
{
    public class Contact
    {
        public bool IsPrimaryContact { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public string Notes { get; set; }
        // TODO replace SAGE enums
        //public NameToUseOnForms NameToUseOnForms { get; set; }
        public string Name { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public Address Address { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        // TODO replace SAGE enums
        //public Gender Gender { get; set; }
    }
}