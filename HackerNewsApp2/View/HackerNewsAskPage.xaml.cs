using HackerNewsApp2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class HackerNewsAskPage : ContentPage
{

    private const string AskStories = "https://hacker-news.firebaseio.com/v0/askstories.json";
    private const string ApiUrl = "https://hacker-news.firebaseio.com/v0/item/";
    private bool isLoading = false;
    private List<string> askItemsTrimmed;
    private ObservableCollection<HackerNewsPostModel> newsItems;

    public HackerNewsAskPage()
	{
		InitializeComponent();

        NewsCollectionView.RemainingItemsThreshold = 2;
        NewsCollectionView.RemainingItemsThresholdReached += NewsCollectionView_RemainingItemsThresholdReached;
    }

    private async void NewsCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (!isLoading && askItemsTrimmed.Count > 0)
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
    }

    private async Task<ObservableCollection<HackerNewsPostModel>> FetchNewsAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                ObservableCollection<HackerNewsPostModel> newsItems = new ObservableCollection<HackerNewsPostModel>();

                for(int i = 0; i < 9; i++)
                {
                    HttpResponseMessage response = await client.GetAsync(ApiUrl + askItemsTrimmed[i] + ".json");
                    var jsonItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                    if (string.IsNullOrEmpty(jsonItem.Text))
                    {
                        jsonItem.Text = jsonItem.Url;
                    }
                    
                    if(!jsonItem.Deleted){
                        newsItems.Add(jsonItem);
                    }
                }

                if(askItemsTrimmed.Count < 9)
                {
                    askItemsTrimmed.RemoveRange(0, askItemsTrimmed.Count);
                }
                else
                {
                    askItemsTrimmed.RemoveRange(0, 9);
                }

                return newsItems;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new ObservableCollection<HackerNewsPostModel>();
        }
    }

    private async Task GetPostItems()
    {
        try
        {
            using(HttpClient client = new HttpClient())
            {
                HttpResponseMessage askPostIds = await client.GetAsync(AskStories);
                string askItems = await askPostIds.Content.ReadAsStringAsync();
                askItemsTrimmed = askItems.TrimStart('[').TrimEnd(']').Split(',').ToList();
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