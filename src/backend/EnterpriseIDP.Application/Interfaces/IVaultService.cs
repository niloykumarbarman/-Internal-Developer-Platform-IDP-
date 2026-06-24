namespace EnterpriseIDP.Application.Interfaces;

public interface IVaultService
{
    Task<string?> GetSecretAsync(string path, string key);
    Task<bool> SetSecretAsync(string path, string key, string value);
    Task<bool> DeleteSecretAsync(string path, string key);
    Task<Dictionary<string, string>> GetAllSecretsAsync(string path);
    Task<bool> RotateSecretAsync(string path, string key);
    Task<bool> EnableKubernetesAuthAsync(string k8sHost, string caCert, string serviceAccountJwt);
    Task<string?> LoginWithKubernetesAsync(string role, string jwt);
}
