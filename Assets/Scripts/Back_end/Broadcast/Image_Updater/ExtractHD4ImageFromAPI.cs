using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;

public class ExtractHD4ImageFromAPI : MonoBehaviour
{
    public string apiUrl = "https://www.wmnf.org/channels/feed/";
    public Image image; // Change the field to Image
    public Button extractImageButton;

    private string broadcastChannelImageUrl;

    [System.Obsolete]
    private void Start()
    {
        extractImageButton.onClick.AddListener(ExtractImage);
    }

    [System.Obsolete]
    public void ExtractImage()
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

                // Parse the XML to extract the image URL for "WMNF HD4 New Sounds"
                broadcastChannelImageUrl = ExtractImageUrlFromXml(rssXml, "WMNF HD4 New Sounds");

                if (!string.IsNullOrEmpty(broadcastChannelImageUrl))
                {
                    // Load and assign the image
                    StartCoroutine(LoadImage(broadcastChannelImageUrl));
                }
                else
                {
                    Debug.LogError("Image URL not found in XML.");
                }
            }
            else
            {
                Debug.LogError("Failed to load RSS XML: " + www.error);
            }
        }
    }

    private string ExtractImageUrlFromXml(string xml, string title)
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Find the image URL by selecting the "image" tag inside the "item" node with the specified title
            XmlNode itemNode = xmlDoc.SelectSingleNode($"//item[title='{title}']");
            if (itemNode != null)
            {
                XmlNode imageNode = itemNode.SelectSingleNode("image");
                if (imageNode != null)
                {
                    return imageNode.InnerText;
                }
            }

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
