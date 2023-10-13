namespace HackerNewsApp2.View;

public partial class WebPage : ContentPage
{
    public WebPage()
    {
        InitializeComponent();
    }
    public WebPage(string url)
    {
        InitializeComponent();
        WebViewPage.Source = url;
    }
}