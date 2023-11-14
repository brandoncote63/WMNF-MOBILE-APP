using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;

public class ExtractBroadcastImageFromAPI : MonoBehaviour
{
    public string apiUrl = "https://www.wmnf.org/channels/feed/";
    public Image image; // Change the field to Image

    // Reference to the buttons for updating the image
    public Button extractImageButton;
    public Button updateButton1;

    private string broadcastChannelImageUrl;

    [System.Obsolete]
    private void Start()
    {
        // Add click events to your buttons
        extractImageButton.onClick.AddListener(LoadImageOnStart);
        updateButton1.onClick.AddListener(ExtractImage);

        // Load the image upon starting the app
        LoadImageOnStart();
    }

    [System.Obsolete]
    private void LoadImageOnStart()
    {
        StartCoroutine(LoadRssXml());
    }

    [System.Obsolete]
    private void ExtractImage()
    {
        StartCoroutine(LoadRssXml());
    }

    [System.Obsolete]
    private IEnumerator LoadRssXml()
    {
        using (WWW www = new WWW(apiUrl))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                string rssXml = www.text;

                // Parse the XML to extract the image URL
                broadcastChannelImageUrl = ExtractImageUrlFromXml(rssXml);

                // Load and assign the image
                StartCoroutine(LoadImage(broadcastChannelImageUrl));
            }
            else
            {
                Debug.LogError("Failed to load RSS XML: " + www.error);
            }
        }
    }

    private string ExtractImageUrlFromXml(string xml)
    {
        try
        {
            // Load the XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Find the image URL by selecting the image tag inside the "item" node with the title "Broadcast Channel"
            XmlNode itemNode = xmlDoc.SelectSingleNode("//item[title='Broadcast Channel']");
            if (itemNode != null)
            {
                XmlNode imageNode = itemNode.SelectSingleNode("image");
                if (imageNode != null)
                {
                    return imageNode.InnerText;
                }
            }

            Debug.LogError("Image URL not found in XML.");
            return null;
        }
        catch (XmlException ex)
        {
            Debug.LogError("Error parsing XML: " + ex.Message);
            return null;
        }
    }

    [System.Obsolete]
    private IEnumerator LoadImage(string url)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                // Create a sprite from the downloaded texture
                Sprite sprite = Sprite.Create(
                    www.texture,
                    new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Assign the sprite to the Image component
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }
}
