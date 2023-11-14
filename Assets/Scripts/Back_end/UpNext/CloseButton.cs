using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    public GameObject uiToActivate;
    public GameObject uiToDeactivate;

    public void OnCloseButtonClick()
    {
        if (uiToActivate != null)
        {
            uiToActivate.SetActive(true);
        }

        if (uiToDeactivate != null)
        {
            uiToDeactivate.SetActive(false);
        }
    }
}
