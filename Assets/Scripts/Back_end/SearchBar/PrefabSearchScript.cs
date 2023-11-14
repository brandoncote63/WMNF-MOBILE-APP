using UnityEngine;
using TMPro;

public class PrefabSearchScript : MonoBehaviour
{
    public TMP_InputField searchInput;
    public Transform prefabContainer;

    private void Start()
    {
        // Attach a listener to the search input field to trigger filtering
        searchInput.onValueChanged.AddListener(FilterPrefabs);
    }

    private void FilterPrefabs(string searchText)
    {
        // Convert the search text to lowercase for case-insensitive search
        searchText = searchText.ToLower();

        // Iterate through all the children of the prefab container
        foreach (Transform child in prefabContainer)
        {
            // Get the TMP components in this child
            TextMeshProUGUI[] tmProComponents = child.GetComponentsInChildren<TextMeshProUGUI>();

            // Flag to determine if the prefab should be shown
            bool shouldShowPrefab = false;

            // Check if any TMP component's text contains the search text
            foreach (TextMeshProUGUI tmPro in tmProComponents)
            {
                if (tmPro.text.ToLower().Contains(searchText))
                {
                    shouldShowPrefab = true;
                    break; // If any match is found, no need to check further
                }
            }

            // Set the prefab's active state based on the filter result
            child.gameObject.SetActive(shouldShowPrefab);
        }
    }
}
