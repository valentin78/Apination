﻿using Sage.Peachtree.API;

namespace Sage50Connector.Models.Payloads
{
    public class Account
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public bool IsInactive { get; set; }
        public AccountClassification Classification { get; set; }
    }
}
