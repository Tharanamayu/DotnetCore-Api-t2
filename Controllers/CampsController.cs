using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{   [Route("api/[controller]")]
    public class CampsController: ControllerBase
    {   //get the instance of repositiry
        private readonly ICampRepository _repository;
        public CampsController(ICampRepository repository)
        {
            _repository = repository;
        }
        //simple GET method ,it returns object
        [HttpGet]
        //added return with status code.
        public async Task<IActionResult> Get()//need to change method to async method
        {
            var results = await _repository.GetAllCampsAsync();//used async method in the repository

            return Ok (results);
        }
    }
}
