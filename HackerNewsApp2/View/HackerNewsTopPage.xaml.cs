using HackerNewsApp2.API;
using HackerNewsApp2.Model;
using System.Collections.ObjectModel;

namespace HackerNewsApp2.View;

public partial class HackerNewsTopPage : ContentPage
{
    private const string TopStories = "https://hacker-news.firebaseio.com/v0/topstories.json";
    private const string ApiUrl = "https://hn.algolia.com/api/v1/items/";
    private bool isLoading = false;
    private List<string> topItemsTrimmed;
    private ObservableCollection<HNAlgoliaModel> newsItems;
    private APICaller apiCaller;

    public HackerNewsTopPage()
    {
        InitializeComponent();
        apiCaller = new APICaller();

        NewsCollectionView.RemainingItemsThreshold = 2;
        NewsCollectionView.RemainingItemsThresholdReached += NewsCollectionView_RemainingItemsThresholdReached;
    }

    private async void NewsCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (!isLoading && topItemsTrimmed?.Count > 0)
        {
            isLoading = true;
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            try
            {
                var moreNewsItems = await apiCaller.FetchNewsAsync(ApiUrl, topItemsTrimmed);
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

        if (topItemsTrimmed is null)
        {
            topItemsTrimmed = await apiCaller.GetPostItems(TopStories);
            newsItems = await apiCaller.FetchNewsAsync(ApiUrl, topItemsTrimmed);
            NewsCollectionView.ItemsSource = newsItems;
        }

        NewsCollectionView.RemainingItemsThresholdReached += NewsCollectionView_RemainingItemsThresholdReached;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        NewsCollectionView.RemainingItemsThresholdReached -= NewsCollectionView_RemainingItemsThresholdReached;
    }

    private async void OpenSelectedPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HNAlgoliaModel post = (e.CurrentSelection.FirstOrDefault() as HNAlgoliaModel);
        await Navigation.PushAsync(new PostContentPage(post));
    }

    protected override bool OnBackButtonPressed()
    {
        Navigation.PopAsync();
        return base.OnBackButtonPressed();
    }
}