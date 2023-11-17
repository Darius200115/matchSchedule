﻿using AutoMapper;
using matchSchedule.Models;
using matchSchedule.ModelsDTO;
using matchSchedule.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace matchSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _service;
        private readonly ILogger<TournamentController> _logger;
        private readonly IMapper _mapper;
        public TournamentController(ITournamentService service, ILogger<TournamentController> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("tournaments")]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Failed to get tournaments!");
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Failed to get tournament!");
            }
        }


        [HttpPost("new")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> Post([FromBody] NewTournamentDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var teams = await _service.GetTeamsByIdAsync(model.TeamIds);
                    var newModel = _mapper.Map<NewTournamentDTO, Tournament>(model);
                    newModel.Teams = teams;
                    _service.AddEntity(newModel);
                    if (_service.SaveAll())
                    {
                        return Created($"/api/tournaments/{newModel.Id}", _mapper.Map<Tournament, NewTournamentDTO>(newModel));
                    }
                }
                else
                    return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return BadRequest("Failed to post the tournament!");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var tournament = await _service.GetByIdAsync(id);
                    if (tournament == null)
                        return NotFound($"The tournament with id: {id} wasn`t found");
                    _service.RemoveEntity(tournament);
                    if (_service.SaveAll())
                        return NoContent();
                }
                else
                    return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return BadRequest(new { Message = "Failed to delete the tournament!" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpPut("{id:Guid}/edit")]
        public async Task<ActionResult> Put(Guid id, [FromBody] TournamentEditDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editedTournament = await _service.EditTournamentByIdAsync(id, model);
                    if (editedTournament == null)
                        return BadRequest(NotFound());
                    return (Ok(editedTournament));
                }
                else
                    return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return BadRequest("Failed to post the match!");
        }

    }
}



