using UnityEngine;

public class callDJ : MonoBehaviour
{
    public string phoneNumber = "8132399663";

    public void CallPhoneNumber()
    {
        string url = "tel:" + phoneNumber;

        Application.OpenURL(url);
    }
}
