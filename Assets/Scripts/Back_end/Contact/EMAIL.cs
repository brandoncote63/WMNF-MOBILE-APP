using UnityEngine;
using UnityEngine.Networking;

public class EMAIL : MonoBehaviour
{
    public string recipient = "dj@wmnf.org";

    public string subject = "Hello DJ!";

    public string body = "Delete this and write your email here!";

    public void OpenDefaultEmailApp()
    {
        string url = "mailto:" + recipient + "?subject=" + UnityWebRequest.EscapeURL(subject) + "&body=" + UnityWebRequest.EscapeURL(body);

        Application.OpenURL(url);
    }
}
