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

    /// <summary>
    /// Fetch posts and their main comments, and sub comments
    /// </summary>
    /// <returns>An observable list containing posts</returns>
    private ObservableCollection<HNAlgoliaModel> FetchPostAsync()
    {
        var childrenContent = new ObservableCollection<HNAlgoliaModel>();
        try
        {
            this.Title = postObject.Title;
            PostTitle.Text = postObject.Title;
            PostText.Text = postObject.Text;

            foreach (HNAlgoliaModel child in postObject.Children)
            {
                childrenContent.Add(child);
                if(child.Children != null)
                {
                    var subComments = FetchSubComments(child);
                    foreach(var s in subComments)
                    {
                        childrenContent.Add(s);
                    }
                }
            }

            return childrenContent;
        }

        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return new ObservableCollection<HNAlgoliaModel>();
        }
    }

    /// <summary>
    /// Get the sub comments that belong to main comments, if they exist
    /// </summary>
    /// <param name="comment">The first sub comment in the main comment</param>
    /// <returns>A list of sub comments</returns>
    private ObservableCollection<HNAlgoliaModel> FetchSubComments(HNAlgoliaModel comment)
    {
        var subComments = new ObservableCollection<HNAlgoliaModel>();
        try
        {
            foreach(var subComment in comment.Children)
            {
                subComments.Add(subComment);
                if(subComment.Children != null)
                {
                    FetchSubComments(subComment);
                }

                else
                {
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return subComments;
        }

        return subComments;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (!string.IsNullOrEmpty(postObject.Url) && postObject.Text.Length - 1 == postObject.Url.Length)
        {
            await Navigation.PushAsync(new WebPage(postObject.Url));
        }
    }
}