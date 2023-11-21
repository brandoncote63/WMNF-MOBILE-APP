using System.Collections.Generic;

[System.Serializable]
public class RSSFeedItem
{
    public string title;
    public string link;
    public string description;
    public string start;
    public string creator;
    public List<string> category;
    public string guid;
    public string comments;
    public string endTime;
}

[System.Serializable]
public class RSSFeed
{
    public string title;
    public string link;
    public string description;
    public string language;
    public string lastBuildDate;
    public string pubDate;
    public List<RSSFeedItem> item;
}
