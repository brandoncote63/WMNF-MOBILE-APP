using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System;

[System.Serializable]
public class ItemResponse
{
    public List<Playlist> playlist;
}

[System.Serializable]
public class Playlist
{
    public string episode_id;
    public string program_id;
    public string name;
    public string program_format;
    public string date;
    public string start_utc;
    public string end_utc;
    public string start_time;
    public string end_time;
    public string hosts;
    public List<PlaylistItem> playlist;
}

[System.Serializable]
public class PlaylistItem
{
    public string program;
    public string id;
    public int duration;
    public string _start_time;
    public string trackName;
    public string artistName;
    public string collectionName;
    public string album_art;
    public BuyInfo buy;
}

[System.Serializable]
public class BuyInfo
{
    public string amazon;
    // Add other buy links here if needed
}

public class Calling_Playlist : MonoBehaviour
{
    public GameObject playlistPrefab;
    public Transform contentContainer;
    public float refreshInterval = 300f;


    private Coroutine refreshCoroutine;
    private List<GameObject> createdUIElements = new List<GameObject>();
    private string apiUrl = "https://api.composer.nprstations.org/v1/widget/555341e0e1c820fc55f5436d/playlist?t=1470075372214&limit=50&order=-1&loadingMesgeId=widgetElement_704&errorMsg=Sorry%2C+there+is+no+playlist+to+display.";
    public bool isDaylightsAVING;
    private string _start_timeFormatted;
    public List<string> programnames;
    public List<string> endtime;
    public int LEGTH;

    public GameObject scrollanker;
    public GameObject scrolltether;

    private void Start()
    {
        RefreshPlaylistData();
    }

    public void onscrollchange()
    {
        if (scrollanker.transform.position.y > scrolltether.transform.position.y)
        {
            RefreshPlaylistData();
            
            Debug.Log("SUCESSFULLY");
        }
       
    }
 
    private void RefreshPlaylistData()
    {
       
        
            Debug.Log("Refreshing Playlist data...");
           StartCoroutine(FetchPlaylistData(OnPlaylistDataReceived));
          
        
    }

    private IEnumerator FetchPlaylistData(System.Action<List<PlaylistItem>> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching data: " + www.error);
                callback(null);
                yield break;
            }

            string jsonResponse = www.downloadHandler.text;

            if (string.IsNullOrEmpty(jsonResponse))
            {
                Debug.LogError("Empty JSON response.");
                callback(null);
                yield break;
            }

            try
            {
                Debug.Log("API Response: " + jsonResponse); // Log the JSON response

                // Parse JSON response
                ItemResponse itemResponse = JsonUtility.FromJson<ItemResponse>(jsonResponse);

                if (itemResponse != null && itemResponse.playlist != null && itemResponse.playlist.Count > 0)
                {

                    List<PlaylistItem> playlistItems = new List<PlaylistItem>();

                    foreach (Playlist playlist in itemResponse.playlist)
                    {


                        if (playlist.playlist != null)

                        {
                            LEGTH = playlist.playlist.Count;

                            foreach (PlaylistItem child in playlist.playlist)
                            {
                                child.program = playlist.name;
                            }

                            playlistItems.AddRange(playlist.playlist);

                            programnames.Add(playlist.name);
                            endtime.Add(playlist.end_time);
                        }


                    }


                    Debug.Log("Number of items in response: " + playlistItems.Count);

                    callback(playlistItems);


                }
                else
                {
                    Debug.LogError("Invalid or empty playlist data.");
                    callback(null);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing JSON: " + e.Message);
                callback(null);
            }
        }
    }


    private void OnPlaylistDataReceived(List<PlaylistItem> playlistData)
    {
        Debug.Log("API data received: " + (playlistData != null ? playlistData.Count.ToString() : "null") + " items");
        ClearPlaylistUI();

        if (playlistData != null)
        {
            Debug.Log("API data received: " + playlistData.Count + " items");

            // Reverse the order of the playlistData list
            playlistData.Reverse();


            foreach (PlaylistItem item in playlistData)
            {






                if (item != null)
                {
                    GameObject uiPrefab;

                    if (createdUIElements.Count > 0)
                    {
                        uiPrefab = createdUIElements[0];
                        createdUIElements.RemoveAt(0);
                    }
                    else
                    {
                        uiPrefab = Instantiate(playlistPrefab, contentContainer);
                    }

                    if (uiPrefab != null)
                    {
                        uiPrefab.SetActive(true);

                        Image albumArtImage = uiPrefab.transform.Find("Image")?.GetComponent<Image>();
                        TextMeshProUGUI startTimeText = uiPrefab.transform.Find("StartTimeText")?.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI programText = uiPrefab.transform.Find("Program/ProgramText")?.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI artistText = uiPrefab.transform.Find("Artist/ArtistText")?.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI albumText = uiPrefab.transform.Find("Album/AlbumText")?.GetComponent<TextMeshProUGUI>();

                        if (albumArtImage != null && startTimeText != null && programText != null && artistText != null && albumText != null)
                        {
                            //////////////////////////////////// Date formatting ///////////////
                            _start_timeFormatted = item._start_time;
                            string formattedStartTime = FormatDate(_start_timeFormatted);
                            Debug.Log(formattedStartTime);
                            ///////////////////////////////////////////////////////////////////


                            StartCoroutine(LoadSpriteImage(item.album_art, albumArtImage));



                            startTimeText.text = formattedStartTime + "-" + item.trackName;
                            programText.text = item.program;
                            artistText.text = item.artistName;
                            albumText.text = item.collectionName;
                        }
                        else
                        {
                            Debug.LogError("One or more UI components not found in prefab.");
                        }
                    }
                    else
                    {
                        Debug.LogError("UI prefab is null.");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Received null playlist data.");
        }
    }

    private void ClearPlaylistUI()
    {
        foreach (Transform child in contentContainer)
        {
            child.gameObject.SetActive(false);
            createdUIElements.Add(child.gameObject);
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

    public string FormatDate(string inputDate)
    {
        // Parse the input date using the correct format
        if (DateTime.TryParseExact(inputDate, "MM-dd-yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        {
            // Format the parsed date to "h:mm tt" (e.g., "4:11 PM")

            string formattedDate = parsedDate.ToString("h:mm tt");

            return formattedDate;
        }
        else
        {
            // Return an error message if parsing fails
            return "Invalid date format";
        }
    }



}
