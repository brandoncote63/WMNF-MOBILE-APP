using System.Collections;
using System;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using HtmlAgilityPack;
using WMNF_API;

using RenderHeads.Media.AVProVideo;
//using UnityEngine.UIElements;

public class Calling_Archives : MonoBehaviour
{
    public GameObject archivePrefab;
    public Transform archiveContainer;
    public float refreshInterval = 2700f;
    public MediaPlayer[] mediaPlayer;
    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI showtimesTMP;
    public TextMeshProUGUI dayOfWeekTMP;
    public TextMeshProUGUI startTimeTMP;
    public TextMeshProUGUI endTimeTMP;
    public TextMeshProUGUI descriptionTMP;
    public TextMeshProUGUI headertitle;
    public Image thumbnailImage;
    public GameObject mask;
    public Button part1Button;
    public Button subpageButton;
    public Button part2Button;
    public Button stopAllButton1;
    public Button stopAllButton2;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public List<Button> playbuttons;
    public List<Button> pausebuttons;
    public List<int> playbuttonsint;
    public GameObject parts;
    public GameObject cameraa;
    public TextMeshProUGUI[] PartTitels;
    public GameObject Content;
    public Transform contentPlayOnDemand;
    public RectTransform vlayoutgroup;
    
    public GameObject PlayOndemand;
    
    private List<GameObject> createdUIElements = new List<GameObject>();
    private List<AudioSource> playingAudioSources = new List<AudioSource>();

    public int nowplayingclip;
    public Slider Sliderr;

    
    public Button PLAYINGNEXT;

    [Obsolete]
    private void Start()
    {
        refresh();
        // Add a click listener to the stop all button
        stopAllButton1.onClick.AddListener(StopAllAudio);
        stopAllButton2.onClick.AddListener(StopAllAudio);
        
    }

    [Obsolete]
    public void refresh()
    {
        ClearArchiveUI();

        UpdateArchiveUI();
    }


   

    [Obsolete]
    private void UpdateArchiveUI()
    {
        Debug.Log("Updating archive UI...");

        StartCoroutine(APIManager_Archives.GetArchivesData(OnArchiveDataReceived));
    }

    private void OnArchiveDataReceived(List<ArchiveData> archiveData)
    {
        if (archiveData != null)
        {
            // Sort the archiveData list alphabetically by title
            archiveData.Sort((a, b) => string.Compare(a.title, b.title, StringComparison.OrdinalIgnoreCase));

            foreach (var archive in archiveData)
            {
                CreateArchiveUIElement(archive);
            }
        }
        else
        {
            Debug.Log("No archive data available.");
        }
    }

    private void CreateArchiveUIElement(ArchiveData archive)
{
        // Instantiate a new archive UI element from the prefab
    GameObject archiveElement = Instantiate(archivePrefab, archiveContainer);
    archiveElement.SetActive(true);
    createdUIElements.Add(archiveElement);// Add to the list of created UI elements




    // Get references to the TMP fields in the UI element
    TextMeshProUGUI archiveTitleText = archiveElement.transform.Find("Title").GetComponent<TextMeshProUGUI>();
    TextMeshProUGUI showtimesText = archiveElement.transform.Find("ShowtimesObject/Showtimes").GetComponent<TextMeshProUGUI>();
    TextMeshProUGUI hostText = archiveElement.transform.Find("HostObject/Host").GetComponent<TextMeshProUGUI>();
        string text = $"<b>{System.Net.WebUtility.HtmlDecode(archive.title)}</b>";
        text = text.Replace("’", "'");
        archiveTitleText.text = text;

        // SET THUMBNAIL 
        Image thumbnail = archiveElement.transform.Find("Image").GetComponent<Image>();
        StartCoroutine(LoadSpriteImage(archive.imagethumb, thumbnail));
        


        // Add a button click listener to expand the archive
        Button archiveButton = archiveElement.GetComponentInChildren<Button>();
        archiveButton.onClick.AddListener(() => OnArchiveButtonClick(archive));

        string showtimeText = GenerateShowtimeText(archive.schedule);
       
    showtimesText.text = showtimeText;

    if (archive.host != null && archive.host.Count > 0)
    {
        hostText.text = archive.host[0];
    }

    string descriptionText = HtmlAgilityPackHelper.StripHtml(System.Net.WebUtility.HtmlDecode(archive.content));
    descriptionTMP.text = descriptionText;

    // Assign a click listener to part1Button
   // part1Button.onClick.RemoveAllListeners();
     //   part1Button.onClick.AddListener(() => audiofuntion(0)) ;//PlayMP3(archive.playlist[0].data[0].file));

    // Assign a click listener to part2Button
   // part2Button.onClick.RemoveAllListeners();
     //   part2Button.onClick.AddListener(() => audiofuntion(1)); //PlayMP3(archive.playlist[0].data[1].file));
}
    public void audiofuntionstart(int clip, Slider slider)
    {
        InvokeRepeating("updateslider", 1, 1);
        foreach (MediaPlayer m in mediaPlayer)
        {
            m.Stop();

        }
        foreach(Button b in pausebuttons)
        {
            if (b == pausebuttons[clip]) { }
            else { b.onClick.Invoke(); }
            
        }
        

       

        mediaPlayer[clip].Events.AddListener(HandleEvent);
        mediaPlayer[clip].Play();
        

        TimeRanges seekRanges = mediaPlayer[clip].Control.GetSeekableTimes();

        slider.maxValue = (float)seekRanges.MaxTime;
        nowplayingclip = clip;
        if (playbuttons.Count > clip + 1)
        {
            
            PLAYINGNEXT = playbuttons[clip + 1];

        }

    }
   
    public void playnextclip()
    {
        PLAYINGNEXT.onClick.Invoke();
    }
   

    // This method is called whenever there is an event from the MediaPlayer
    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        Debug.Log("MediaPlayer " + mp.name + " generated event: " + eventType.ToString());
        if (eventType == MediaPlayerEvent.EventType.Unpaused)
        {
            cameraa.GetComponent<masteraudio>().fixpasuetomatch();
        }

        if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
        {
            Debug.Log("we got this!");

            playnextclip();
        }
        
        if (eventType == MediaPlayerEvent.EventType.Error)
        {
            Debug.LogError("Error: " + code);
        }
    }


    public void audiofuntionstop(int clip)
    {
        Debug.Log("clip =" + clip);
        mediaPlayer[clip].Stop();
        
    }
    public void updateBuutons(Button playthisbutton, Button pasuebutton, Slider slider, int which)
    {
        Debug.Log("THE BUTTON: " + playthisbutton.name + "which: " + which);
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(slider); });
        Sliderr = slider;
        playthisbutton.onClick.RemoveAllListeners();
        playthisbutton.onClick.AddListener(() => audiofuntionstart(which, slider));
        pasuebutton.onClick.RemoveAllListeners();
        pasuebutton.onClick.AddListener(() => audiofuntionstop(which));
        pausebuttons.Add(pasuebutton);
        playbuttons.Add(playthisbutton);
        
    }
    public void ValueChangeCheck(Slider slider)
    {

        mediaPlayer[nowplayingclip].Control.SeekFast(slider.value);

    }
    public void updateslider()
    {
        Sliderr.onValueChanged.RemoveAllListeners();

        Sliderr.value = (float)mediaPlayer[nowplayingclip].Control.GetCurrentTime();

        Sliderr.onValueChanged.AddListener(delegate { ValueChangeCheck(Sliderr); });


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
    private void OnArchiveButtonClick(ArchiveData archive)
    {
        back();
        if (archive.playlist != null)
        {
            var interations = archive.playlist.Count;
            for (int ii = 0; ii < interations; ii++)
            {

                if (archive.playlist[ii].data.Count > 0)
                {
                    string text = archive.playlist[ii].name;

                    var iterationCount = archive.playlist[ii].data.Count;
                    for (int i = 0; i < iterationCount; i++)
                    {
                        //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[i].file, i));
                        if (i + ii <= mediaPlayer.Length - 1)
                        {

                            mediaPlayer[ii + i].OpenMedia(new MediaPath(archive.playlist[ii].data[i].file, MediaPathType.AbsolutePathOrURL), autoPlay: false);

                            GameObject partpart = Instantiate(parts, contentPlayOnDemand);
                            partpart.SetActive(true);
                            TextMeshProUGUI title = partpart.transform.Find("Text (TMP) titel").GetComponent<TextMeshProUGUI>();
                            Button playbutton = partpart.transform.Find("playPauseButton").GetComponent<Button>();
                            Button pausebutton = partpart.transform.Find("PauseButton (1)").GetComponent<Button>();
                            Slider slider = partpart.transform.Find("Slider").GetComponent<Slider>();
                            int iii = i + ii;
                            
                            updateBuutons(playbutton, pausebutton, slider, iii);



                            title.text = text + " - " + archive.playlist[ii].data[i].title;
                        }

                    }


                }
                else
                {
                    PlayOndemand.SetActive(false);
                }
            }
        }
        //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[0].file, 0));
        //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[1].file, 1));
        //Debug.Log("Playlist parts count: " + archive.playlist[0].data.Count);
       // Debug.Log("Part 1 file: " + archive.playlist[0].data[0].file);
        //Debug.Log("Part 2 file: " + archive.playlist[0].data[1].file);
       // PartTitels[0].text = archive.playlist[0].data[0].title;
        //PartTitels[0].text = archive.playlist[0].data[1].title;
        

        titleTMP.text = $"<b>{System.Net.WebUtility.HtmlDecode(archive.title)}</b>";

        string showtimeText = GenerateShowtimeText(archive.schedule);
        showtimesTMP.text = "All times are Eastern Standard Time (EST)";

        headertitle.text = $"<b>{System.Net.WebUtility.HtmlDecode(archive.title)}</b>";

        dayOfWeekTMP.text = archive.schedule[0].day;


        string firsttwo = archive.schedule[0].start.Substring(0, 2);
        int result = 0;
        for (int i = 0; i < firsttwo.Length; i++)
        {
            char letter = firsttwo[i];
            result = 10 * result + (letter - 48);
        }
        
        if (result < 12) { startTimeTMP.text = result.ToString() + ":00 AM"; }
        
        else if (result == 12) { startTimeTMP.text = result.ToString() + ":00 PM"; }
        else { startTimeTMP.text = (result - 12).ToString() + ":00 PM"; }
        if (result == 0) { startTimeTMP.text = result.ToString() + "12:00 AM"; }

        // startTimeTMP.text = archive.schedule[0].start;

        if (!string.IsNullOrEmpty(archive.schedule[0].end))
        {
            EndtimeFormatting(archive);
            //endTimeTMP.text = archive.schedule[0].end;
        }
        else
        {
            endTimeTMP.gameObject.SetActive(false);
        }

        string descriptionText = HtmlAgilityPackHelper.StripHtml(System.Net.WebUtility.HtmlDecode(archive.content));
        descriptionTMP.text = descriptionText;

        archiveContainer.gameObject.SetActive(false);
        
        titleTMP.gameObject.SetActive(true);
        showtimesTMP.gameObject.SetActive(true);
        dayOfWeekTMP.gameObject.SetActive(true);
        startTimeTMP.gameObject.SetActive(true);
        endTimeTMP.gameObject.SetActive(true);
        descriptionTMP.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(vlayoutgroup);
        mask.GetComponent<dropdownsize>().rreset();

    }

    private string GenerateShowtimeText(List<Schedule> schedules)
    {
        List<string> showtimeStrings = new List<string>();

        foreach (var schedule in schedules)
        {
            string firsttoo = schedule.start.Substring(0, 2);
            
            int result = 0;
            for (int i = 0; i < firsttoo.Length; i++)
            {
                char letter = firsttoo[i];
                result = 10 * result + (letter - 48);
            }
            
            string showtimeT;
           
            if (result < 12) {showtimeT = result.ToString() + ":00 AM"; }
            
            else if (result == 12) { showtimeT = "12:00 PM"; }
            else { showtimeT = (result - 12).ToString() + ":00 PM"; }
            if (result == 0) { showtimeT = "12:00 AM"; }

            string showtime = $"{schedule.day} @ {showtimeT}";
            showtimeStrings.Add(showtime);
        }

        string showtimeText = string.Join(", ", showtimeStrings);

        return showtimeText;
    }

    public void EndtimeFormatting(ArchiveData archive)
    {
       
            string firsttoo = archive.schedule[0].end.Substring(0, 2);

            int result = 0;
            for (int i = 0; i < firsttoo.Length; i++)
            {
                char letter = firsttoo[i];
                result = 10 * result + (letter - 48);
            }
            
            string showtimeT;

            if (result < 12) { showtimeT = result.ToString() + ":00 AM"; }

            else if (result == 12) { showtimeT = "12:00 PM"; }
            else { showtimeT = (result - 12).ToString() + ":00 PM"; }
            if (result == 0) { showtimeT = "12:00 AM"; }

        endTimeTMP.text = showtimeT;
        
    }

    private void ClearArchiveUI()
    {
        foreach (var uiElement in createdUIElements)
        {
            Destroy(uiElement);
        }
        createdUIElements.Clear();
    }

   

    public void StopAllAudio()
    {
        foreach (var audioSource in playingAudioSources)
        {
            audioSource.Stop();
        }

        playingAudioSources.Clear();
    }

   


    public void SetContentActive()
    {
        back();
        if (Content != null)
        {
            Content.SetActive(true);
        }
        else
        {
            Debug.LogError("UI element reference is not assigned in the Inspector.");
        }
    }

    public static class HtmlAgilityPackHelper
    {
        public static string StripHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc.DocumentNode.InnerText;
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
