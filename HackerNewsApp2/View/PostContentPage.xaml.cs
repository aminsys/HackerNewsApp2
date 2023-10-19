using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using HackerNewsApp2.Model;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class PostContentPage : ContentPage
{

    private const string apiUrl = "https://hacker-news.firebaseio.com/v0/item/";
    private readonly HackerNewsPostModel postObject;

    public PostContentPage()
    {
        InitializeComponent();
    }

    public PostContentPage(HackerNewsPostModel post)
    {
        InitializeComponent();
        this.postObject = post;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        List<HackerNewsPostModel> kidsItems = await FetchPostAsync();
        PostCollectionView.ItemsSource = kidsItems;
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private async Task<List<HackerNewsPostModel>> FetchPostAsync()
    {
        List<HackerNewsPostModel> kidsContent = new List<HackerNewsPostModel>();
        try
        {
            kidsContent.Add(this.postObject); // to get full text of post
            using (HttpClient client = new HttpClient())
            {
                if (postObject.Kids != null)
                {
                    foreach (var kid in postObject.Kids)
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl + kid + ".json");
                        var kidItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                        if (kidItem != null && !kidItem.Dead)
                        {
                            kidsContent.Add(kidItem);
                        }
                    }
                }
            }
            return kidsContent;
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new List<HackerNewsPostModel>();
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