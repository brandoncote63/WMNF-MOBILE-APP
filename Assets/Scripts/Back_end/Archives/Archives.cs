using System.Collections.Generic;
using System;


namespace WMNF_API
{
    [Serializable]
    public class ArchiveData
    {
        public int id;
        public string title;

  
        public string imagethumb;
        public string content;
        public List<string> host;
        public List<Schedule> schedule;
        public List<Playlist> playlist;
    }

    [Serializable]
    public class Playlist
    {
        public string name;
        public List<Part> data;
    }

    [Serializable]
    public class Part
    {
        public string file;
        public int length;
        public string title;
        
    }

    [Serializable]
    public class Schedule
    {
        public string day;
        public string start;
        public string end;
        
    }

    [Serializable]
    public class Root
    {
        public string status;
        public List<ArchiveData> data;
    }

}
