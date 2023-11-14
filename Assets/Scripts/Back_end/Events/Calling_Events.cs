using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HtmlAgilityPack;
using SimpleJSON;
using System;
using System.Linq;
using System.Runtime.InteropServices;

public class Calling_Events : MonoBehaviour
{
    public GameObject eventPrefab;
    public Transform eventContainer;
    public string feedUrl = "https://www.wmnf.org/api/events.php?ver=20160427";
    public float refreshInterval = 60f;
    public Transform expandedEventContainer; // Assign your expanded event container in the Inspector

    // Manually assign TextMeshProUGUI components in the Inspector
    public TextMeshProUGUI titleUIText;
    public TextMeshProUGUI timeUIText;
    public TextMeshProUGUI venueUIText;
    public TextMeshProUGUI linkUIText;
    public TextMeshProUGUI descriptionUIText;
    public TextMeshProUGUI readMoreUIText;
    public TextMeshProUGUI dateTMP; // TextMeshProUGUI for date

    public GameObject Content; // Assign your UI element in the Inspector
    public TextMeshProUGUI headertitle;

    private GameObject currentExpandedEvent;
    private List<GameObject> createdEventElements = new List<GameObject>(); // Keep track of created event elements

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

                    string decodedTitle = System.Web.HttpUtility.HtmlDecode(item.title);
                    titleTMP.text = decodedTitle;

                    if (System.DateTime.TryParse(item.pubDate, out System.DateTime eventDate))
                    {
                        string formattedDate = eventDate.ToString("MMM dd");

                        // Update the date TMP within the event prefab
                        dateTMP = eventElement.transform.Find("Image/Image1/DateTMP").GetComponent<TextMeshProUGUI>();
                        dateTMP.text = formattedDate;
                    }
                }
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
        titleUIText.text = item.title;
        timeUIText.text = item.pubDate;
        venueUIText.text = ""; // Manually assign venue here
        headertitle.text = item.title;

        // Use HtmlAgilityPack to extract the link from the HTML format
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(item.description);

        var linkNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@href]");
        string extractedLink = linkNode?.Attributes["href"]?.Value;

        if (!string.IsNullOrEmpty(extractedLink))
        {
            linkUIText.text = extractedLink;
        }
        else
        {
            linkUIText.text = "Link not found";
        }

        // Use HtmlAgilityPack to remove HTML tags from the description
        string decodedDescription = System.Web.HttpUtility.HtmlDecode(item.description);
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
        ShareOnAndroid(title, link);
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

}
