using HackerNewsApp2.Model;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class HackerNewsAskPage : ContentPage
{

    private const string AskStories = "https://hacker-news.firebaseio.com/v0/askstories.json";
    private const string ApiUrl = "https://hacker-news.firebaseio.com/v0/item/";
    private int postCounter = 0;
    private List<string> askItemsTrimmed;
    private ObservableCollection<HackerNewsPostModel> newsItems;

    public HackerNewsAskPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await GetPostItems();
        postCounter = 0;

        newsItems = await FetchNewsAsync();
        NewsCollectionView.ItemsSource = newsItems;
        //loadingIndicator.IsRunning = false;
        //loadingIndicator.IsVisible = false;
    }

    /*private async void OnFetchMoreNewsClicked(object sender, EventArgs e)
    {
        newsItems = await FetchNewsAsync();
        NewsCollectionView.ItemsSource = newsItems;
    }*/

    private async Task<ObservableCollection<HackerNewsPostModel>> FetchNewsAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                ObservableCollection<HackerNewsPostModel> newsItems = new ObservableCollection<HackerNewsPostModel>();

                //foreach (var item in askItemsTrimmed)
                for(int i = postCounter; i < 9; i++)
                {
                    HttpResponseMessage response = await client.GetAsync(ApiUrl + askItemsTrimmed[i] + ".json");
                    var jsonItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                    if (string.IsNullOrEmpty(jsonItem.Text))
                    {
                        jsonItem.Text = jsonItem.Url;
                    }
                    
                    if(!jsonItem.Deleted){
                        newsItems.Add(jsonItem);
                        postCounter++;
                    }
                }
                Debug.WriteLine("Number of posts BEFORE deletíng: " + askItemsTrimmed.Count());
                askItemsTrimmed.RemoveRange(0, postCounter);
                Debug.WriteLine("Number of posts AFTER deletíng: " + askItemsTrimmed.Count());
                postCounter = 0;
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

    private async void NewsCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        var moreNewsItems = await FetchNewsAsync();
        foreach (var item in moreNewsItems)
        {
            newsItems.Add(item);
            Debug.WriteLine("#### Should have gotten new posts now ####");
        }
    }
}