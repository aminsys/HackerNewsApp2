using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using HackerNewsApp2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class PostContentPage : ContentPage
{

    private const string apiUrl = "https://hacker-news.firebaseio.com/v0/item/";
    private readonly HackerNewsPostModel postObject;
    private bool isLoading = false;
    private ObservableCollection<HackerNewsPostModel> kidsItems;

    public PostContentPage()
    {
        InitializeComponent();
    }

    public PostContentPage(HackerNewsPostModel post)
    {
        InitializeComponent();
        postObject = post;


        PostCollectionView.RemainingItemsThreshold = 2;
        PostCollectionView.RemainingItemsThresholdReached += PostCollectionView_RemainingItemsThresholdReached;
    }

    private async void PostCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (!isLoading && postObject.Kids.Length > 0)
        {
            isLoading = true;
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            try
            {
                Debug.WriteLine("Loading more comments. Number of Kids left: " + postObject.Kids.Length);
                var moreNewsItems = await FetchPostAsync();
                foreach (var item in moreNewsItems)
                {
                    kidsItems.Add(item);
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
        kidsItems = await FetchPostAsync();
        PostCollectionView.ItemsSource = kidsItems;
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private async Task<ObservableCollection<HackerNewsPostModel>> FetchPostAsync()
    {
        ObservableCollection<HackerNewsPostModel>  kidsContent = new ObservableCollection<HackerNewsPostModel>();
        try
        {
            this.Title = postObject.Title;
            PostTitle.Text = postObject.Title;
            PostText.Text = postObject.Text;
            using (HttpClient client = new HttpClient())
            {
                if (postObject.Kids != null)
                {
                    for(int i = 0; i < postObject.Kids.Length; i++)
                    {
                        Debug.WriteLine("postObject.Kids[i]: " + postObject.Kids[i]);
                        HttpResponseMessage response = await client.GetAsync(apiUrl + postObject.Kids[i] + ".json");
                        var kidItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                        if (kidItem != null && !kidItem.Dead)
                        {
                            kidsContent.Add(kidItem);
                        }
                        if(i > 9)
                        {
                            break;
                        }
                    }
                }
            }

            var tempList = postObject.Kids.ToList();
            if(tempList.Count < 9)
            {
                tempList.RemoveRange(0, tempList.Count());
            }
            else
            {
                tempList.RemoveRange(0, 9);

            }
            postObject.Kids = tempList.ToArray();
            return kidsContent;
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new ObservableCollection<HackerNewsPostModel>();
        }
    }

    private async void PostCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HackerNewsPostModel post = (e.CurrentSelection.FirstOrDefault() as HackerNewsPostModel);
        if(post.Url != null)
        {
            await Navigation.PushAsync(new WebPage(post.Url));
        }
        
        else
        {
            string text = "This post is not URL based.";
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = Toast.Make(text, duration, fontSize);
            await toast.Show();
        }
    }
}