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

public class Calling_Archives : MonoBehaviour
{
    public GameObject archivePrefab;
    public Transform archiveContainer;
    public float refreshInterval = 60f;

    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI showtimesTMP;
    public TextMeshProUGUI dayOfWeekTMP;
    public TextMeshProUGUI startTimeTMP;
    public TextMeshProUGUI endTimeTMP;
    public TextMeshProUGUI descriptionTMP;
    public TextMeshProUGUI headertitle;
    public Button part1Button;
    public Button subpageButton;
    public Button part2Button;
    public Button stopAllButton1;
    public Button stopAllButton2;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public List<Button> playbuttons;
    public List<int> playbuttonsint;
    public GameObject parts;
    public TextMeshProUGUI[] PartTitels;
    public GameObject Content;
    public Transform contentPlayOnDemand;
    public GameObject PlayOndemand;
    private List<GameObject> createdUIElements = new List<GameObject>();
    private List<AudioSource> playingAudioSources = new List<AudioSource>();

    [Obsolete]
    private void Start()
    {
        StartCoroutine(RefreshArchiveData());
        // Add a click listener to the stop all button
        stopAllButton1.onClick.AddListener(StopAllAudio);
        stopAllButton2.onClick.AddListener(StopAllAudio);
        
    }

    [Obsolete]
    private IEnumerator RefreshArchiveData()
    {
        while (true)
        {
            Debug.Log("Refreshing archive data...");

            ClearArchiveUI();

            UpdateArchiveUI();

            yield return new WaitForSeconds(refreshInterval);
        }
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

    archiveTitleText.text = $"<b>{HttpUtility.HtmlDecode(archive.title)}</b>";

        // Add a button click listener to expand the archive
        Button archiveButton = archiveElement.GetComponentInChildren<Button>();
        archiveButton.onClick.AddListener(() => OnArchiveButtonClick(archive));

        string showtimeText = GenerateShowtimeText(archive.schedule);

    showtimesText.text = showtimeText;

    if (archive.host != null && archive.host.Count > 0)
    {
        hostText.text = archive.host[0];
    }

    string descriptionText = HtmlAgilityPackHelper.StripHtml(HttpUtility.HtmlDecode(archive.content));
    descriptionTMP.text = descriptionText;

    // Assign a click listener to part1Button
   // part1Button.onClick.RemoveAllListeners();
     //   part1Button.onClick.AddListener(() => audiofuntion(0)) ;//PlayMP3(archive.playlist[0].data[0].file));

    // Assign a click listener to part2Button
   // part2Button.onClick.RemoveAllListeners();
     //   part2Button.onClick.AddListener(() => audiofuntion(1)); //PlayMP3(archive.playlist[0].data[1].file));
}
    public void audiofuntion(int clip)
    {
        Debug.Log("clip =" + clip);
        audioSource.clip = audioClips[clip];
        audioSource.Play();
    }
    public void updateBuutons(Button playthisbutton, int which)
    {
        Debug.Log("THE BUTTON: " + playthisbutton.name + "which: " + which);
        
        playthisbutton.onClick.RemoveAllListeners();
        playthisbutton.onClick.AddListener(() => audiofuntion(which));
        playbuttons.Add(playthisbutton);
        
    }
    private void OnArchiveButtonClick(ArchiveData archive)
    {
        if (archive.playlist[0].data.Count > 0)
        {
            var iterationCount = archive.playlist[0].data.Count;
            for (int i = 0; i < iterationCount; i++)
            {
                StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[i].file, i));
                Debug.Log("part" + i + "file:" + archive.playlist[0].data[i].file);
                GameObject partpart = Instantiate(parts, contentPlayOnDemand);
                partpart.SetActive(true);
                TextMeshProUGUI title = partpart.transform.Find("Text (TMP) titel").GetComponent<TextMeshProUGUI>();
                Button playbutton = partpart.transform.Find("playPauseButton").GetComponent<Button>();
                updateBuutons(playbutton, i);

             
                title.text = archive.playlist[0].data[i].title;
            }


        }
        else
        {
            PlayOndemand.SetActive(false);
        }

        //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[0].file, 0));
        //StartCoroutine(LoadAudioFromURL(archive.playlist[0].data[1].file, 1));
        Debug.Log("Playlist parts count: " + archive.playlist[0].data.Count);
       // Debug.Log("Part 1 file: " + archive.playlist[0].data[0].file);
        //Debug.Log("Part 2 file: " + archive.playlist[0].data[1].file);
       // PartTitels[0].text = archive.playlist[0].data[0].title;
        //PartTitels[0].text = archive.playlist[0].data[1].title;
        

        titleTMP.text = $"<b>{HttpUtility.HtmlDecode(archive.title)}</b>";

        string showtimeText = GenerateShowtimeText(archive.schedule);
        showtimesTMP.text = "All times are Eastern Standard Time (EST)";

        headertitle.text = $"<b>{HttpUtility.HtmlDecode(archive.title)}</b>";

        dayOfWeekTMP.text = archive.schedule[0].day;


        string firsttwo = archive.schedule[0].start.Substring(0, 2);
        int result = 0;
        for (int i = 0; i < firsttwo.Length; i++)
        {
            char letter = firsttwo[i];
            result = 10 * result + (letter - 48);
        }
        Debug.Log(result);
        if (result < 12) { startTimeTMP.text = result.ToString() + ":00 AM"; }
        
        else if (result == 12) { startTimeTMP.text = result.ToString() + ":00 PM"; }
        else { startTimeTMP.text = (result - 12).ToString() + ":00 PM"; }
        if (result == 0) { startTimeTMP.text = result.ToString() + "12:00 AM"; }

        // startTimeTMP.text = archive.schedule[0].start;

        if (!string.IsNullOrEmpty(archive.schedule[0].end))
        {
            endTimeTMP.text = archive.schedule[0].end;
        }
        else
        {
            endTimeTMP.gameObject.SetActive(false);
        }

        string descriptionText = HtmlAgilityPackHelper.StripHtml(HttpUtility.HtmlDecode(archive.content));
        descriptionTMP.text = descriptionText;

        archiveContainer.gameObject.SetActive(false);

        titleTMP.gameObject.SetActive(true);
        showtimesTMP.gameObject.SetActive(true);
        dayOfWeekTMP.gameObject.SetActive(true);
        startTimeTMP.gameObject.SetActive(true);
        endTimeTMP.gameObject.SetActive(true);
        descriptionTMP.gameObject.SetActive(true);
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
            Debug.Log(result);
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

    private void ClearArchiveUI()
    {
        foreach (var uiElement in createdUIElements)
        {
            Destroy(uiElement);
        }
        createdUIElements.Clear();
    }

    public void PlayMP3(string mp3URL, int data)
    {
        if (!string.IsNullOrEmpty(mp3URL) && audioSource != null)
        {
            StartCoroutine(LoadAudioFromURL(mp3URL, data));
            // Add the audio source to the list of playing audio sources
            playingAudioSources.Add(audioSource);
        }
    }

    public void StopAllAudio()
    {
        foreach (var audioSource in playingAudioSources)
        {
            audioSource.Stop();
        }

        playingAudioSources.Clear();
    }

    private IEnumerator LoadAudioFromURL(string mp3URL, int data)
    {
        Debug.Log("Attempting to load audio from URL: " + mp3URL);

        using (var www = UnityWebRequestMultimedia.GetAudioClip(mp3URL, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading audio from URL: " + mp3URL);
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log(mp3URL + "Downloading ");
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

                if (audioClip != null)
                {
                    audioClips.Add(audioClip);
                    //audioSource.Stop();
                    //audioSource.clip = audioClip;
                    //audioSource.Play(0);
                    Debug.Log("Audio downloaded successfully and assigned to audio source: " + mp3URL);
                    Debug.Log(audioClips);
                }
                else
                {
                    Debug.LogError("Error loading audio clip from URL: " + mp3URL);
                }
            }
        }
    }

   public void orderlist()
    {
        
    }

    public void SetContentActive()
    {
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
}
