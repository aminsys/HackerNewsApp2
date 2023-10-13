using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HackerNewsApp2.Model
{
    /*
        id 	            The item's unique id.
        deleted 	    true if the item is deleted.
        type 	        The type of item. One of "job", "story", "comment", "poll", or "pollopt".
        by 	            The username of the item's author.
        time 	        Creation date of the item, in Unix Time.
        text 	        The comment, story or poll text. HTML.
        dead 	        true if the item is dead.
        parent 	        The comment's parent: either another comment or the relevant story.
        poll 	        The pollopt's associated poll.
        kids 	        The ids of the item's comments, in ranked display order.
        url 	        The URL of the story.
        score 	        The story's score, or the votes for a pollopt.
        title 	        The title of the story, poll or job. HTML.
        parts 	        A list of related pollopts, in display order.
        descendants 	In the case of stories or polls, the total comment count.
     */
    public class HackerNewsPostModel
    {
        private string text;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string By { get; set; }
        public int Descendants { get; set; }
        public int Score { get; set; }
        public bool Deleted { get; set; }
        public bool Dead { get; set; }
        public long Time { get; set; }
        public DateTime DateTime
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(Time).LocalDateTime;
            }
        }
        public int Parent { get; set; }
        public int Poll { get; set; }
        public int Parts { get; set; }
        public string Url { get; set; }
        public int[] Kids { get; set; }
        public string Text
        {
            get
            {
                try
                {
                    text = WebUtility.HtmlDecode(text);
                    return Regex.Replace(text, @"<p>", Environment.NewLine);
                }
                catch { return null; }                
            }
            set => text = value;
        }
    }
}
