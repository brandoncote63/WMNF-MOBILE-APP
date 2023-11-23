using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Web;
using System.Collections;
using WMNF_API;
using HtmlAgilityPack;
using RenderHeads.Media.AVProVideo;

public class ScheduleManager : MonoBehaviour
{
    
    public MediaPlayer[] mediaPlayer;
    public GameObject parts;
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
    public RectTransform vertlayoutg;
    public Transform contentPlayOnDemand;
    public GameObject PlayOndemand;
    private List<UnityWebRequest> activeRequests = new List<UnityWebRequest>();
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    
    public List<Button> playbuttons;
    public List<Button> pausebuttons;

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
        www.timeout = 15;
        activeRequests.Add(www);
        yield return www.SendWebRequest();
        activeRequests.Remove(www);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching schedule data: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        json = json.Replace("image-thumb", "imageThumb");
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
                        string decodedString = System.Web.HttpUtility.HtmlDecode(program.title);
                        titleText.text = decodedString;
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

                        string decodedString = System.Web.HttpUtility.HtmlDecode(formattedContent);
                        descriptionText.text = decodedString;
                        Debug.Log("Set description text for program: " + program.title);
                    }
                    else
                    {
                        Debug.LogWarning("Description TextMeshProUGUI component not found on prefab for program: " + program.title);
                    }

                    var childImage = prefab.transform.Find("Image")?.GetComponent<Image>();
                    if (childImage != null)
                    {
                        
                        var imageUrl = program.imageThumb; // Use the 'image-thumb' field from the API data

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            //childImage.sprite = null;
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

                    prefab.SetActive(true);
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
        { "Sunday", 6 },
        { "Monday", 0 },
        { "Tuesday", 1 },
        { "Wednesday", 2 },
        { "Thursday", 3 },
        { "Friday", 4 },
        { "Saturday", 5 }
    };

    private void SetShowtimeTMPFields(Schedule schedule)
    {
        dayTMP.text = schedule.day;
        startTimeTMP.text = schedule.start;
        endTimeTMP.text = !string.IsNullOrEmpty(schedule.end) ? schedule.end : "N/A";
    }

    private void OnScheduleButtonClick(Program program)
    {
        back();
      
        Debug.Log("Schedule button clicked for program: " + program.title);


        if (program.playlist[0].data.Count > 0)
        {

            var iterationCount = program.playlist[0].data.Count;
            if (iterationCount > 0)
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[i].file, i));


                    mediaPlayer[i].OpenMedia(new MediaPath(program.playlist[0].data[i].file, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                    Debug.Log("part" + i + "file:" + program.playlist[0].data[i].file);
                    GameObject partpart = Instantiate(parts, contentPlayOnDemand);
                    partpart.SetActive(true);
                    TextMeshProUGUI title = partpart.transform.Find("Text (TMP) titel").GetComponent<TextMeshProUGUI>();
                    Button playbutton = partpart.transform.Find("playPauseButton").GetComponent<Button>();
                    Button pausebutton = partpart.transform.Find("PauseButton (1)").GetComponent<Button>();
                    updateBuutons(playbutton, pausebutton, i);



                    title.text = program.playlist[0].data[i].title;
                }
            }
            else { contentPlayOnDemand.gameObject.SetActive(false); }


        }
        else
        {
            PlayOndemand.SetActive(false);
        }
        string decodedString = System.Web.HttpUtility.HtmlDecode(program.title);
        string decodedStringDES = System.Web.HttpUtility.HtmlDecode(program.content);
        titleTMP.text = decodedString;
        headertitle.text = decodedString;
        descriptionTMP.text = FormatHtmlContent(decodedStringDES);



        SetMP3Tracks(program.playlist);
        LayoutRebuilder.ForceRebuildLayoutImmediate(vertlayoutg);
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
            Debug.Log(mp3URL.ToString());
            StartCoroutine(LoadAudioFromURL(mp3URL));
        }
    }

    public void updateBuutons(Button playthisbutton, Button pasuebutton, int which)
    {
        Debug.Log("THE BUTTON: " + playthisbutton.name + "which: " + which);

        playthisbutton.onClick.RemoveAllListeners();
        playthisbutton.onClick.AddListener(() => audiofuntionstart(which));
        pasuebutton.onClick.RemoveAllListeners();
        pasuebutton.onClick.AddListener(() => audiofuntionstop(which));
        pausebuttons.Add(pasuebutton);
        playbuttons.Add(playthisbutton);

    }
    public void audiofuntionstart(int clip)
    {

        foreach (MediaPlayer m in mediaPlayer)
        {
            m.Stop();

        }
        foreach (Button b in pausebuttons)
        {
            if (b == pausebuttons[clip]) { }
            else { b.onClick.Invoke(); }

        }


        Debug.Log("clip =" + clip);
        mediaPlayer[clip].Play();
        //audioSource.clip = audioClips[clip];
        //audioSource.Play();
    }
    public void audiofuntionstop(int clip)
    {
        Debug.Log("clip =" + clip);
        mediaPlayer[clip].Stop();

    }

    public void back()
    {
        pausebuttons.Clear();
        playbuttons.Clear();

        foreach (Transform child in contentPlayOnDemand)
        {
            if (child.tag == "part")
            {
                Destroy(child.gameObject);
            }
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
