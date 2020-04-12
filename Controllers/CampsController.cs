using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;
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
        {   //added try catch block
            try 
            {
                var results = await _repository.GetAllCampsAsync();//used async method in the repository

                return Ok(results);
            } catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");//added exception status code with meaningful exception
            }
            
        }
    }
}
