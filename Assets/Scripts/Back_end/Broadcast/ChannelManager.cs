using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ChannelManager : MonoBehaviour
{
    [System.Serializable]
    public class ChannelInfo
    {
        public string name;
        public string streamLink;
    }

    public AudioSource audioSource;
    public GameObject prefabContainer; // Reference to the container for instantiated prefabs

    private List<ChannelInfo> channels = new List<ChannelInfo>();
    private int currentChannelIndex = 0;

    private void Start()
    {
        StartCoroutine(LoadChannelsFromFeed());
    }

    private IEnumerator LoadChannelsFromFeed()
    {
        string feedUrl = "https://www.wmnf.org/channels/feed/";

        using (UnityWebRequest www = UnityWebRequest.Get(feedUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching feed data: " + www.error);
            }
            else
            {
                Debug.Log("Feed data loaded successfully.");

                string feedContent = www.downloadHandler.text;
                channels = ParseFeedContent(feedContent);

                if (channels.Count > 0)
                {
                    Debug.Log("Channels loaded: " + channels.Count);
                    PlayCurrentChannel();
                }
                else
                {
                    Debug.LogError("No channels found in feed.");
                }
            }
        }
    }

    private List<ChannelInfo> ParseFeedContent(string feedContent)
    {
        List<ChannelInfo> parsedChannels = new List<ChannelInfo>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(feedContent);

        XmlNodeList itemNodes = xmlDoc.SelectNodes("//item");
        foreach (XmlNode itemNode in itemNodes)
        {
            ChannelInfo channel = new ChannelInfo();
            channel.name = itemNode.SelectSingleNode("title").InnerText;
            channel.streamLink = itemNode.SelectSingleNode("link").InnerText;
            parsedChannels.Add(channel);
        }

        return parsedChannels;
    }

    private void PlayCurrentChannel()
    {
        if (channels.Count == 0 || currentChannelIndex >= channels.Count)
        {
            Debug.LogError("No channels available or index out of range.");
            return;
        }

        ChannelInfo currentChannel = channels[currentChannelIndex];
        StartCoroutine(PlayAudioFromUrl(currentChannel.streamLink));

        // Update the channel name text
        TMP_Text channelNameText = prefabContainer.transform.GetChild(currentChannelIndex).GetComponentInChildren<TMP_Text>();
        if (channelNameText != null)
        {
            channelNameText.text = currentChannel.name;
        }
    }

    private IEnumerator PlayAudioFromUrl(string audioUrl)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading audio clip: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();

                Debug.Log("Audio clip is now playing. Clip duration: " + clip.length + " seconds");
            }
        }
    }

    public void SwitchChannel(int newIndex)
    {
        currentChannelIndex = newIndex;
        PlayCurrentChannel();
    }
}
