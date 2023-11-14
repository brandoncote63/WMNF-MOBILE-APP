using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class APIManager_UpNext
{
    [System.Obsolete]
    public static IEnumerator GetUpNextData(System.Action<UpNextItem[]> onComplete)
    {
        string url = "https://www.wmnf.org/api/upcoming.php";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error fetching Up Next data: " + www.error);
                onComplete?.Invoke(null);
            }
            else
            {
                string responseData = www.downloadHandler.text;
                Debug.Log("API response data: " + responseData);

                UpNextResponse response = JsonUtility.FromJson<UpNextResponse>(responseData);

                if (response != null && response.status == "success")
                {
                    Debug.Log("Up Next data parsed successfully.");

                    if (response.data != null && response.data.Length > 0)
                    {
                        foreach (UpNextItem item in response.data)
                        {
                            Debug.Log("Up Next Item: " + JsonUtility.ToJson(item));
                        }
                    }
                    else
                    {
                        Debug.Log("No Up Next items found.");
                    }

                    onComplete?.Invoke(response.data);
                }
                else
                {
                    Debug.LogError("Error parsing Up Next data.");
                    onComplete?.Invoke(null);
                }
            }
        }
    }
}
