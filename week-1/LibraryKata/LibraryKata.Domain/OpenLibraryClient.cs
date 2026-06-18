using System.Text.Json;
using Serilog;

namespace LibraryKata.Domain;

public class OpenLibraryClient
{
    private static readonly HttpClient client = new();

    public async Task<LibraryItem?> FetchByIsbnAsync(string isbn)
    {
        string url = $"https://openlibrary.org/search.json?q=isbn:{isbn}&fields=title,author_name&limit=1";

        try
        {
            string jsonResponse = await client.GetStringAsync(url);

            return Parse(jsonResponse);
        }
        catch (HttpRequestException ex)
        {
            Log.Warning("Network fetch failed for {isbn}: {Message}", isbn, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Log.Warning("FetchByIsbnAsync failed: {Message}", ex.Message);
            return null;
        }
    }

    public static LibraryItem? Parse(string json)
    {
        Dictionary<string, JsonElement>? resp = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        if (resp is null || !resp.TryGetValue("docs", out JsonElement docs) || docs.GetArrayLength() == 0)
        {
            return null;
        }

        JsonElement foundbook = docs[0];

        string title = foundbook.GetProperty("title").GetString() ?? "Untitled";

        string author = "Unknown";

        if (foundbook.TryGetProperty("author_name", out JsonElement authors) && authors.GetArrayLength() > 0)
        {
            author = authors[0].GetString() ?? "Unknown";
        }

        return LibraryItemFactory.Create(ItemKind.Book, title, author);

    }
}