[System.Serializable]
public class UpNextResponse
{
    public string status;
    public UpNextItem[] data;
}

[System.Serializable]
public class UpNextItem
{
    public string start;
    public string end;
    public string ID;
    public string description1;
    public string description2;
    public string post_title;
    public string post_name;
    public string weekday;
    public bool current_event;
}
