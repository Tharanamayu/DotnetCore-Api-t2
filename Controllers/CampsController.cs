using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{   [Route("api/[controller]")]
    public class CampsController: ControllerBase
    {   
        //simple GET method ,it returns object
        public object Get()
        {
            return new { Moniker = "AJK2019", name = "Atlantic" };
        }
    }
}
