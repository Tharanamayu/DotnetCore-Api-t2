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
    //vreate an association controller
{   [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController: ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository repository,IMapper mapper,LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker,true);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Fail to get talk model");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker,int id)
        {
            try
            {
                var talks = await _repository.GetTalkByMonikerAsync(moniker,id,true);
                return _mapper.Map<TalkModel>(talks);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Fail to get talk model");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker,TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);//getting the relevent camp
                if (camp == null) return BadRequest("Camp doesn't exist");

                var talk = _mapper.Map<Talk>(model);//conver Talkmodel to Talk entity 
                talk.Camp = camp;

                if (model.speaker == null) return BadRequest("Speaker id is required");
                var speaker = await _repository.GetSpeakerAsync(model.speaker.SpeakerId);//getting the relevent speaker from the speaker id
                if (speaker == null) return BadRequest("Speaker couldn't be found");
                talk.Speaker= speaker;//add the speaker data to talk entity
                _repository.Add(talk);


                if(await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext, "Get", values: new { moniker, id = talk.TalkId });//generate the link when the execute the post request and show the data set using get request
                    return Created(url, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Failed to save new Talk");
                }

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Fail to get talk model");
            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker,int id,TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker,id,true);//getting the relevent talk
                if (talk == null) return NotFound("Couldn't find the task");

                 _mapper.Map(model,talk); 

                if(model.speaker !=null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.speaker.SpeakerId);
                    if(speaker !=null)
                    {
                        talk.Speaker = speaker;
                    }
                }
                       
                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(talk);
                }
                else
                {
                    return BadRequest("Failed to update database");
                }
             

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Fail to get talk model");
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker,int id)
        {
            try
            {
                var talk = _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound("Failed to find the talk to delete");
                _repository.Delete(talk);

                if(await _repository.SaveChangesAsync())
                { return Ok();
                }
                else
                {
                    return BadRequest("Failed to delete talk");
                }
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Fail to get talk model");
            }
        }
    }
}
