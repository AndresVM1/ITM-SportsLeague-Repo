using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(ISponsorRepository sponsorRepository, ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all Sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving Sponsor with ID: {SponsorId}", id);
        var sponsor = await _sponsorRepository.GetByIdAsync(id);

        if (sponsor == null)
            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

        return sponsor;
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        // Validaciones

        // Evitar duplicados
        var existingSponsor = await _sponsorRepository.ExistByNameAsync(sponsor.Name);
        if (existingSponsor != null)
        {
            _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
            throw new InvalidOperationException(
                $"There is already a sponsor with that name: '{sponsor.Name}'");
        }

        // Email con formato único
        var emailValidator = new EmailAddressAttribute();

        if (string.IsNullOrWhiteSpace(sponsor.ContactEmail) ||
            !emailValidator.IsValid(sponsor.ContactEmail))
        {
            _logger.LogWarning("Invalid email format for sponsor: {Email}", sponsor.ContactEmail);
            throw new InvalidOperationException("The email address is not in a valid format");
        }

        _logger.LogInformation("Creating Sponsor: {SponsorName}", sponsor.Name);
        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
        if (existingSponsor == null)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
            throw new KeyNotFoundException(
                $"No sponsor with that ID was found: {id}");
        }

        if (existingSponsor.Name != sponsor.Name)
        {
            var sponsorWithSameName = await _sponsorRepository.ExistByNameAsync(sponsor.Name);
            if (sponsorWithSameName != null)
            {
                throw new InvalidOperationException(
                    $"There is already a sponsor with that name '{sponsor.Name}'");
            }
        }

        var emailValidator = new EmailAddressAttribute();

        if (string.IsNullOrWhiteSpace(sponsor.ContactEmail) ||
            !emailValidator.IsValid(sponsor.ContactEmail))
        {
            _logger.LogWarning("Invalid email format for sponsor: {Email}", sponsor.ContactEmail);
            throw new InvalidOperationException("The email address is not in a valid format");
        }

        existingSponsor.Name = sponsor.Name;
        existingSponsor.ContactEmail = sponsor.ContactEmail;
        existingSponsor.Phone = sponsor.Phone;
        existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
        existingSponsor.Category = sponsor.Category;

        _logger.LogInformation("Updating Sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.UpdateAsync(existingSponsor);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _sponsorRepository.ExistsAsync(id);
        if (!exists)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
            throw new KeyNotFoundException(
                $"No sponsor with that ID was found: {id}");
        }

        _logger.LogInformation("Deleting Sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.DeleteAsync(id);
    }
}