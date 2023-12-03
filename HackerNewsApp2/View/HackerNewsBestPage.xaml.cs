using HackerNewsApp2.API;
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
    private APICaller apiCaller;

    public HackerNewsBestPage()
	{
		InitializeComponent();
        apiCaller = new APICaller();

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
                var moreNewsItems = await apiCaller.FetchNewsAsync(ApiUrl, bestItemsTrimmed);
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

        bestItemsTrimmed = await apiCaller.GetPostItems(BestStories);
        newsItems = await apiCaller.FetchNewsAsync(ApiUrl, bestItemsTrimmed);
        NewsCollectionView.ItemsSource = newsItems;

        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    

    private async void OpenSelectedPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HackerNewsPostModel post = (e.CurrentSelection.FirstOrDefault() as HackerNewsPostModel);
        await Navigation.PushAsync(new PostContentPage(post));        
    }
}