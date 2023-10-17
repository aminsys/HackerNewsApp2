using HackerNewsApp2.Model;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class HackerNewsAskPage : ContentPage
{

    private const string AskStories = "https://hacker-news.firebaseio.com/v0/askstories.json";
    private const string ApiUrl = "https://hacker-news.firebaseio.com/v0/item/";

    public HackerNewsAskPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        List<HackerNewsPostModel> newsItems = await FetchNewsAsync();
        NewsCollectionView.ItemsSource = newsItems;
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private async void OnFetchNewsClicked(object sender, EventArgs e)
    {
        List<HackerNewsPostModel> newsItems = await FetchNewsAsync();
        NewsCollectionView.ItemsSource = newsItems;
    }

    private async Task<List<HackerNewsPostModel>> FetchNewsAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var c = 0;
                List<HackerNewsPostModel> newsItems = new List<HackerNewsPostModel>();
                HttpResponseMessage askPostIds = await client.GetAsync(AskStories);

                string askItems = await askPostIds.Content.ReadAsStringAsync();
                var askItemsTrimmed = askItems.TrimStart('[').TrimEnd(']').Split(',');
                foreach (var item in askItemsTrimmed)
                {
                    HttpResponseMessage response = await client.GetAsync(ApiUrl + item + ".json");
                    var jsonItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                    if (string.IsNullOrEmpty(jsonItem.Text))
                    {
                        jsonItem.Text = jsonItem.Url;
                    }
                    
                    if(!jsonItem.Deleted){
                        newsItems.Add(jsonItem);
                        c++;
                    }

                    if (c > 5)
                    {
                        break;
                    }
                }
                return newsItems;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new List<HackerNewsPostModel>();
        }
    }

    private async void OpenSelectedPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine("Ask HN post clicked");
        HackerNewsPostModel post = (e.CurrentSelection.FirstOrDefault() as HackerNewsPostModel);
        await Navigation.PushAsync(new PostContentPage(post));
    }
}