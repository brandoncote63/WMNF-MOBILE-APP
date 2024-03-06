using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Analytics;
using UnityEngine;


public class firerbaseSetup : MonoBehaviour
{
    private Firebase.FirebaseApp app;

    // Start is called before the first frame update
    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        
        startapp();
    }

   public void startapp()
    {
       
        
       
        FirebaseAnalytics.LogEvent("session_start");
        
      
        
    }

    public void archivespageview()
    {
        FirebaseAnalytics.LogEvent("Archives_page_view");

    }
    public void Newspageview()
    {
        FirebaseAnalytics.LogEvent("News_page_view");

    }
    public void Evventspageview()
    {
        FirebaseAnalytics.LogEvent("Events_page_view");

    }
    public void schedulepageview()
    {
        FirebaseAnalytics.LogEvent("Schedule_page_view");

    }
    public void supportpageview()
    {
        FirebaseAnalytics.LogEvent("Support_page_view");

    }
    public void callDJClicked()
    {
        FirebaseAnalytics.LogEvent("Call_DJ_click");

    }
    public void EmailClicked()
    {
        FirebaseAnalytics.LogEvent("Email_click");

    }
    public void donateClicked()
    {
        FirebaseAnalytics.LogEvent("Donate_click");

    }
    
    public void Playlistpageview()
    {
        FirebaseAnalytics.LogEvent("Playlist_page_view");

    }
    public void UpNextpageview()
    {
        FirebaseAnalytics.LogEvent("UpNext_page_view");

    }
    public void StreamListenHD(string I)
    {
        FirebaseAnalytics.LogEvent("Stream_Listen", new Parameter("WMNF_HD",I));

    }
    public void Listenpageview()
    {
        FirebaseAnalytics.LogEvent("Listen_page_view");

    }

    private void OnDestroy()
    {
        FirebaseAnalytics.LogEvent("session_end");
    }

}
