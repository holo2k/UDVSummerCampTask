using System.Text.Json;
using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.Services
{
    public class VkService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;

        public VkService(HttpClient httpClient, IConfiguration config)
        {
            this.httpClient = httpClient;
            this.config = config;
        }

        public async Task<List<Post>> GetLastPostsAsync(string userInput)
        {
            int ownerId = await ResolveOwnerIdAsync(userInput);
            return await GetPostsByOwnerIdAsync(ownerId);
        }

        public async Task<int> ResolveOwnerIdAsync(string input)
        {
            var token = config["Vk:AccessToken"];

            if (int.TryParse(input, out int numericId))
            {
                return numericId;
            }

            var url = $"https://api.vk.com/method/utils.resolveScreenName?screen_name={input}&access_token={token}&v=5.199";

            var response = await httpClient.GetFromJsonAsync<JsonElement>(url);

            if (!response.TryGetProperty("response", out var data) || data.ValueKind != JsonValueKind.Object)
            {
                throw new Exception("VK API did not resolve the screen name (maybe it does not exist)");
            }

            var type = data.GetProperty("type").GetString();
            var objectId = data.GetProperty("object_id").GetInt32();

            return type switch
            {
                "user" => objectId,   // если это пользователь — возвращаем ID как есть
                "group" => -objectId, // если это группа — возвращаем отрицательный ID
                                      // (VK API использует отрицательные ID для групп)
                _ => throw new Exception($"Unsupported VK object type: {type}")
            };
        }

        public async Task<List<Post>> GetPostsByOwnerIdAsync(int ownerId)
        {
            var token = config["Vk:AccessToken"];
            var url = $"https://api.vk.com/method/wall.get?owner_id={ownerId}&count=5&access_token={token}&v=5.199";

            var json = await httpClient.GetFromJsonAsync<JsonElement>(url);

            if (json.TryGetProperty("error", out var error))
            {
                var msg = error.GetProperty("error_msg").GetString();
                throw new Exception($"VK API error: {msg}");
            }

            if (!json.TryGetProperty("response", out var response))
                throw new Exception("VK API returned no 'response' field");

            if (!response.TryGetProperty("items", out var items))
                throw new Exception("VK API response has no 'items' field");

            var result = new List<Post>();
            foreach (var item in items.EnumerateArray())
            {
                if (item.TryGetProperty("text", out var text))
                {
                    var post = text.GetString();
                    if (!string.IsNullOrWhiteSpace(post))
                        result.Add(
                            new Post(ownerId, post)  
                        );
                }
            }

            return result;
        }
    }
}
