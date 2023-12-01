using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEditor;

public static class APIManager_Events
{
    [Obsolete]
    public static RSSFeed GetEventsFromRSS(string feedUrl)
    {
        // Fetch the JSON data using Unity's UnityWebRequest
        UnityWebRequest www = UnityWebRequest.Get(feedUrl);
        www.SendWebRequest();

        while (!www.isDone) { }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
            return null;
        }

        string json = www.downloadHandler.text;
        
        json = json.Replace("image-thumb", "imagethumb");

        Debug.Log("API Response JSON: " + json); // Debugging line

        // Parse the JSON string using SimpleJSON
        JSONNode rootNode = JSON.Parse(json);

        // Deserialize the JSONNode into an RSSFeed object
        RSSFeed rssFeed = new RSSFeed
        {
            title = rootNode["title"],
            
            link = rootNode["content"],
            description = rootNode["description"],
            language = rootNode["language"],
            lastBuildDate = rootNode["lastBuildDate"],
            pubDate = rootNode["pubDate"],
            item = ParseItemsList(rootNode["data"].AsArray)
        };

        return rssFeed;
    }

    private static List<RSSFeedItem> ParseItemsList(JSONArray dataNode)
    {
        List<RSSFeedItem> itemList = new List<RSSFeedItem>();
        foreach (JSONNode dataItem in dataNode)
        {
            RSSFeedItem item = new RSSFeedItem
            {
                title = dataItem["title"],
                venue = dataItem["venue"],
                imagethumb = dataItem["imagethumb"],
                tickets = dataItem["tickets"],

                link = dataItem["link"],
                description = dataItem["content"],
                start = dataItem["start"],
                endTime = dataItem["end"], // Assuming you have an "end" field in your JSON
                creator = "", // Set creator field if available in JSON
                category = new List<string>(), // Set category list if available in JSON
                guid = "", // Set guid if available in JSON
                comments = "" // Set comments if available in JSON
            };
            itemList.Add(item);
        }
        return itemList;
    }
}
