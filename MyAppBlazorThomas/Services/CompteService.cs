namespace MyAppBlazorThomas.Services;

using Microsoft.JSInterop;

public class CompteService
{
    private const string StorageKey = "kaamelott.compte";
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;

    public CompteService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? Login { get; private set; }

    public bool EstConnecte => !string.IsNullOrWhiteSpace(Login);

    public async Task InitialiserAsync()
    {
        if (_isInitialized)
            return;

        Login = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        _isInitialized = true;
    }

    public async Task ConnexionAsync(string login)
    {
        login = (login ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login invalide", nameof(login));

        Login = login;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, login);
    }

    public async Task DeconnexionAsync()
    {
        Login = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }

    public string GetLoginStorageSafe()
    {
        return Uri.EscapeDataString((Login ?? string.Empty).Trim());
    }
}
