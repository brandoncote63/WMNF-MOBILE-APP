using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using WMNF_API;
using System.Collections.Generic;

public static class APIManager_Archives
{
    
    [Obsolete]
    public static IEnumerator GetArchivesData(System.Action<List<ArchiveData>> onComplete)
    {
        string url = "https://www.wmnf.org/api/programs.php?ver=20160427";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                onComplete?.Invoke(null);
            }
            else
            {
                try
                {
                    Root rootData = JsonUtility.FromJson<Root>(www.downloadHandler.text);
                    onComplete?.Invoke(rootData.data);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                    onComplete?.Invoke(null);
                }
            }
        }
    }
}
