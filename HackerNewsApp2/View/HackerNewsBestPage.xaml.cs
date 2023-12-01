using HackerNewsApp2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class HackerNewsBestPage : ContentPage
{
    private const string BestStories = "https://hacker-news.firebaseio.com/v0/beststories.json";
    private const string ApiUrl = "https://hacker-news.firebaseio.com/v0/item/";
    private ObservableCollection<HackerNewsPostModel> newsItems;
    private bool isLoading = false;
    private List<string> bestItemsTrimmed;

    public HackerNewsBestPage()
	{
		InitializeComponent();

        NewsCollectionView.RemainingItemsThreshold = 2;
        NewsCollectionView.RemainingItemsThresholdReached += NewsCollectionView_RemainingItemsThresholdReached;

    }

    private async void NewsCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (!isLoading && bestItemsTrimmed.Count > 0)
        {
            isLoading = true;
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            try
            {
                var moreNewsItems = await FetchNewsAsync();
                foreach (var item in moreNewsItems)
                {
                    newsItems.Add(item);
                }
            }
            finally
            {
                isLoading = false;
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await GetPostItems();
        newsItems = await FetchNewsAsync();
        NewsCollectionView.ItemsSource = newsItems;

        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private async Task<ObservableCollection<HackerNewsPostModel>> FetchNewsAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                ObservableCollection<HackerNewsPostModel> newsItems = new ObservableCollection<HackerNewsPostModel>();

                for (int i = 0; i < 9; i++)
                {
                    HttpResponseMessage response = await client.GetAsync(ApiUrl + bestItemsTrimmed[i] + ".json");
                    var jsonItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                    if (string.IsNullOrEmpty(jsonItem.Text))
                    {
                        jsonItem.Text = jsonItem.Url;
                    }
                    if (!jsonItem.Deleted)
                    {
                        newsItems.Add(jsonItem);
                    }
                }

                if (bestItemsTrimmed.Count < 9)
                {
                    bestItemsTrimmed.RemoveRange(0, bestItemsTrimmed.Count);
                }
                else
                {
                    bestItemsTrimmed.RemoveRange(0, 9);
                }

                return newsItems;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new ObservableCollection<HackerNewsPostModel>();
        }
    }

    private async Task GetPostItems()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage bestPostIds = await client.GetAsync(BestStories);
                string askItems = await bestPostIds.Content.ReadAsStringAsync();
                bestItemsTrimmed = askItems.TrimStart('[').TrimEnd(']').Split(',').ToList();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    private async void OpenSelectedPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HackerNewsPostModel post = (e.CurrentSelection.FirstOrDefault() as HackerNewsPostModel);
        await Navigation.PushAsync(new PostContentPage(post));        
    }
}