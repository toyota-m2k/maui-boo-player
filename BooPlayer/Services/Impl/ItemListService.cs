using BooPlayer.Models;
using BooPlayer.Utils;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BooPlayer.Services.Impl;

internal class ItemListService : IItemListService {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPageService _pageService;


    private HttpClient httpClient => _httpClientFactory.CreateClient();

    public ItemListService(IHttpClientFactory httpClientFactory, IPageService pageService) {
        _httpClientFactory = httpClientFactory;
        _pageService = pageService;
    }

    public async Task<ItemList> GetItemListAsync(HostEntry host) {
        while (true) {
            var itemList = await GetItemListCoreAsync(host);
            if (itemList != null) {
                return itemList;
            }
        }
    }

    private async Task<ItemList?> GetItemListCoreAsync(HostEntry host) {
        var url = host.AccessToken.IsEmpty() ? $"http://{host.Address}/list?type=all" : $"http://{host.Address}/list?type=all&auth={host.AccessToken}";
        using (var response = await httpClient.GetAsync(url)) {
            if (response?.IsSuccessStatusCode == true) {
                var json = await response.Content.ReadAsStringAsync();
                var itemList = JsonConvert.DeserializeObject<ItemList?>(json);
                if (itemList == null) {
                    throw new InvalidDataException("bad json");
                }
                return itemList;
            }
            else if (response?.StatusCode == HttpStatusCode.Unauthorized) {
                if (await Authenticate(host, response)) {
                    return null;
                }
                else {
                    throw new HttpRequestException("unauthorized");
                }
            }
            else {
                throw new HttpRequestException(response?.StatusCode.ToString()??"no response");
            }
        }
    }

    private async Task<string> GetChallenge(HttpResponseMessage response) {
        var json = await response.Content.ReadAsStringAsync();
        var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        if (dic == null) {
            throw new InvalidDataException("bad json");
        }
        return dic["challenge"];
    }


    private async Task<bool> Authenticate(HostEntry host, HttpResponseMessage response) {
        var challenge = await GetChallenge(response);
        if (challenge.IsEmpty()) {
            throw new InvalidDataException("no challenge");
        }
        host.Challenge = challenge;
        host.AccessToken = null;
        while(true) {
            var password= await GetPassword();
            if(password== null) {
                // cancelled
                return false;
            }
            var accessToken = await AuthWithPassword(host, challenge, password);
            if(accessToken != null) {
                host.AccessToken = accessToken;
                return true;
            }
        }
    }

    private async Task<string?> GetPassword() {
        return await _pageService.ShowPasswordDialog();
    }

    const string PWD_SEED = "y6c46S/PBqd1zGFwghK2AFqvSDbdjl+YL/DKXgn/pkECj0x2fic5hxntizw5";

    private string GetPassPhrase(string challenge, string password) {
        var hashedPassword = HashBuilder.SHA256.Append(PWD_SEED).Append(password).Build().AsHexString;
        return HashBuilder.SHA256.Append(hashedPassword).Append(challenge).Build().AsBase64String;
    }

    private async Task<string?> AuthWithPassword(HostEntry host, string challenge, string password) {
        var url = $"http://{host.Address}/auth";
        var passPhrase = GetPassPhrase(challenge, password);
        var content = new StringContent(passPhrase, Encoding.UTF8, "text/plain");
        using (var response = await httpClient.PutAsync(url, content)) {
            if (response?.IsSuccessStatusCode==true) {
                var json = await response.Content.ReadAsStringAsync();
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (dic == null) {
                    throw new InvalidDataException("bad json");
                }
                var accessToken = dic["token"];
                if (accessToken.IsEmpty()) {
                    throw new InvalidDataException("no access_token");
                }
                return accessToken;
            } else {
                return null;
            }
        }
    }
}
