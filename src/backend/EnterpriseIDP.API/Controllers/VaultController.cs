using EnterpriseIDP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VaultController : ControllerBase
{
    private readonly IVaultService _vaultService;
    private readonly ILogger<VaultController> _logger;

    public VaultController(IVaultService vaultService, ILogger<VaultController> logger)
    {
        _vaultService = vaultService;
        _logger = logger;
    }

    /// <summary>Get a specific secret</summary>
    [HttpGet("{path}/{key}")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> GetSecret(string path, string key)
    {
        var value = await _vaultService.GetSecretAsync(path, key);
        if (value == null)
            return NotFound(new { message = $"Secret not found: {path}/{key}" });

        return Ok(new { path, key, value });
    }

    /// <summary>Get all secrets at a path</summary>
    [HttpGet("{path}")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> GetAllSecrets(string path)
    {
        var secrets = await _vaultService.GetAllSecretsAsync(path);
        return Ok(new { path, secrets, count = secrets.Count });
    }

    /// <summary>Set a secret</summary>
    [HttpPost("{path}/{key}")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> SetSecret(string path, string key, [FromBody] SetSecretRequest request)
    {
        var success = await _vaultService.SetSecretAsync(path, key, request.Value);
        if (!success)
            return BadRequest(new { message = "Failed to set secret" });

        _logger.LogInformation("Secret set: {Path}/{Key} by {User}", path, key, User.Identity?.Name);
        return Ok(new { message = "Secret set successfully", path, key });
    }

    /// <summary>Delete a secret</summary>
    [HttpDelete("{path}/{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSecret(string path, string key)
    {
        var success = await _vaultService.DeleteSecretAsync(path, key);
        if (!success)
            return BadRequest(new { message = "Failed to delete secret" });

        _logger.LogInformation("Secret deleted: {Path}/{Key} by {User}", path, key, User.Identity?.Name);
        return Ok(new { message = "Secret deleted successfully" });
    }

    /// <summary>Rotate a secret</summary>
    [HttpPost("{path}/{key}/rotate")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> RotateSecret(string path, string key)
    {
        var success = await _vaultService.RotateSecretAsync(path, key);
        if (!success)
            return BadRequest(new { message = "Failed to rotate secret" });

        _logger.LogInformation("Secret rotated: {Path}/{Key} by {User}", path, key, User.Identity?.Name);
        return Ok(new { message = "Secret rotated successfully", path, key, rotatedAt = DateTime.UtcNow });
    }

    /// <summary>Health check for Vault connectivity</summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<IActionResult> Health()
    {
        try
        {
            var testSecret = await _vaultService.GetSecretAsync("health", "ping");
            return Ok(new { status = "connected", timestamp = DateTime.UtcNow });
        }
        catch
        {
            return StatusCode(503, new { status = "disconnected", timestamp = DateTime.UtcNow });
        }
    }
}

public record SetSecretRequest(string Value);
