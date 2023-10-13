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
    }

    private async Task<List<HackerNewsPostModel>> FetchPostAsync()
    {
        Label TitleLabel;
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
                            TitleLabel = FindByName("TitleLabel") as Label;
                            if(TitleLabel != null)
                            {
                                TitleLabel.Text = "GFDGDGTRf";
                            }
                        }
                    }
                }

                //else
                //{
                //    string text = "There are no comments for this post... yet!";
                //    ToastDuration duration = ToastDuration.Short;
                //    double fontSize = 14;
                //    var toast = Toast.Make(text, duration, fontSize);
                //    await toast.Show();
                //}


            }
            return kidsContent;
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new List<HackerNewsPostModel>();
        }
    }
}