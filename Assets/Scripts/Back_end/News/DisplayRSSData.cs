using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using Firebase.Analytics;

public class DisplayRSSData : MonoBehaviour
{
    public string feedUrl = "https://www.wmnf.org/category/news/feed";
    public GameObject dataElementPrefab; // Assign the prefab in the Inspector
    public Transform dataContainer; // Assign the container transform in the Inspector
    public Transform expandedDataUI; // Assign the manually created UI layout through the Inspector
    public Image thumnail;
    private GameObject currentExpandedData;
    private List<GameObject> createdDataElements = new List<GameObject>(); // Keep track of created data elements

    public GameObject Content; // Assign your UI element in the Inspector
    public TextMeshProUGUI headertitle;
    public TextMeshProUGUI page;
    public Button shareButton1;
    public Button shareButton2;

    public int pageNumber = 1;





    private void Start()
    {
        
        Debug.Log("Fetching and displaying RSS data...");
        FetchAndDisplayRSSData();

    }
    

    private void FetchAndDisplayRSSData()
    {
        List<RSSItem> rssItems = ParseRSSFeed(feedUrl);

        Debug.Log($"Fetched {rssItems.Count} RSS items.");

        ClearDataUI(); // Clear old instances before fetching and displaying

        foreach (RSSItem item in rssItems)
        {
            // Create a new instance of the prefab for each RSS item
            GameObject dataElement = Instantiate(dataElementPrefab, dataContainer);
            dataElement.SetActive(true); // Activate the cloned prefab
            createdDataElements.Add(dataElement); // Add to the list of created data elements
            if (dataElement == null)
            {
                Debug.LogError("Prefab instantiation failed!");
            }

            TextMeshProUGUI titleTMP = dataElement.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateTMP = dataElement.transform.Find("Date").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI tag = dataElement.transform.Find("tag/TagText").GetComponent <TextMeshProUGUI>();

            //image thumbnail


            Image thumimage = dataElement.transform.Find("Image").GetComponent<Image>();
            
            
                StartCoroutine(LoadSpriteImage(item.Image, thumimage));

            
            

            // Make the title bold using rich text formatting
            titleTMP.text = $"<b>{item.Title}</b>";

            // Format the date as "MMM d, yyyy" (e.g., "Sep 1, 2023")
            dateTMP.text = item.PubDate.ToString("MMM d, yyyy");

            tag.text = item.Category;

            // Add a button click listener to expand the data
            Button dataButton = dataElement.GetComponentInChildren<Button>();
            dataButton.onClick.AddListener(() => OnDataButtonClick(item));

            

            Debug.Log($"Displayed RSS item: {item.Title}");


            
            // Yield to the next frame before processing the next item
            

        }
        

       
      
        Debug.Log($"Created {createdDataElements.Count} data elements."); // Log the number of data elements created
        

    }
  

    private void OnDataButtonClick(RSSItem item)
    {
        FirebaseAnalytics.LogEvent("News_Article_page_view", new Parameter("Article_ID", item.Title));

        // Set the UI elements with the data from the clicked RSS item
        TextMeshProUGUI titleTMP = expandedDataUI.Find("Title").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dateTMP = expandedDataUI.Find("Date").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionTMP = expandedDataUI.Find("Description").GetComponent<TextMeshProUGUI>();
        Button readMoreButton = expandedDataUI.Find("ReadMore").GetComponent<Button>();
        Image pic = expandedDataUI.Find("pic").GetComponent<Image>();


        StartCoroutine(LoadSpriteImage(item.Image, pic));
        // Set the Title
        titleTMP.text = $"<b>{item.Title}</b>";

        // Set the Date
        dateTMP.text = item.PubDate.ToString("MMM d, yyyy");

        headertitle.text = item.Title;

        // Set the Description
        descriptionTMP.text = StripHTMLTags(System.Net.WebUtility.HtmlDecode(item.Description));

        // Set up the "Read More" button
        readMoreButton.onClick.RemoveAllListeners(); // Clear existing click listeners
        readMoreButton.onClick.AddListener(() => OpenLink(item.Link, item.Title));

        // Set up the first share button
        shareButton1.onClick.RemoveAllListeners(); // Clear existing click listeners
        shareButton1.onClick.AddListener(() => Share(item.Link, item.Title));

        // Set up the second share button
        shareButton2.onClick.RemoveAllListeners(); // Clear existing click listeners
        shareButton2.onClick.AddListener(() => Share(item.Link, item.Title));

        // Deactivate the data container after expanding the data
        dataContainer.gameObject.SetActive(false);

        // Activate the manually assigned UI elements
        expandedDataUI.gameObject.SetActive(true);

        Debug.Log($"Expanded RSS item: {item.Title}");
    }

    private string StripHTMLTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    // Function to open the link in a web browser
    private void OpenLink(string link, string titel)
    {
        FirebaseAnalytics.LogEvent("News_Article_Link_click", new Parameter("Article_ID", titel));
        Application.OpenURL(link);
        Debug.Log($"Opened link: {link}");
    }

    private void ClearDataUI()
    {
        // Destroy all previously created data elements
        foreach (var dataElement in createdDataElements)
        {
            Destroy(dataElement);
        }
        createdDataElements.Clear(); // Clear the list

        Debug.Log("Cleared old data elements.");
    }

    public void SetContentActive()
    {
        // Check if the reference to your UI element is not null
        if (Content != null)
        {
            // Set the UI element as active
            Content.SetActive(true);
            Debug.Log("Set content as active.");
        }
        else
        {
            Debug.LogError("UI element reference is not assigned in the Inspector.");
        }
    }

    public struct RSSItem
    {
        public string post_id;
        public string Title;
        public string Link;
        public string Category;
        public string Description;
        public string Image;
        public DateTime PubDate;
    }

    public List<RSSItem> ParseRSSFeed(string feedUrl)
    {
        List<RSSItem> items = new List<RSSItem>();

        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(feedUrl);

            // Change the XPath query to select <item> elements directly under <channel>
            XmlNodeList itemNodes = xmlDoc.SelectNodes("rss/channel/item");

            if (itemNodes == null)
            {

                Debug.Log("No item nodes found in the RSS feed.");
                return items;
            }

            foreach (XmlNode itemNode in itemNodes)
            {
                RSSItem rssItem = new RSSItem();
                rssItem.Title = itemNode.SelectSingleNode("title")?.InnerText;
                rssItem.Link = itemNode.SelectSingleNode("link")?.InnerText;
                rssItem.Description = itemNode.SelectSingleNode("description")?.InnerText;
                rssItem.Category = itemNode.SelectSingleNode("category").InnerText;
                string tag = itemNode.SelectSingleNode("category").InnerText;
                rssItem.Image = Regex.Match(rssItem.Description, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
                Debug.Log("HERE     :  " + tag);
                // Use a unique identifier like "guid" to differentiate items
                rssItem.post_id = itemNode.SelectSingleNode("post-id")?.InnerText;

                string pubDateStr = itemNode.SelectSingleNode("pubDate")?.InnerText;
                if (!string.IsNullOrEmpty(pubDateStr))
                {
                    if (DateTime.TryParse(pubDateStr, out DateTime pubDate))
                    {
                        rssItem.PubDate = pubDate;
                    }
                    else
                    {
                        Debug.LogError("Error parsing pubDate: " + pubDateStr);
                        // Handle parsing error as needed
                    }
                }

                items.Add(rssItem);

                //Debug.Log("Parsed RSS item:");
                //Debug.Log("Title: " + rssItem.Title);
                //Debug.Log("Link: " + rssItem.Link);
                //Debug.Log("Description: " + rssItem.Description);
                //Debug.Log("Post ID: " + rssItem.post_id);
                //Debug.Log("PubDate: " + rssItem.PubDate.ToString("yyyy-MM-dd HH:mm:ss"));
                //Debug.Log("-----------------------------");
            }

        }
        catch (Exception ex)
        {
            // Handle exception as needed
            Debug.LogError("Error parsing RSS feed: " + ex.Message);
        }

        return items;
    }

    private void Share(string link, string title)
    {
        FirebaseAnalytics.LogEvent("News_article_share", new Parameter("Article_ID", title));

#if UNITY_ANDROID && !UNITY_EDITOR
            
             new NativeShare()
        .SetSubject("WMNF News").SetText(title).SetUrl(link)
        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();


#elif UNITY_IOS && !UNITY_EDITOR
            ShareOniOS(link,title);
#else
        // For other platforms (Windows, macOS, etc.), display a simple share prompt.
        ShowSharePopup(title + " " + link);

        

#endif
    }

    // Android Native Share
    [DllImport("JNIUnity")]
    private static extern void ShareOnAndroid(string message, string subject);

    // iOS Native Share
    [DllImport("__Internal")]
    private static extern void ShareOniOS(string message, string subject);

    // Display a basic share prompt for other platforms
    private void ShowSharePopup(string message)
    {
        // Implement a platform-specific share dialogue here (e.g., using native plugins).
        Debug.Log("Sharing on this platform is not supported.");
    }

    public void pagechange(bool more)
    {
        if (more) { pageNumber++;
            page.text = "Page: " + pageNumber.ToString();
            feedUrl = "https://www.wmnf.org/category/news/feed" + "/?paged=" + pageNumber.ToString();
            FetchAndDisplayRSSData();
        }

        else {

            if (pageNumber > 1)
            { pageNumber = pageNumber - 1;
                page.text = "Page: " + pageNumber.ToString();
                feedUrl = "https://www.wmnf.org/category/news/feed" + "/?paged=" + pageNumber.ToString();
                FetchAndDisplayRSSData();
            }
            else { }
           
            
        }
     

    }

    private IEnumerator LoadSpriteImage(string url, Image image)
    {
        
        if (!string.IsNullOrEmpty(url))
        {
           
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    
                    Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    if (image != null)
                    {
                        image.sprite = sprite;

                    }
                    
                       

                    
                    
                }
                else
                {
                    Debug.LogError("Error loading image: " + www.error);
                }
            }
        }
    }


}