using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class AdService : IAdService
{
    private readonly string _ldapPath;
    private readonly string _domain;
    private readonly ILogger<AdService> _logger;

    public AdService(IConfiguration config, ILogger<AdService> logger)
    {
        _ldapPath = config["ActiveDirectory:LdapPath"]
                    ?? throw new Exception("LdapPath no configurado");
        _domain = config["ActiveDirectory:NetBiosDomain"]
                    ?? throw new Exception("NetBiosDomain no configurado");
        _logger = logger;
    }

    public async Task<AdUser?> AuthenticateAsync(string username, string password)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var context = new PrincipalContext(
                    ContextType.Domain,
                    _domain,
                    username,
                    password);

                bool isValid = context.ValidateCredentials(username, password);
                if (!isValid) return null;

                return GetUserInternal(username, password);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("AD auth failed for {User}: {Message}",
                                   username, ex.Message);
                return null;
            }
        });
    }

    public async Task<AdUser?> GetUserAsync(string username)
    {
        return await Task.Run(() =>
        {
            try
            {
                return GetUserInternal(username, null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("AD lookup failed for {User}: {Message}",
                                   username, ex.Message);
                return null;
            }
        });
    }

    private AdUser? GetUserInternal(string username, string? password)
    {
        try
        {
            DirectoryEntry entry;

            if (!string.IsNullOrEmpty(password))
                entry = new DirectoryEntry(
                    _ldapPath,
                    $"{_domain}\\{username}",
                    password,
                    AuthenticationTypes.Secure);
            else
                entry = new DirectoryEntry(_ldapPath);

            using var searcher = new DirectorySearcher(entry)
            {
                Filter = $"(&(objectClass=user)(sAMAccountName={username}))"
            };

            searcher.PropertiesToLoad.AddRange(new[]
            {
                "sAMAccountName", "displayName", "mail",
                "department", "title", "memberOf"
            });

            var result = searcher.FindOne();
            if (result is null) return null;

            var user = new AdUser
            {
                Username = GetProp(result, "sAMAccountName"),
                DisplayName = GetProp(result, "displayName"),
                Email = GetProp(result, "mail"),
                Department = GetProp(result, "department"),
                Title = GetProp(result, "title"),
                Groups = GetGroups(result)
            };

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("AD internal error for {User}: {Message}",
                             username, ex.Message);
            return null;
        }
    }

    private static string GetProp(SearchResult result, string prop)
    {
        if (result.Properties.Contains(prop) &&
            result.Properties[prop].Count > 0)
            return result.Properties[prop][0]?.ToString() ?? string.Empty;
        return string.Empty;
    }

    private static List<string> GetGroups(SearchResult result)
    {
        var groups = new List<string>();
        if (!result.Properties.Contains("memberOf")) return groups;

        foreach (var group in result.Properties["memberOf"])
        {
            var dn = group?.ToString() ?? string.Empty;
            // Extraer CN del distinguished name: CN=Trade_Staff,OU=...
            var cn = dn.Split(',')
                       .FirstOrDefault(p => p.TrimStart().StartsWith("CN="))
                       ?.Replace("CN=", "")
                       .Trim();
            if (!string.IsNullOrEmpty(cn))
                groups.Add(cn);
        }

        return groups;
    }
}