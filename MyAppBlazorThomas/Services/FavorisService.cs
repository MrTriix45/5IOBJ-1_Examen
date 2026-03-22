namespace MyAppBlazorThomas.Services;

using System.Text.Json;
using Microsoft.JSInterop;

public class KaamelottCitationInfo
{
    public string Auteur { get; set; }
    public string Acteur { get; set; }
    public string Personnage { get; set; }
    public string Saison { get; set; }
    public string Episode { get; set; }
}

public class KaamelottCitation
{
    public string Citation { get; set; }
    public KaamelottCitationInfo Infos { get; set; }
}

public class KaamelottResponse
{
    public int Status { get; set; }
    public List<KaamelottCitation> Citation { get; set; }
}

public class FavorisService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly CompteService _compteService;
    private string? _storageKeyLoadedFor;

    public FavorisService(IJSRuntime jsRuntime, CompteService compteService)
    {
        _jsRuntime = jsRuntime;
        _compteService = compteService;
    }

    public List<KaamelottCitation> Favoris { get; } = new();

    public async Task InitialiserAsync()
    {
        await _compteService.InitialiserAsync();

        string storageKey = GetStorageKey();
        if (string.Equals(_storageKeyLoadedFor, storageKey, StringComparison.Ordinal))
            return;

        string? rawFavoris = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", storageKey);
        if (!string.IsNullOrWhiteSpace(rawFavoris))
        {
            var favorisCharges = JsonSerializer.Deserialize<List<KaamelottCitation>>(rawFavoris);
            if (favorisCharges is not null)
            {
                Favoris.Clear();
                Favoris.AddRange(favorisCharges);
            }
        }
        else
        {
            Favoris.Clear();
        }

        _storageKeyLoadedFor = storageKey;
    }

    public bool EstEnFavoris(KaamelottCitation citation)
    {
        return Favoris.Any(f => MemeCitation(f, citation));
    }

    public async Task AjouterAsync(KaamelottCitation citation)
    {
        if (!EstEnFavoris(citation))
        {
            Favoris.Add(citation);
            await SauvegarderAsync();
        }
    }

    public async Task RetirerAsync(KaamelottCitation citation)
    {
        var element = Favoris.FirstOrDefault(f => MemeCitation(f, citation));
        if (element is not null)
        {
            Favoris.Remove(element);
            await SauvegarderAsync();
        }
    }

    private async Task SauvegarderAsync()
    {
        await _compteService.InitialiserAsync();

        string storageKey = GetStorageKey();

        string favorisJson = JsonSerializer.Serialize(Favoris);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", storageKey, favorisJson);
    }

    private string GetStorageKey()
    {
        if (_compteService.EstConnecte)
        {
            return $"kaamelott.favoris.{_compteService.GetLoginStorageSafe()}";
        }

        return "kaamelott.favoris.anonyme";
    }

    private static bool MemeCitation(KaamelottCitation left, KaamelottCitation right)
    {
        return string.Equals(left.Citation, right.Citation, StringComparison.Ordinal)
            && string.Equals(left.Infos.Personnage, right.Infos.Personnage, StringComparison.Ordinal)
            && string.Equals(left.Infos.Saison, right.Infos.Saison, StringComparison.Ordinal)
            && string.Equals(left.Infos.Episode, right.Infos.Episode, StringComparison.Ordinal);
    }
}
