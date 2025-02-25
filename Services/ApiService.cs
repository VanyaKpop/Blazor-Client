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
    private string _url = "http://localhost:5089";
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    private IHttpClientFactory _httpClientFactory;

    public ApiService(ILocalStorageService storageService, IHttpClientFactory httpClientFactory)
    {
        _localStorageService = storageService;
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient httpClient = new();

    public async Task<bool> Login(string username, string password)
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        var uri = new Uri($"{_url}/auth/login/{username}/{password}");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        try
        {
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
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        var uri = new Uri($"{_url}/auth/register");

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
        return false;
    }

    public async Task<List<Comment>> GetComments(long id)
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");

            var uri = new Uri($"{_url}/api/users/");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            using var response = await httpClient.SendAsync(request);

            List<Comment> comments = await response.Content.ReadFromJsonAsync<List<Comment>>();

            return comments;
        }
        else return new List<Comment>();
    }
    public async Task<List<User>> GetUsers()
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");

            var uri = new Uri($"{_url}/api/users/");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            using var response = await httpClient.SendAsync(request);

            List<User> users = await response.Content.ReadFromJsonAsync<List<User>>();

            return users;
        }
        else return new List<User>();
    }

    public async Task<List<Test?>> GetTestAsync()
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");

            var uri = new Uri($"{_url}/api/gettests/");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            using var response = await httpClient.SendAsync(request);

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
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");
            var user = await _localStorageService.GetItemAsync<UserRequest>("Token");

            var uri = new Uri($"{_url}/api/gettests/{user.Username}");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            using var response = await httpClient.SendAsync(request);

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
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
        {
            var token = await _localStorageService.GetItemAsync<TokenRequest>("Token");
            UserRequest? user = await _localStorageService.GetItemAsync<UserRequest>("User");

            var uri = new Uri($"{_url}/api/posttest/");

            TestRequest test = new TestRequest { Name = testName, Author = user.Username, Questions= questions};

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.token);

            request.Content = new StringContent(JsonSerializer.Serialize(test), Encoding.UTF8, "application/json");

            using var response = await httpClient.SendAsync(request);
        }
    }

    public async void Logout()
    {
        if (await _localStorageService.ContainKeyAsync("Token") & await _localStorageService.ContainKeyAsync("User"))
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