using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Calling_UpNext : MonoBehaviour
{
    public GameObject upNextPrefab; // Reference to the prefab with TMPs
    public Transform contentContainer; // Reference to the container where prefabs will be placed
    public float refreshInterval = 300f; // Refresh every 5 minutes

    private List<GameObject> createdUIElements = new List<GameObject>(); // Keep track of created UI elements

    [System.Obsolete]
    private void Start()
    {
        StartCoroutine(RefreshUpNextData());
    }

    [System.Obsolete]
    private IEnumerator RefreshUpNextData()
    {
        while (true)
        {
            Debug.Log("Refreshing Up Next data...");

            ClearUpNextUI(); // Clear old instances before refreshing

            UpdateUpNextUI();

            yield return new WaitForSeconds(refreshInterval);
        }
    }

    [System.Obsolete]
    private void UpdateUpNextUI()
    {
        Debug.Log("Updating Up Next UI...");

        StartCoroutine(APIManager_UpNext.GetUpNextData(OnUpNextDataReceived));
    }

    private void OnUpNextDataReceived(UpNextItem[] upNextData)
    {
        Debug.Log("API data received: " + upNextData.Length + " items");

        if (upNextData != null && upNextData.Length > 0)
        {
            Debug.Log("Received Up Next data with " + upNextData.Length + " items.");

            // Iterate through the data and instantiate UI prefabs
            foreach (UpNextItem item in upNextData)
            {
                GameObject clonedPrefab = Instantiate(upNextPrefab, contentContainer);
                clonedPrefab.SetActive(true);
                createdUIElements.Add(clonedPrefab); // Add to the list of created UI elements

                // Find TextMeshPro components in the prefab
                TextMeshProUGUI titleText = clonedPrefab.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeText = clonedPrefab.transform.Find("Time").GetComponent<TextMeshProUGUI>();

                // Set TMP values based on UpNext item data
                titleText.text = item.post_title;
                timeText.text = "Start: " + item.start + " - End: " + item.end;
            }
        }
        else
        {
            Debug.Log("No Up Next data available.");
        }
    }

    private void ClearUpNextUI()
    {
        // Destroy all previously created UI elements
        foreach (var uiElement in createdUIElements)
        {
            Destroy(uiElement);
        }
        createdUIElements.Clear(); // Clear the list
    }
}
