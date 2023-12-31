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
        private int numberOfPostsToLoad = 6;
        internal async Task<ObservableCollection<HNAlgoliaModel>> FetchNewsAsync(string ApiUrl,List<string> itemsTrimmed)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ObservableCollection<HNAlgoliaModel> newsItems = new ObservableCollection<HNAlgoliaModel>();

                    for (int i = 0; i < numberOfPostsToLoad; i++)
                    {
                        HttpResponseMessage response = await client.GetAsync(ApiUrl + itemsTrimmed[i]);
                        var jsonItem = await response.Content.ReadFromJsonAsync<HNAlgoliaModel>();
                        if (string.IsNullOrEmpty(jsonItem.Text))
                        {
                            jsonItem.Text = jsonItem.Url;
                        }
                        if (!jsonItem.Deleted)
                        {
                            newsItems.Add(jsonItem);
                        }
                    }

                    if (itemsTrimmed.Count < numberOfPostsToLoad)
                    {
                        itemsTrimmed.RemoveRange(0, itemsTrimmed.Count);
                    }
                    else
                    {
                        itemsTrimmed.RemoveRange(0, numberOfPostsToLoad);
                    }

                    return newsItems;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return new ObservableCollection<HNAlgoliaModel>();
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
