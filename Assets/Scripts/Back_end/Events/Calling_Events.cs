using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HtmlAgilityPack;
using SimpleJSON;
using System;
using System.Web;
using System.Net.Http;

using System.Linq;
using System.Runtime.InteropServices;
using static ScheduleManager;
using System.Threading;

using UnityEngine.Networking;

public class Calling_Events : MonoBehaviour
{
    public GameObject eventPrefab;
    public Transform eventContainer;
    public string feedUrl = "https://www.wmnf.org/api/wmnf-events.php"; 
    public float refreshInterval = 60f;
    public Transform expandedEventContainer; // Assign your expanded event container in the Inspector

    // Manually assign TextMeshProUGUI components in the Inspector
    public TextMeshProUGUI titleUIText;
    public TextMeshProUGUI timeUIText;
    public TextMeshProUGUI venueUIText;
    public TextMeshProUGUI venueparent;
    public TextMeshProUGUI linkUIText;
    public TextMeshProUGUI descriptionUIText;
    public TextMeshProUGUI readMoreUIText;
    public TextMeshProUGUI dateTMP;
    public TextMeshProUGUI dateTMPend;// TextMeshProUGUI for date
    public GameObject cent;
    public TextMeshProUGUI timeTime;
    public GameObject Content; // Assign your UI element in the Inspector
    public TextMeshProUGUI headertitle;
    public Image thumbnail;
    private GameObject currentExpandedEvent;
    private List<GameObject> createdEventElements = new List<GameObject>(); // Keep track of created event elements

    public RectTransform vlayoutgroup;
    public RectTransform vlayoutgroup2;

    public Button shareButton1; // Button for sharing
    public Button shareButton2; // Button for sharing

    [Obsolete]
    private void Start()
    {
        StartCoroutine(RefreshEventData());
    }

    [Obsolete]
    private IEnumerator RefreshEventData()
    {
        while (true)
        {
            RSSFeed rssFeed = APIManager_Events.GetEventsFromRSS(feedUrl);

            ClearEventUI(); // Clear old instances before refreshing

            if (rssFeed != null && rssFeed.item != null && rssFeed.item.Count > 0)
            {
                dateTMP.text = ""; // Clear the date text

                foreach (RSSFeedItem item in rssFeed.item)
                {
                    GameObject eventElement = Instantiate(eventPrefab, eventContainer);
                    eventElement.SetActive(true);
                    createdEventElements.Add(eventElement); // Add to the list of created event elements

                    Button eventButton = eventElement.GetComponentInChildren<Button>();
                    eventButton.onClick.AddListener(() => OnEventButtonClick(item));

                    TextMeshProUGUI titleTMP = eventElement.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI timeTMP = eventElement.transform.Find("Time").GetComponent<TextMeshProUGUI>();
                    
                    string decodedTitle = System.Net.WebUtility.HtmlDecode(item.title);
                    titleTMP.text = decodedTitle;

                    
                    if (System.DateTime.TryParse(item.start, out System.DateTime eventDate))
                    {
                       // TimeZoneInfo easternZone;
                        DateTime easternTime;
                        //try
                        // {
                        //    easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                        //     easternTime = TimeZoneInfo.ConvertTimeFromUtc(eventDate, easternZone);
                        // }
                        //  catch (TimeZoneNotFoundException)
                        // {
                        //     easternZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                        //     easternTime = TimeZoneInfo.ConvertTimeFromUtc(eventDate, easternZone);
                        //  }
                        // finally
                        // {
                        //     easternTime = eventDate.AddHours(-5);
                        // }

                        easternTime = eventDate.AddHours(-5);


                        string formattedDate = easternTime.ToString("MMM dd");
                        string formattedTime = easternTime.ToString("t");
                        // Update the date TMP within the event prefab
                        dateTMP = eventElement.transform.Find("Image/Image1/GameObject/DateTMP").GetComponent<TextMeshProUGUI>();
                        dateTMP.text = formattedDate;

                        timeTMP.text = formattedTime; 
                    }
                    if (System.DateTime.TryParse(item.endTime , out System.DateTime eeventDate))
                    {
                       // TimeZoneInfo easternZone;
                       // try
                       // {
                        //    easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                       // }
                       // catch (TimeZoneNotFoundException)
                       // {
                       //     easternZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                       // }
                        
                        DateTime easternTime = eeventDate.AddHours(-5);
                        string formattedDate = easternTime.ToString("MMM dd");
                        string formattedTimeE = easternTime.ToString("t");
                        dateTMPend.text = formattedDate;
                        timeTMP.text = timeTMP.text + " - " + formattedTimeE;

                    }
                    if (dateTMP.text == dateTMPend.text)
                    {
                        dateTMPend.gameObject.SetActive(false);
                        cent.SetActive(false);
                    }
                    else
                    {
                        dateTMPend.gameObject.SetActive(true);
                        cent.SetActive(true);
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(vlayoutgroup);
                LayoutRebuilder.ForceRebuildLayoutImmediate(vlayoutgroup2);
            }
            else
            {
                GameObject eventElement = Instantiate(eventPrefab, eventContainer);
                eventElement.SetActive(true);

                TextMeshProUGUI titleTMP = eventElement.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeTMP = eventElement.transform.Find("Time").GetComponent<TextMeshProUGUI>();

                titleTMP.text = "No upcoming events";
                timeTMP.text = "";
            }

            yield return new WaitForSeconds(refreshInterval);
        }
    }

    private void OnEventButtonClick(RSSFeedItem item)
    {
        currentExpandedEvent = expandedEventContainer.gameObject;
        currentExpandedEvent.SetActive(true);

        // Assign date, title, time, venue, link, and description manually
        string decodedString = System.Net.WebUtility.HtmlDecode(item.title);
        titleUIText.text = decodedString;

        System.DateTime dateTime = System.DateTime.Parse(item.start);
        System.DateTime dateTimeend = System.DateTime.Parse(item.endTime);
        dateTime = dateTime.AddHours(-5);
        dateTimeend = dateTimeend.AddHours(-5);

        timeUIText.text = dateTime.ToString("ddd hh:mm tt")+" - "+dateTimeend.ToString("ddd hh:mm tt");

        if (string.IsNullOrEmpty(item.venue))
        {
            venueUIText.gameObject.SetActive(false);
            venueparent.gameObject.SetActive(false);
        }
        else
        {

            venueUIText.text = item.venue;
            venueparent.text = "Venue:";
            venueUIText.gameObject.SetActive(true);
            venueparent.gameObject.SetActive(true);
        }
        
        headertitle.text = decodedString;


        //image thumb
        
        StartCoroutine(LoadSpriteImage(item.imagethumb, thumbnail));


        // Use HtmlAgilityPack to extract the link from the HTML format
       // var htmlDoc = new HtmlDocument();
       // htmlDoc.LoadHtml(item.description);

       // var linkNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@href]");
        string extractedLink = item.tickets;

        if (!string.IsNullOrEmpty(extractedLink))
        {
            linkUIText.GetComponent<Button>().onClick.RemoveAllListeners();
            linkUIText.GetComponent<Button>().onClick.AddListener(() => OpenLink(extractedLink));
        }
        else
        {
            linkUIText.text = "Link not found";
        }

        // Use HtmlAgilityPack to remove HTML tags from the description
        string decodedDescription = System.Net.WebUtility.HtmlDecode(item.description);
        string cleanedDescription = RemoveHtmlTags(decodedDescription);
        descriptionUIText.text = cleanedDescription;

        readMoreUIText.text = "Read More >>";

        readMoreUIText.GetComponent<Button>().onClick.RemoveAllListeners();
        readMoreUIText.GetComponent<Button>().onClick.AddListener(() => OpenLink(extractedLink));

        // Set up the first share button
        shareButton1.onClick.RemoveAllListeners(); // Clear existing click listeners
        shareButton1.onClick.AddListener(() => Share(extractedLink, item.title));

        // Set up the second share button
        shareButton2.onClick.RemoveAllListeners(); // Clear existing click listeners
        shareButton2.onClick.AddListener(() => Share(extractedLink, item.title));

        // Deactivate the event container after expanding the event
        eventContainer.gameObject.SetActive(false);

        // Activate the expanded event container
        currentExpandedEvent.gameObject.SetActive(true);

        rebuild();
        Invoke("rebuild", .7f);
        
        
    }

    public void rebuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(vlayoutgroup);
        LayoutRebuilder.ForceRebuildLayoutImmediate(vlayoutgroup2);
    }

    private void OpenLink(string url)
    {
        // Open the link in a web browser or implement your desired behavior here
        Application.OpenURL(url);
    }

    private string RemoveHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var doc = new HtmlDocument();
        doc.LoadHtml(input);

        // Use XPath to select all text nodes within the HTML document
        var textNodes = doc.DocumentNode.SelectNodes("//text()");

        if (textNodes == null)
            return input;

        // Concatenate the text nodes to reconstruct the cleaned text
        var cleanedText = string.Join(" ", textNodes.Select(node => node.InnerText));

        return cleanedText;
    }

    public void SetContentActive()
    {
        rebuild();
        // Check if the reference to your UI element is not null
        if (Content != null)
        {
            // Set the UI element as active
            Content.SetActive(true);
        }
        else
        {
            Debug.LogError("UI element reference is not assigned in the Inspector.");
        }
    }

    private void ClearEventUI()
    {
        // Destroy all previously created event elements
        foreach (var eventElement in createdEventElements)
        {
            Destroy(eventElement);
        }
        createdEventElements.Clear(); // Clear the list
    }

    private void Share(string link, string title)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
         new NativeShare()
        .SetSubject("WMNF News").SetText(title).SetUrl(link)
        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
#elif UNITY_IOS && !UNITY_EDITOR
        ShareOniOS(title, link);
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
                    image.sprite = sprite;
                }
                else
                {
                    Debug.LogError("Error loading image: " + www.error);
                }
            }
        }
    }

}
