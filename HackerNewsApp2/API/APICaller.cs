using HackerNewsApp2.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApp2.API
{
    public class APICaller
    {
        internal async Task<ObservableCollection<HackerNewsPostModel>> FetchNewsAsync(string ApiUrl,List<string> itemsTrimmed)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ObservableCollection<HackerNewsPostModel> newsItems = new ObservableCollection<HackerNewsPostModel>();

                    for (int i = 0; i < 9; i++)
                    {
                        HttpResponseMessage response = await client.GetAsync(ApiUrl + itemsTrimmed[i] + ".json");
                        var jsonItem = await response.Content.ReadFromJsonAsync<HackerNewsPostModel>();
                        if (string.IsNullOrEmpty(jsonItem.Text))
                        {
                            jsonItem.Text = jsonItem.Url;
                        }
                        if (!jsonItem.Deleted)
                        {
                            newsItems.Add(jsonItem);
                        }
                    }

                    if (itemsTrimmed.Count < 9)
                    {
                        itemsTrimmed.RemoveRange(0, itemsTrimmed.Count);
                    }
                    else
                    {
                        itemsTrimmed.RemoveRange(0, 9);
                    }

                    return newsItems;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return new ObservableCollection<HackerNewsPostModel>();
            }
        }

        internal async Task<List<string>> GetPostItems(string stories)
        {
            List<string> itemsTrimmed = new List<string>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage bestPostIds = await client.GetAsync(stories);
                    string askItems = await bestPostIds.Content.ReadAsStringAsync();
                    itemsTrimmed = askItems.TrimStart('[').TrimEnd(']').Split(',').ToList();
                }
                return itemsTrimmed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
