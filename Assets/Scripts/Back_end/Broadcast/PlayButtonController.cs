using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayButtonController : MonoBehaviour
{
    public string m3uUrl;
    public AudioSource audioSource; // Reference to the AudioSource component

    public void PlayAudioFromM3U()
    {
        Debug.Log("Play button clicked.");
        StartCoroutine(LoadAndPlayAudio());
    }

    private IEnumerator LoadAndPlayAudio()
    {
        Debug.Log("Loading M3U file...");

        using (UnityWebRequest www = UnityWebRequest.Get(m3uUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching M3U file: " + www.error);
            }
            else
            {
                Debug.Log("M3U file loaded successfully.");

                string m3uContent = www.downloadHandler.text;
                string[] audioClipUrls = ParseM3UContent(m3uContent);

                if (audioClipUrls != null && audioClipUrls.Length > 0)
                {
                    Debug.Log("Audio clip URLs extracted: " + string.Join(", ", audioClipUrls));

                    // Load and play the audio clips
                    foreach (string audioClipUrl in audioClipUrls)
                    {
                        StartCoroutine(PlayAudioClip(audioClipUrl));
                        yield return new WaitForSeconds(1f); // Add a delay between playing audio clips
                    }
                }
                else
                {
                    Debug.LogError("No audio clip URLs found in M3U file.");
                }
            }
        }
    }

    private IEnumerator PlayAudioClip(string audioUrl)
    {
        audioUrl = audioUrl.Trim(); // Trim leading and trailing whitespace

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.UNKNOWN))
        {
            Debug.Log("Loading audio clip from URL: " + audioUrl); // Print the URL being used

            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("Error loading audio clip: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip; // Set the clip for the AudioSource
                audioSource.Play(); // Play the audio clip using the AudioSource

                Debug.Log("Audio clip is now playing. Clip duration: " + clip.length + " seconds");

                // Wait until the audio finishes playing
                yield return new WaitForSeconds(clip.length);
            }
        }
    }

    private string[] ParseM3UContent(string m3uContent)
    {
        string[] lines = m3uContent.Split('\n');
        List<string> audioClipUrls = new List<string>();

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            {
                audioClipUrls.Add(line.Trim());
            }
        }

        return audioClipUrls.ToArray();
    }
}
