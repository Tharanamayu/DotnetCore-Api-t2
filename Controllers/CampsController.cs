using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{   [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")
    public class CampsController: ControllerBase
    {   //get the instance of repositiry
        private readonly ICampRepository _repository;
        //get the instance of AutoMapper
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public CampsController(ICampRepository repository,IMapper mapper,LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        //simple GET method ,it returns object
        [HttpGet]
        //added return with status code.
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks=false)//need to change method to async method/addping includeTalk parameter to use query string
        {   //added try catch block
            try 
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);//used async method in the repository/adding includeTalks to enable query string
                
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
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate,bool includeTalks)//searchby date using query string
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!results.Any()) return NotFound();
                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError,"Databse error");
            }
        }
       // [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existing = await _repository.GetCampAsync(model.Moniker);
                if(existing != null)
                {
                    return BadRequest("Moniker in use");
                }
                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not usecurrent moniker");
                }

                //create a new campmodel
                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);
                if(await _repository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{camp.Moniker}",_mapper.Map<Camp>(model));
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Databse error");
            }
            return BadRequest();
        }
        [HttpPut]
        public async Task<ActionResult<CampModel>> Put(CampModel model,string moniker)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Couldn't find camp with moniker of {moniker}");
                _mapper.Map(model, oldCamp);
                if(await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Databse error");
            }
            return BadRequest();
        }
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound();

                _repository.Delete(oldCamp);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Databse error");
            }
            return BadRequest();
        }
    }
}
