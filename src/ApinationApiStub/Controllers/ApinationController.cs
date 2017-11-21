using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sage50Connector.Models;
using Sage50Connector.Models.Payloads;

namespace ApinationApiStub.Controllers
{
    public class ApinationController : Controller
    {
        // GET api/apination
        [HttpGet]
        [Route("api/config")]
        public Config Get()
        {
            return new Config
            {
                HeartBeatCronSchedule = "0 0/5 * * * ?",

                Sage50CronSchedule = "0 0/5 * * * ?",
                ApinationCronSchedule = "0 0/5 * * * ?",

                ApinationActionEndpointUrl = "api/sage50DTO",
            };
        }

        [HttpGet]
        [Route("api/actions")]
        public dynamic Actions()
        {
            return new dynamic[]
            {
                new {
                    type = "UpdateCustomer",
                    id = "1",
                    companyName = "Chase Ridge Holdings",
                    payload = new Customer
                    {
                        Id = "5",
                        Name = "Customer 1",
                        AccountNumber = "123",
                        BillToContact = new Contact()
                        {
                            FirstName = "Name",
                            LastName = "Lastname"
                        }
                    }
                },
                new {
                    type = "CreateInvoice",
                    id = "2",
                    companyName = "Chase Ridge Holdings",
                    payload = new SalesInvoice
                    {
                        FreightAmount = 0,
                        DiscountAmount = 0,
                        CustomerNote = "Note",
                        ShipDate = DateTime.Today,
                        ShipToAddress = new NameAndAddress()
                        {
                            Name = "Name 1",
                            Address = new Address
                            {
                                Address1 = "Addr1",
                                Address2 = "Addr2",
                            }
                        },
                        Customer = new Customer
                        {
                            Id = "3",
                            Name = "Customer 3",
                            AccountNumber = "123",
                            BillToContact = new Contact()
                            {
                                FirstName = "Name",
                                LastName = "Lastname"
                            }
                        }
                    }
                }
            };
        }

        [HttpPatch]
        [Route("api/actions")]
        public void ActionsPatch([FromBody]PatchAction[] list)
        {

        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }


    public class PatchAction
    {
        public string id { get; set; }
        public bool processed { get; set; }
    }
}
