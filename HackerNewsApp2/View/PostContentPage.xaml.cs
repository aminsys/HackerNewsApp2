using HackerNewsApp2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HackerNewsApp2.View;

public partial class PostContentPage : ContentPage
{

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


    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChildrenItems = FetchPostAsync();
        PostCollectionView.ItemsSource = ChildrenItems;
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private ObservableCollection<HNAlgoliaModel> FetchPostAsync()
    {
        ObservableCollection<HNAlgoliaModel> ChildrenContent = new ObservableCollection<HNAlgoliaModel>();
        try
        {
            this.Title = postObject.Title;
            PostTitle.Text = postObject.Title;
            PostText.Text = postObject.Text;

            foreach (HNAlgoliaModel child in postObject.Children)
            {
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

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (!string.IsNullOrEmpty(postObject.Url) && postObject.Text.Length - 1 == postObject.Url.Length)
        {
            await Navigation.PushAsync(new WebPage(postObject.Url));
        }
    }
}