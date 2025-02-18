using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Net.Http.Json;
using Blazor_Client.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Blazor_Client.Services;

public interface IApiService {
    void Login(string name, string password);
    void Register(string name, string password);
}

public class ApiService
{
    private string _url = "http://localhost:5089";
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    private IHttpClientFactory _httpClientFactory;

    private static LoginRequest user = new(token: "");

    public ApiService(ILocalStorageService storageService, IHttpClientFactory httpClientFactory){
        _localStorageService = storageService;
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient httpClient = new();

    public async void Login(string name, string password) 
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        await _localStorageService.RemoveItemAsync("User");

        var uri = new Uri($"{_url}/auth/login/{name}/{password}");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        using var response = await httpClient.SendAsync(request);

        LoginRequest? token = await response.Content.ReadFromJsonAsync<LoginRequest>();

        await _localStorageService.SetItemAsync("User", token);

    }

    public async void Register(UserRequest user) 
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        var uri = new Uri($"{_url}/auth/register");

        var request = new HttpRequestMessage(HttpMethod.Post, uri);

        request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request);
    }

    public async Task<Comment[]> GetComments(long id) 
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        var user = await _localStorageService.GetItemAsync<LoginRequest>("User");

        var uri = new Uri($"{_url}/api/Comments/{id}");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer" + user.token);

        using var response = await httpClient.SendAsync(request);

        Comment[] comments = await response.Content.ReadFromJsonAsync<Comment[]>();

        return comments;
    }
    public async Task<List<User>> GetUsers() 
    {
        HttpClient httpClient = new();
        var client = _httpClientFactory.CreateClient();

        if (await _localStorageService.ContainKeyAsync("User")){
            var user = await _localStorageService.GetItemAsync<LoginRequest>("User");

            var uri = new Uri($"{_url}/api/users/");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.token);

            using var response = await httpClient.SendAsync(request);

            List<User> users = await response.Content.ReadFromJsonAsync<List<User>>();

            return users;
        }
        else return new List<User>();
    }

    public async Task<bool> IsTokenExist() 
    {
        var user = await _localStorageService.GetItemAsync<LoginRequest>("User");

        return user is null;
    }
}