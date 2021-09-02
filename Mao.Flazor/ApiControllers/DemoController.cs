using Mao.Flazor.Features.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mao.Flazor.ApiControllers
{
    [RoutePrefix("api/Demo")]
    public class DemoController : ApiController
    {
        private readonly DemoService _demoService;
        public DemoController(DemoService demoService)
        {
            _demoService = demoService;
        }

        [HttpGet, Route("GetSecondaryOptions")]
        public IHttpActionResult GetSecondaryOptions(string primarySelectedValue)
        {
            var secondaryOptions = _demoService.GetSecondaryOptions(primarySelectedValue);
            return Ok(secondaryOptions);
        }
    }
}
