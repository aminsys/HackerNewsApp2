using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApp2.Model
{
    public class HNAlgoliaModel
    {
        private string text;
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Author { get; set; }
        public int? Points { get; set; }
        public bool Deleted { get; set; }
        public bool Dead { get; set; }
        public DateTime Created_At { get; set; }
        public string ItemAge
        {
            get
            {
                return Util.Util.GetTimeAgo(Created_At);
            }
        }
        public int Parent { get; set; }
        public int Poll { get; set; }
        public int Parts { get; set; }
        public string Url { get; set; }
        public HNAlgoliaModel[] Children { get; set; }
        public int Descendants { 
            get 
            { 
                return Children.Length;  
            } 
            set => Descendants = value;
        }

        public string Text
        {
            get
            {
                HtmlDocument doc = new HtmlDocument();
                try
                {
                    text = WebUtility.HtmlDecode(text);
                    doc.LoadHtml(text);
                    var root = doc.DocumentNode;
                    var sb = new StringBuilder();
                    foreach (var node in root.DescendantsAndSelf())
                    {
                        if (!node.HasChildNodes)
                        {
                            string text = node.InnerHtml;
                            if (!string.IsNullOrEmpty(text))
                            {
                                sb.AppendLine(text.Trim());
                            }
                        }
                    }
                    text = sb.ToString();

                    return text;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error while converting text property:\n" + e.Message);
                    return null;
                }
            }
            set => text = value;
        }
    }
}
