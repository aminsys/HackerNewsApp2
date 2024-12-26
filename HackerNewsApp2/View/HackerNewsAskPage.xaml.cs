using HackerNewsApp2.API;
using HackerNewsApp2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class HackerNewsAskPage : ContentPage
{

    private const string AskStories = "https://hacker-news.firebaseio.com/v0/askstories.json";
    private const string ApiUrl = "https://hn.algolia.com/api/v1/items/";
    private bool isLoading = false;
    private List<string> askItemsTrimmed;
    private ObservableCollection<HNAlgoliaModel> newsItems;
    private APICaller apiCaller;

    public HackerNewsAskPage()
	{
		InitializeComponent();
        apiCaller = new APICaller();

        NewsCollectionView.RemainingItemsThreshold = 2;
        NewsCollectionView.RemainingItemsThresholdReached += NewsCollectionView_RemainingItemsThresholdReached;
    }

    private async void NewsCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (!isLoading && askItemsTrimmed?.Count > 0)
        {
            isLoading = true;
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            try
            {
                var moreNewsItems = await apiCaller.FetchNewsAsync(ApiUrl, askItemsTrimmed);
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

        askItemsTrimmed = await apiCaller.GetPostItems(AskStories);
        newsItems = await apiCaller.FetchNewsAsync(ApiUrl, askItemsTrimmed);
        NewsCollectionView.ItemsSource = newsItems;
    }

    private async void OpenSelectedPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HNAlgoliaModel post = (e.CurrentSelection.FirstOrDefault() as HNAlgoliaModel);
        await Navigation.PushAsync(new PostContentPage(post));
    }
}