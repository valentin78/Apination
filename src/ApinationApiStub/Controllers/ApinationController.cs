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
                HeartBeatCronSchedule = "0 0/1 * * * ?",
                
                Sage50CronSchedule = "0/30 * * * * ?",
                ApinationCronSchedule = "0/30 * * * * ?",
                
                ApinationActionEndpointUrl = "api/sage50DTO",
            };
        }

        [HttpGet]
        [Route("api/actions")]
        public dynamic Actions()
        {
            return new []
            {
                new {
                    type = "UpdateCustomer",
                    companyName = "Chase Ridge Holdings",
                    payload = new Customer
                    {
                        Id = "CST01",
                        Name = "Apination 1"
                    }
                },
                new {
                    type = "UpdateCustomer",
                    companyName = "Chase Ridge Holdings",
                    payload = new Customer
                    {
                        Id = "CST02",
                        Name = "Apination 2"
                    }
                }
            };
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
}
