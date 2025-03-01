using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Net.Http.Json;
using Blazor_Client.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Blazor_Client.Services;

public interface IApiService
{
    void Login(string name, string password);
    void Register(string name, string password);
}

public class ApiService
{
    private string bebe = "http://172.16.221.14:85";
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    private IHttpClientFactory _httpClientFactory;

    public ApiService(ILocalStorageService storageService, IHttpClientFactory httpClientFactory)
    {
        _localStorageService = storageService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> Login(string username, string password)
    {
        var client = _httpClientFactory.CreateClient();

        var uri = new Uri($"{bebe}/auth/login/");

        var user = new UserRequest {Username = username, Password = password};

        var request = new HttpRequestMessage(HttpMethod.Post, uri);

        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            await _localStorageService.RemoveItemAsync("User");
            await _localStorageService.RemoveItemAsync("Token");
        }

        try
        {
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);

            TokenRequest? token = await response.Content.ReadFromJsonAsync<TokenRequest>();

            await _localStorageService.SetItemAsync("Token", token);
            await _localStorageService.SetItemAsync("User", new UserRequest { Username = username, Password = "password" });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Register(UserRequest user)
    {
        var client = _httpClientFactory.CreateClient();

        var uri = new Uri($"{bebe}/auth/register");

        var request = new HttpRequestMessage(HttpMethod.Post, uri);

        try
        {
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Test?>> GetTestAsync()
    {
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");

            var uri = new Uri($"{bebe}/api/gettests/");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            using var response = await client.SendAsync(request);

            try
            {
                var Tests = await response.Content.ReadFromJsonAsync<List<Test?>>();

                return Tests;
            }
            catch
            {
                return new List<Test?>();
            }
        }
        else return new List<Test?>();
    }

    public async Task<List<Test?>> GetUserTestsAsync()
    {
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");
            var user = await _localStorageService.GetItemAsync<UserRequest>("User");

            var uri = new Uri($"{bebe}/api/gettests/{user.Username}");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            using var response = await client.SendAsync(request);

            try
            {
                var Tests = await response.Content.ReadFromJsonAsync<List<Test?>>();

                return Tests;
            }
            catch
            {
                return new List<Test?>();
            }
        }
        else return new List<Test?>();
    }
    public async void PostTest(string testName, List<Question> questions)
    {
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");
            UserRequest? user = await _localStorageService.GetItemAsync<UserRequest>("User");

            var uri = new Uri($"{bebe}/api/posttest/");

            TestRequest test = new TestRequest { Name = testName, Author = user.Username, Questions= questions};

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            request.Content = new StringContent(JsonSerializer.Serialize(test), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
        }
    }

    public async void Logout()
    {
        if (await _localStorageService.ContainKeyAsync("Token") && await _localStorageService.ContainKeyAsync("User"))
        {
            await _localStorageService.RemoveItemAsync("User");
            await _localStorageService.RemoveItemAsync("Token");
        }
    }

    public async Task<bool> IsTokenDoesntExist()
    {
        TokenRequest? token = await _localStorageService.GetItemAsync<TokenRequest>("Token");

        if (token is null)
            return true;

        return false;
    }
}