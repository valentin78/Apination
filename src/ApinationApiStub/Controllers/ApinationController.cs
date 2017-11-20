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
            return new[]
            {
                new {
                    type = "UpdateCustomer",
                    id = "1",
                    companyName = "Chase Ridge Holdings",
                    payload = new Customer
                    {
                        Id = "1",
                        Name = "Customer 1",
                        CashAccount = new Account
                        {
                            Id = "ACC1",
                            IsInactive = false, 
                            Description = "Test"
                        }
                    }
                },
                new {
                    type = "UpdateCustomer",
                    id = "3",
                    companyName = "Chase Ridge Holdings",
                    payload = new Customer
                    {
                        Id = "3",
                        Name = "Customer 2"
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
