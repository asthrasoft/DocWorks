using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docu3cDemoWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class docu3cAPIController : ControllerBase
    {
        [HttpGet]
        public async Task<docu3clist> Get()
        {
            docu3cAPI d3 = new docu3cAPI();
            var docs = await d3.ClassifyDocument("comp", "https://docworksweb.azurewebsites.net/data/1593383499/ACC_XFR_Abraham%20C%20Diaz_.pdf");
            return docs;
        }
        // POST: api/docu3cAPI
        [HttpPost]
        public async Task<docu3clist> Post(docu3cInput parse)
        {
            docu3cAPI d3 = new docu3cAPI();
            var docs = await d3.ClassifyDocument("comp", "https://docworksweb.azurewebsites.net/data/1593383499/ACC_XFR_Abraham%20C%20Diaz_.pdf");
            return docs;
        }

        //// POST: api/version
        //[HttpPost]
        //public async Task<string> Ver([FromBody] string doc_type)
        //{
        //    return "0.45";
        //}
    }
}
