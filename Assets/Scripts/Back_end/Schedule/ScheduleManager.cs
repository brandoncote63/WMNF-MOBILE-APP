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
using Firebase.Analytics;



public class ScheduleManager : MonoBehaviour
{

    public MediaPlayer[] mediaPlayer;
    public GameObject parts;
    public GameObject[] containers;
    public GameObject dayPrefab;
    public string apiUrl = "https://www.wmnf.org/api/programs.php";
    public int maxEventsPerContainer = 35;
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
    public List<GameObject> elements;
    public GameObject mask;
    public List<Button> playbuttons;
    public List<Button> pausebuttons;

    public int nowplayingclip;
    public Slider Sliderr;

    public TextMeshProUGUI curt;
    public TextMeshProUGUI tott;
    public Slider currentSlider;
    public Button PLAYINGNEXT;
    public GameObject cameraa;

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
            // Group events by day and sort them by start time
            Dictionary<string, List<Program>> groupedEvents = new Dictionary<string, List<Program>>();
            foreach (var program in scheduleResponse.data)
            {
                var dayAsString = program.schedule[0].day;

                if (!groupedEvents.ContainsKey(dayAsString))
                {
                    groupedEvents[dayAsString] = new List<Program>();
                }

                groupedEvents[dayAsString].Add(program);

            }

            // Sort events for each day by start time
            foreach (var dayEvents in groupedEvents)
            {
                dayEvents.Value.Sort((x, y) =>
                {
                    if (TimeSpan.TryParse(x.schedule[0].start, out TimeSpan startTimeX) && TimeSpan.TryParse(y.schedule[0].start, out TimeSpan startTimeY))
                    {
                        return startTimeX.CompareTo(startTimeY);
                    }
                    return 0;
                });
            }

            // Display the sorted events
            foreach (var dayEvents in groupedEvents)
            {
                var dayAsString = dayEvents.Key;

                if (dayToIndex.ContainsKey(dayAsString))
                {
                    var dayIndex = dayToIndex[dayAsString];
                    var container = containers[dayIndex];

                    if (container.transform.childCount >= maxEventsPerContainer)
                    {
                        Debug.Log($"Container for {dayAsString} is full. Skipping events.");
                        continue;
                    }

                    foreach (var program in dayEvents.Value)
                    {
                        var prefab = Instantiate(dayPrefab, container.transform);
                        var description = prefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        string decodedStringDES = System.Net.WebUtility.HtmlDecode(program.content);

                        description.text = FormatHtmlContent(decodedStringDES);

                        var titleText = prefab.GetComponentInChildren<TextMeshProUGUI>();
                        if (titleText != null)
                        {
                            string timestart;
                            string decodedString = System.Net.WebUtility.HtmlDecode(program.title);

                            decodedString = decodedString.Replace("’", "'");
                            if (System.DateTime.TryParse(program.schedule[0].start, out System.DateTime st))
                            {
                                timestart = st.ToString("t");
                                titleText.text = timestart + " - " + decodedString;

                            }

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
                }
                else
                {
                    Debug.LogError($"Unknown day of the week: {dayAsString}. Skipping events.");
                }
            }


        }
    }

    private string FormatHtmlContent(string htmlContent)
    {
        var doc = new HtmlDocument();

        doc.LoadHtml(htmlContent);
        var innerText = doc.DocumentNode.InnerText;
        if (innerText.StartsWith("\r\n\r\n")) { innerText = innerText.Remove(0, 8); }
        if (innerText.StartsWith("\n\n")) { innerText = innerText.Remove(0, 4); }
        if (innerText.StartsWith("\n")) { innerText = innerText.Remove(0, 2); }
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
    public class Schedule
    {
        public string day;
        public string start;
        public string end;
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
        FirebaseAnalytics.LogEvent("Schedule_Program_page_view", new Parameter("Program_ID", program.title));

        if (program.playlist != null)
        {
            var interations = program.playlist.Count;

            for (int ii = 0; ii < interations; ii++)
            {


                if (program.playlist[ii].data.Count > 0)
                {
                    string text = program.playlist[ii].name;

                    var iterationCount = program.playlist[ii].data.Count;
                    if (iterationCount > 0)
                    {
                        for (int i = 0; i < iterationCount; i++)
                        {
                            //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[i].file, i));

                            if (i + ii <= mediaPlayer.Length - 1)
                            {
                                mediaPlayer[i + ii].OpenMedia(new MediaPath(program.playlist[ii].data[i].file, MediaPathType.AbsolutePathOrURL), autoPlay: false);
                                //Debug.Log("part" + i + "file:" + program.playlist[ii].data[i].file);
                                GameObject partpart = Instantiate(parts, contentPlayOnDemand);
                                partpart.SetActive(true);

                                TextMeshProUGUI title = partpart.transform.Find("Text (TMP) titel").GetComponent<TextMeshProUGUI>();
                                Button playbutton = partpart.transform.Find("playPauseButton").GetComponent<Button>();
                                Button pausebutton = partpart.transform.Find("PauseButton (1)").GetComponent<Button>();
                                Slider slider = partpart.transform.Find("Slider").GetComponent<Slider>();
                                Button fastforward = partpart.transform.Find("fastforward").GetComponent<Button>();
                                Button backtrack = partpart.transform.Find("backtrack").GetComponent<Button>();
                                TextMeshProUGUI curretTime = partpart.transform.Find("timetotal").GetComponent<TextMeshProUGUI>();
                                TextMeshProUGUI totalTime = partpart.transform.Find("timecurret").GetComponent<TextMeshProUGUI>();
                                title.text = text + " - " + program.playlist[ii].data[i].title;
                                string TITELANDPARTTITEL = program.title + " : "+title.text;
                                updateBuutons(playbutton, pausebutton, slider, (i + ii), fastforward, backtrack, curretTime, totalTime, TITELANDPARTTITEL);

                                if (System.DateTime.TryParse(program.schedule[0].start, out System.DateTime eeventDate))
                                {
                                    //  TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                                    // DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(eeventDate, easternZone);

                                    string formattedTimeE = eeventDate.ToString("t");
                                    startTimeTMP.text = formattedTimeE;

                                }
                                if (System.DateTime.TryParse(program.schedule[0].end, out System.DateTime eventDate))
                                {
                                    // TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                                    //DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(eventDate, easternZone);

                                    string formattedTimeE = eventDate.ToString("t");
                                    endTimeTMP.text = formattedTimeE;

                                }


                               

                                
                                
                            }
                        }
                    }
                    else { contentPlayOnDemand.gameObject.SetActive(false); }


                }
                else
                {
                    PlayOndemand.SetActive(false);
                }
            }
           



            
            
        }
        string decodedString = System.Net.WebUtility.HtmlDecode(program.title);
        string decodedStringDES = System.Net.WebUtility.HtmlDecode(program.content);
        titleTMP.text = decodedString;
        headertitle.text = decodedString;
        descriptionTMP.text = FormatHtmlContent(decodedStringDES);
        dayTMP.text = program.schedule[0].day;
        LayoutRebuilder.ForceRebuildLayoutImmediate(vertlayoutg);
        mask.GetComponent<dropdownsize>().rreset();
    }

   

    

    public void updateBuutons(Button playthisbutton, Button pasuebutton, Slider slider, int which, Button ff, Button rewid, TextMeshProUGUI ct, TextMeshProUGUI tt, string titel)
    {
        Debug.Log("THE BUTTON: " + playthisbutton.name + "which: " + which);
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(slider); });
        Sliderr = slider;
        playthisbutton.onClick.RemoveAllListeners();
        playthisbutton.onClick.AddListener(() => audiofuntionstart(which, slider, ct, tt, titel));
        pasuebutton.onClick.RemoveAllListeners();
        pasuebutton.onClick.AddListener(() => audiofuntionstop(which));
        ff.onClick.RemoveAllListeners();
        ff.onClick.AddListener(() => audiofunctionForward(which));
        rewid.onClick.RemoveAllListeners();
        rewid.onClick.AddListener(() => audioFuntionBackwards(which));
        pausebuttons.Add(pasuebutton);
        playbuttons.Add(playthisbutton);

    }
    public void audiofuntionstart(int clip, Slider slider, TextMeshProUGUI ct, TextMeshProUGUI tt, string titel)
    {
        FirebaseAnalytics.LogEvent("Schedule_Program_episode_play", new Parameter("Episode_ID", titel));

        InvokeRepeating("updateslider", 1, 1);
        foreach (MediaPlayer m in mediaPlayer)
        {
            m.Stop();

        }
        foreach (Button b in pausebuttons)
        {
            if (b == pausebuttons[clip]) { }
            else { b.onClick.Invoke(); }

        }


        TimeSpan ts = TimeSpan.FromSeconds(mediaPlayer[clip].Info.GetDuration());
        if (ts.Hours > 0)
        {
            tt.text = ts.ToString("h\\:mm\\:ss");

        }
        else { tt.text = ts.ToString("mm\\:ss"); }


        TimeSpan tss = TimeSpan.FromSeconds(mediaPlayer[clip].Control.GetCurrentTime());
        if (tss.Hours > 0)
        {
            ct.text = tss.ToString("h\\:mm\\:ss");

        }
        else { ct.text = tss.ToString("mm\\:ss"); }

        Debug.Log("clip =" + clip);
        mediaPlayer[clip].Events.AddListener(HandleEvent);
        mediaPlayer[clip].Play();
        TimeRanges seekRanges = mediaPlayer[clip].Control.GetSeekableTimes();

        slider.maxValue = (float)seekRanges.MaxTime;
        curt = ct;
        tott = tt;
        currentSlider = slider;
        nowplayingclip = clip;
        if (playbuttons.Count > clip + 1)
        {

            PLAYINGNEXT = playbuttons[clip + 1];

        }
       


        //audioSource.clip = audioClips[clip];
        //audioSource.Play();
    }
    public void playnextclip()
    {
        PLAYINGNEXT.onClick.Invoke();
    }

    public void audiofuntionstop(int clip)
    {
        Debug.Log("clip =" + clip);
        mediaPlayer[clip].Stop();

    }

    public void ValueChangeCheck(Slider slider)
    {

        mediaPlayer[nowplayingclip].Control.SeekFast(slider.value);


        TimeSpan tss = TimeSpan.FromSeconds(mediaPlayer[nowplayingclip].Control.GetCurrentTime());
        if (tss.Hours > 0)
        {
            curt.text = tss.ToString("h\\:mm\\:ss");

        }
        else { curt.text = tss.ToString("mm\\:ss"); }

    }
    public void audiofunctionForward(int i)
    {
        mediaPlayer[i].Control.Seek(mediaPlayer[i].Control.GetCurrentTime() + 15);

        TimeSpan tss = TimeSpan.FromSeconds(mediaPlayer[i].Control.GetCurrentTime());
        if (tss.Hours > 0)
        {
            curt.text = tss.ToString("h\\:mm\\:ss");

        }
        else { curt.text = tss.ToString("mm\\:ss"); }
    }
    public void audioFuntionBackwards(int i)
    {
        mediaPlayer[i].Control.Seek(mediaPlayer[i].Control.GetCurrentTime() - 15);

        TimeSpan tss = TimeSpan.FromSeconds(mediaPlayer[i].Control.GetCurrentTime());
        if (tss.Hours > 0)
        {
            curt.text = tss.ToString("h\\:mm\\:ss");

        }
        else { curt.text = tss.ToString("mm\\:ss"); }


    }
    public void updateslider()
    {
        currentSlider.onValueChanged.RemoveAllListeners();

        float F = (float)mediaPlayer[nowplayingclip].Control.GetCurrentTime();
        currentSlider.value = F;


        currentSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(currentSlider); });


        TimeSpan tss = TimeSpan.FromSeconds(mediaPlayer[nowplayingclip].Control.GetCurrentTime());
        if (tss.Hours > 0)
        {
            curt.text = tss.ToString("h\\:mm\\:ss");

        }
        else { curt.text = tss.ToString("mm\\:ss"); }

        TimeSpan ts = TimeSpan.FromSeconds(mediaPlayer[nowplayingclip].Info.GetDuration());
        if (ts.Hours > 0)
        {
            tott.text = ts.ToString("h\\:mm\\:ss");

        }
        else { tott.text = ts.ToString("mm\\:ss"); }


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



}