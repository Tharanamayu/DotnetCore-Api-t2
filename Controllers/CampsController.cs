using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
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
        //get the instance of AutoMapper
        private readonly IMapper _mapper;
        public CampsController(ICampRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        //simple GET method ,it returns object
        [HttpGet]
        //added return with status code.
        public async Task<ActionResult<CampModel[]>> Get()//need to change method to async method
        {   //added try catch block
            try 
            {
                var results = await _repository.GetAllCampsAsync();//used async method in the repository
                
                return _mapper.Map<CampModel[]>(results);//results map to CampModel
            } catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");//added exception status code with meaningful exception
            }
            
        }
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>>Get(String moniker)
        {
            try
            {
                var results = await _repository.GetCampAsync(moniker);
                if (results == null) return NotFound();
                return _mapper.Map<CampModel>(results);
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database error");
            }
        }
    }
}
