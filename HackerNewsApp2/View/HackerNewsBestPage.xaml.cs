using HackerNewsApp2.API;
using HackerNewsApp2.Model;
using System.Collections.ObjectModel;

namespace HackerNewsApp2.View;

public partial class HackerNewsBestPage : ContentPage
{
    private const string BestStories = "https://hacker-news.firebaseio.com/v0/beststories.json";
    private const string ApiUrl = "https://hn.algolia.com/api/v1/items/";
    private ObservableCollection<HNAlgoliaModel> newsItems;
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
        if (!isLoading && bestItemsTrimmed?.Count > 0)
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

        if (bestItemsTrimmed is null)
        {
            bestItemsTrimmed = await apiCaller.GetPostItems(BestStories);
            newsItems = await apiCaller.FetchNewsAsync(ApiUrl, bestItemsTrimmed);
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