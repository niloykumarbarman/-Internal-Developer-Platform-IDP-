using System.Text;
using System.Text.Json;
using EnterpriseIDP.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Infrastructure.Services;

public class VaultService : IVaultService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VaultService> _logger;
    private readonly string _vaultAddress;
    private readonly string _vaultToken;
    private readonly string _mountPath;

    public VaultService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<VaultService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _vaultAddress = configuration["Vault:Address"] ?? "http://vault:8200";
        _vaultToken = configuration["Vault:Token"] ?? string.Empty;
        _mountPath = configuration["Vault:MountPath"] ?? "secret";

        _httpClient.BaseAddress = new Uri(_vaultAddress);
        _httpClient.DefaultRequestHeaders.Add("X-Vault-Token", _vaultToken);
    }

    public async Task<string?> GetSecretAsync(string path, string key)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/v1/{_mountPath}/data/{path}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Vault secret not found: {Path}/{Key}", path, key);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);
            var data = doc.RootElement
                .GetProperty("data")
                .GetProperty("data");

            return data.TryGetProperty(key, out var val) ? val.GetString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving secret from Vault: {Path}/{Key}", path, key);
            return null;
        }
    }

    public async Task<bool> SetSecretAsync(string path, string key, string value)
    {
        try
        {
            var payload = new
            {
                data = new Dictionary<string, string> { { key, value } }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/v1/{_mountPath}/data/{path}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting secret in Vault: {Path}/{Key}", path, key);
            return false;
        }
    }

    public async Task<bool> DeleteSecretAsync(string path, string key)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/v1/{_mountPath}/data/{path}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting secret from Vault: {Path}", path);
            return false;
        }
    }

    public async Task<Dictionary<string, string>> GetAllSecretsAsync(string path)
    {
        var result = new Dictionary<string, string>();
        try
        {
            var response = await _httpClient.GetAsync($"/v1/{_mountPath}/data/{path}");
            if (!response.IsSuccessStatusCode) return result;

            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);
            var data = doc.RootElement
                .GetProperty("data")
                .GetProperty("data");

            foreach (var prop in data.EnumerateObject())
                result[prop.Name] = prop.Value.GetString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all secrets from Vault: {Path}", path);
        }
        return result;
    }

    public async Task<bool> RotateSecretAsync(string path, string key)
    {
        try
        {
            var newValue = Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
            var success = await SetSecretAsync(path, key, newValue);
            if (success)
                _logger.LogInformation("Secret rotated: {Path}/{Key}", path, key);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rotating secret: {Path}/{Key}", path, key);
            return false;
        }
    }

    public async Task<bool> EnableKubernetesAuthAsync(string k8sHost, string caCert, string serviceAccountJwt)
    {
        try
        {
            var enablePayload = new { type = "kubernetes" };
            var enableJson = JsonSerializer.Serialize(enablePayload);
            await _httpClient.PostAsync("/v1/sys/auth/kubernetes",
                new StringContent(enableJson, Encoding.UTF8, "application/json"));

            var configPayload = new
            {
                kubernetes_host = k8sHost,
                kubernetes_ca_cert = caCert,
                token_reviewer_jwt = serviceAccountJwt
            };

            var configJson = JsonSerializer.Serialize(configPayload);
            var configResponse = await _httpClient.PostAsync("/v1/auth/kubernetes/config",
                new StringContent(configJson, Encoding.UTF8, "application/json"));

            return configResponse.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling Kubernetes auth in Vault");
            return false;
        }
    }

    public async Task<string?> LoginWithKubernetesAsync(string role, string jwt)
    {
        try
        {
            var payload = new { role, jwt };
            var json = JsonSerializer.Serialize(payload);
            var response = await _httpClient.PostAsync("/v1/auth/kubernetes/login",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);
            return doc.RootElement
                .GetProperty("auth")
                .GetProperty("client_token")
                .GetString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in with Kubernetes auth");
            return null;
        }
    }
}
