using Blazored.LocalStorage;
using System.Security.Cryptography;
using System.Windows.Markup;
using Blazor_Client.Models;

namespace Blazor_Client.Services;

public class AppStateService
{
    public static event Action OnChange;
    private ILocalStorageService _localStorageService;

    public AppStateService(ILocalStorageService localStorage)
    {
        _localStorageService = localStorage;
    }

    public static string? ShowModal { get; private set; } = "none";

    public static bool? ShouldRenderModal
    {
        set
        {
            if (value is false)
                ShowModal = "none";
            else ShowModal = "show-modal";
            NotifyStateChanged();
        }
    }
    public static string ModalBody { get; set; } = ":3";
    public static string? ShowProfile { get; private set; }
    public static string? ShowRL { get; private set; }
    private static Test? _Test { get; set; }
    private static bool? _LoggedIn { get; set; }

    public Test? Test
    {
        get
        {
            return _Test;
        }
        set
        {
            if (_Test != value)
                _Test = value;
            NotifyStateChanged();
        }
    }
    private bool? _ShowRL
    {
        set
        {
            if (value == false)
                ShowRL = "none";
            else ShowRL = "show";

            NotifyStateChanged();
        }
    }

    private bool? _ShowProfile
    {
        set
        {
            if (value == false)
                ShowProfile = "none";
            else ShowProfile = "show";
            NotifyStateChanged();
        }
    }
    public bool? LoggedIn
    {
        get
        {
            return _LoggedIn;
        }
        set
        {
            _LoggedIn = value;
            if (value == false)
            {
                _ShowProfile = false;
                _ShowRL = true;
            }

            else
            {
                _ShowProfile = true;
                _ShowRL = false;
            }
            NotifyStateChanged();
        }
    }

    public void SaveTestId(Test? test)
    {
        if (test is not null)
            _localStorageService.SetItemAsync<Test>("TestId", test);
    }

    public async void SaveTestAsync(Test test)
    {
        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User") && test is not null)
        {
            await _localStorageService.SetItemAsync<Test>("Test", test);
        }
    }

    public async Task<Test?> LoadTestAsync()
    {
        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User") && await _localStorageService.ContainKeyAsync("Test"))
        {
            return await _localStorageService.GetItemAsync<Test>("Test");
        }

        return _Test;
    }

    public async void SaveCreatedTest(string testName, List<Question> questions)
    {
        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            TestRequest test = new TestRequest { Name = testName, Author = "self", Questions= questions};
            await _localStorageService.SetItemAsync<TestRequest>("Cache", test);
        }
    }

    public async void DeleteSavedCacheTest()
    {
        await _localStorageService.RemoveItemAsync("Cache");
    }

    public async Task<TestRequest?> LoadSavedCacheTestAsync()
    {
        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            return await _localStorageService.GetItemAsync<TestRequest>("Cache");
        }

        return null;
    }

    public async void SaveTestAsync()
    {
        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User") && _Test is not null)
        {
            await _localStorageService.SetItemAsync<Test>("Test", _Test);
        }
    }

    public async void IsLogged()
    {
        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
        {
            LoggedIn = true;
        }
        else LoggedIn = false;
    }

    public static void Update() => NotifyStateChanged();

    private static void NotifyStateChanged() => OnChange?.Invoke();
}