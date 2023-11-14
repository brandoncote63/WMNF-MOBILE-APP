using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

[Serializable]
public class NPRPlaylistData
{
    public string ucsNow;
    public NPRPlaylistEpisode[] playlist;
}



[Serializable]
public class NPRPlaylistEpisode
{
    public string episode_id;
    public string program_id;
    public string name;
    public string program_format;
    public string program_link;
    public string date;
    public string start_utc;
    public string end_utc;
    public string start_time;
    public string end_time;
    public NPRPlaylistItem[] playlist;
}

[Serializable]
public class NPRPlaylistItem
{
    public string id;
    public long _duration;
    public string _start_time;
    public string trackName;
    public string artistName;
    public string collectionName;
    public string album_art;
    public NPRPlaylistBuyLinks buy;
}

[Serializable]
public class NPRPlaylistBuyLinks
{
    public string amazon;
}

public class NPRPlaylistFetcher : MonoBehaviour
{
    public string playlistUrl = "https://api.composer.nprstations.org/v1/widget/555341e0e1c820fc55f5436d/playlist?t=1470075372214&limit=50&order=-1&loadingMesgeId=widgetElement_704&errorMsg=Sorry%2C+there+is+no+playlist+to+display.";
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI timeFrameText;
    public int startresults;
    public int endresults;
    public int startresults1;
    public int endresults1;
   
    public string startresults2;
    public string endresults2;
    [Obsolete]
    private IEnumerator Start()
    {
        yield return FetchPlaylistData();
    }

    [Obsolete]
    private IEnumerator FetchPlaylistData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(playlistUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching playlist data: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;

                try
                {
                    NPRPlaylistData playlistData = JsonUtility.FromJson<NPRPlaylistData>(json);

                    if (playlistData.playlist.Length > 0)
                    {
                        NPRPlaylistEpisode nowPlayingEpisode = playlistData.playlist[playlistData.playlist.Length - 1];
                        Debug.Log("Now Playing: " + nowPlayingEpisode.name);
                        Debug.Log(playlistData.playlist.Length);

                        string title = nowPlayingEpisode.name;

                        //IntParseFast(nowPlayingEpisode.start_time);
                        //IntParseFastend(nowPlayingEpisode.end_time);

                       

                        // Update the TextMeshPro UI elements
                        //titleText.text = title;
                        UpdateUpNextUI();
                        //timeFrameText.text = timeFrame;
                    }
                    else
                    {
                        Debug.LogWarning("No episodes found in the playlist.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
        }
    }


    public int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        Debug.Log(result);
        startresults = result;
        return result;
        
    }
    public int IntParseFastend(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        Debug.Log(result);
        endresults = result;
        return result;

    }

    [Obsolete]
    private void UpdateUpNextUI()
    {
        Debug.Log("Updating now playing UI...");

        StartCoroutine(APIManager_UpNext.GetUpNextData(OnUpNextDataReceived));
    }

    private void OnUpNextDataReceived(UpNextItem[] upNextData)
    {
        if (upNextData != null && upNextData.Length > 0)
        {
            string firsttwo = upNextData[0].start.Substring(0, 2);
            string efirsttwo = upNextData[0].end.Substring(0, 2);
            IntParseFast(firsttwo);
            IntParseFastend(efirsttwo);
           

            startresults1 = startresults;
            if (startresults1 < 12)
            {
                startresults2 = (startresults1.ToString())+ upNextData[0].start.Substring(2,3) + " AM";
            }
            else
            {
                if (startresults1 == 12) { startresults2 = startresults1.ToString() + upNextData[0].start.Substring(2, 3) + " PM";}
                else {
                    startresults1 = startresults1 - 12;
                    startresults2 = startresults1.ToString() + upNextData[0].start.Substring(2, 3) + " PM";

                }
                
            }

            endresults1 = endresults;
            if (endresults1 < 12)
            {
                endresults2 = (endresults1.ToString()) + upNextData[0].end.Substring(2, 3) + " AM";
            }
            else
            {
                if (endresults1 == 12) { endresults2 = endresults1.ToString() + upNextData[0].end.Substring(2, 3) + " PM"; }
                else
                {
                    endresults1 = endresults1 - 12;
                    endresults2 = endresults1.ToString() + upNextData[0].end.Substring(2, 3) + " PM";

                }

            }
            string timeFrame = startresults2 + " - " + endresults2;

            titleText.text = upNextData[0].post_title;
            timeFrameText.text = timeFrame;
        }

    }

}