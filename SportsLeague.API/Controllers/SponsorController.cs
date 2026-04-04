using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;
    private readonly ILogger<SponsorController> _logger;

    public SponsorController(
        ISponsorService sponsorService,
        IMapper mapper,
        ILogger<SponsorController> logger)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet] // Get
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        var sponsorsDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
        return Ok(sponsorsDto);
    }

    [HttpGet("{id}")] // Get By ID
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);

        if (sponsor == null)
            return NotFound(new { message = $"Sponsor with ID {id} not found" });

        var sponsorDto = _mapper.Map<SponsorResponseDTO>(sponsor);
        return Ok(sponsorDto);
    }

    [HttpPost] // Post
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            var createdSponsor = await _sponsorService.CreateAsync(sponsor);
            var responseDto = _mapper.Map<SponsorResponseDTO>(createdSponsor);

            return CreatedAtAction(
                nameof(GetById),
                new { id = responseDto.Id },
                responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")] // Put
    public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")] // Delete
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Asociar un torneo a un patrocinador
    [HttpPost("{id}/tournaments")] 
    public async Task<ActionResult> RegisterTournament(int id, TournamentSponsorRequestDTO dto)
    {
        try
        {
            await _sponsorService.RegisterForTournamentAsync(id, dto.TournamentId, dto.ContractAmount);
            return Ok(new { message = "Tournament successfully registered" });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    // Torneos asociados a un patrocinador
    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentResponseDTO>>> GetTournaments(int id)
    {
        try
        {
            var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(id);
            return Ok(_mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    // Eliminar un torneo de un patrocinador
    [HttpDelete("{id}/tournaments/{tournamentId}")] 
    public async Task<ActionResult> UnregisterTournament(int id, int tournamentId)
    {
        try
        {
            await _sponsorService.LeaveTournamentAsync(id, tournamentId);

            return Ok(new { message = "Sponsor successfully withdrawn from the tournament" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}