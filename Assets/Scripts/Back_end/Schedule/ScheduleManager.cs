using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Collections;
using WMNF_API;
using HtmlAgilityPack;

public class ScheduleManager : MonoBehaviour
{
    public GameObject[] containers;
    public GameObject dayPrefab;
    public string apiUrl = "https://www.wmnf.org/api/programs.php?ver=20160427";
    public int maxEventsPerContainer = 100;
    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI dayTMP;
    public TextMeshProUGUI startTimeTMP;
    public TextMeshProUGUI endTimeTMP;
    public TextMeshProUGUI descriptionTMP;
    public Button part1Button;
    public Button part2Button;
    public Button part3Button;
    public AudioSource audioSource;
    public TextMeshProUGUI headertitle;

    private List<UnityWebRequest> activeRequests = new List<UnityWebRequest>();
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    private void OnApplicationQuit()
    {
        foreach (var coroutine in activeCoroutines)
        {
            StopCoroutine(coroutine);
        }

        foreach (var request in activeRequests)
        {
            request.Abort();
        }
    }

    private IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        www.timeout = 100;
        activeRequests.Add(www);
        yield return www.SendWebRequest();
        activeRequests.Remove(www);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching schedule data: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        var scheduleResponse = JsonConvert.DeserializeObject<ScheduleResponse>(json);

        if (scheduleResponse != null && scheduleResponse.data != null)
        {
            foreach (var program in scheduleResponse.data)
            {
                var dayAsString = program.schedule[0].day;

                if (dayToIndex.ContainsKey(dayAsString))
                {
                    var dayIndex = dayToIndex[dayAsString];
                    var container = containers[dayIndex];

                    if (container.transform.childCount >= maxEventsPerContainer)
                    {
                        Debug.Log($"Container for {dayAsString} is full. Skipping event: {program.title}");
                        continue;
                    }

                    var prefab = Instantiate(dayPrefab, container.transform);

                    var titleText = prefab.GetComponentInChildren<TextMeshProUGUI>();
                    if (titleText != null)
                    {
                        titleText.text = program.title;
                        Debug.Log("Set title text for program: " + program.title);
                    }
                    else
                    {
                        Debug.LogError("Title TextMeshProUGUI component not found on prefab for program: " + program.title);
                    }

                    var descriptionText = prefab.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
                    if (descriptionText != null)
                    {
                        var formattedContent = FormatHtmlContent(program.content);
                        formattedContent = RemoveImagesFromHtml(formattedContent);
                        int maxCharacters = 200;
                        if (formattedContent.Length > maxCharacters)
                        {
                            formattedContent = formattedContent.Substring(0, maxCharacters) + "...";
                        }
                        descriptionText.text = formattedContent;
                        Debug.Log("Set description text for program: " + program.title);
                    }
                    else
                    {
                        Debug.LogWarning("Description TextMeshProUGUI component not found on prefab for program: " + program.title);
                    }

                    var childImage = prefab.transform.Find("Image")?.GetComponent<Image>();
                    if (childImage != null)
                    {
                        childImage.sprite = null;
                        var imageUrl = program.imageThumb; // Use the 'image-thumb' field from the API data

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = Uri.UnescapeDataString(imageUrl); // Unescape the URL
                            Coroutine coroutine = StartCoroutine(LoadImage(childImage, imageUrl));
                            activeCoroutines.Add(coroutine);
                        }
                        else
                        {
                            Debug.LogWarning("Image URL not found in API data for program: " + program.title);
                        }

                    }
                    else
                    {
                        Debug.LogWarning("Child Image component not found on prefab for program: " + program.title);
                    }

                    SetShowtimeTMPFields(program.schedule[0]);

                    Button programButton = prefab.GetComponentInChildren<Button>();
                    programButton.onClick.AddListener(() => OnScheduleButtonClick(program));
                }
                else
                {
                    Debug.LogError($"Unknown day of the week: {dayAsString} for program: " + program.title);
                }
            }
        }
    }

    private string FormatHtmlContent(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        var innerText = doc.DocumentNode.InnerText;
        return innerText;
    }

    private string RemoveImagesFromHtml(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        var imgNodes = doc.DocumentNode.SelectNodes("//img");
        if (imgNodes != null)
        {
            foreach (var imgNode in imgNodes)
            {
                imgNode.ParentNode.RemoveChild(imgNode);
            }
        }
        var innerText = doc.DocumentNode.InnerHtml;
        return innerText;
    }

    private IEnumerator LoadImage(Image image, string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading image: " + www.error);
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        if (texture != null)
        {
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.rectTransform.sizeDelta = new Vector2(117, 113);
        }
    }

    [System.Serializable]
    public class ScheduleResponse
    {
        public string status;
        public List<Program> data;
    }

    [System.Serializable]
    public class Program
    {
        public int id;
        public string title;
        public string content;
        public string imageThumb;
        public List<Playlist> playlist;
        public List<Schedule> schedule;
    }

    [System.Serializable]
    public class Playlist
    {
        public string name;
        public List<Track> data;
    }

    [System.Serializable]
    public class Track
    {
        public string file;
        public int length;
        public string title;
    }

    private Dictionary<string, int> dayToIndex = new Dictionary<string, int>
    {
        { "Sunday", 0 },
        { "Monday", 1 },
        { "Tuesday", 2 },
        { "Wednesday", 3 },
        { "Thursday", 4 },
        { "Friday", 5 },
        { "Saturday", 6 }
    };

    private void SetShowtimeTMPFields(Schedule schedule)
    {
        dayTMP.text = schedule.day;
        startTimeTMP.text = schedule.start;
        endTimeTMP.text = !string.IsNullOrEmpty(schedule.end) ? schedule.end : "N/A";
    }

    private void OnScheduleButtonClick(Program program)
    {
        Debug.Log("Schedule button clicked for program: " + program.title);

        titleTMP.text = program.title;
        headertitle.text = program.title;
        descriptionTMP.text = FormatHtmlContent(program.content);
        SetMP3Tracks(program.playlist);
    }

    private void SetMP3Tracks(List<Playlist> playlists)
    {
        part1Button.onClick.RemoveAllListeners();
        part2Button.onClick.RemoveAllListeners();
        part3Button.onClick.RemoveAllListeners();

        foreach (var playlist in playlists)
        {
            foreach (var track in playlist.data)
            {
                if (string.Equals(track.title, "Part 1", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Setting Part 1 button click listener for program: " + track.title);
                    part1Button.onClick.AddListener(() => PlayMP3(track.file));
                }
                else if (string.Equals(track.title, "Part 2", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Setting Part 2 button click listener for program: " + track.title);
                    part2Button.onClick.AddListener(() => PlayMP3(track.file));
                }
                else if (string.Equals(track.title, "Part 3", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Setting Part 3 button click listener for program: " + track.title);
                    part3Button.onClick.AddListener(() => PlayMP3(track.file));
                }
            }
        }
    }

    private void PlayMP3(string mp3URL)
    {
        if (!string.IsNullOrEmpty(mp3URL) && audioSource != null)
        {
            StartCoroutine(LoadAudioFromURL(mp3URL));
        }
    }

    private IEnumerator LoadAudioFromURL(string mp3URL)
    {
        using (var www = UnityWebRequestMultimedia.GetAudioClip(mp3URL, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

                if (audioClip != null)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("Error loading audio clip from URL: " + mp3URL);
                }
            }
        }
    }
}
