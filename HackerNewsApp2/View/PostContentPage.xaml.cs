using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using HackerNewsApp2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HackerNewsApp2.View;

public partial class PostContentPage : ContentPage
{

    private const string apiUrl = "https://hn.algolia.com/api/v1/items/";
    private readonly HNAlgoliaModel postObject;
    private ObservableCollection<HNAlgoliaModel> ChildrenItems;

    public PostContentPage()
    {
        InitializeComponent();
    }

    public PostContentPage(HNAlgoliaModel post)
    {
        InitializeComponent();
        postObject = post;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ChildrenItems = await FetchPostAsync();
        PostCollectionView.ItemsSource = ChildrenItems;
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private async Task<ObservableCollection<HNAlgoliaModel>> FetchPostAsync()
    {
        ObservableCollection<HNAlgoliaModel>  ChildrenContent = new ObservableCollection<HNAlgoliaModel>();
        try
        {
            this.Title = postObject.Title;
            PostTitle.Text = postObject.Title;
            PostText.Text = postObject.Text;

            foreach (HNAlgoliaModel child in postObject.Children)
            {
                Debug.WriteLine("Comment Author: " + child.Author);
                Debug.WriteLine("Comment Descendants: " + child.Descendants);
                ChildrenContent.Add(child);
            }

            return ChildrenContent;
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new ObservableCollection<HNAlgoliaModel>();
        }
    }

    private async void PostCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        HNAlgoliaModel post = (e.CurrentSelection.FirstOrDefault() as HNAlgoliaModel);
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