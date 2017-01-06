using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace radio.ViewModels
{
    public class IndexViewModel
    {
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public int? tracknumber { get; set; }
        public int? modal { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string format { get; set; }
        public string location { get; set; }
        public int? ID { get; set; }
        public int? TrackListID { get; set; }
        public int? PlayCount { get; set; }
        public string Art { get; set; }
        public string Username { get;  set; }
        public string Playlist { get; set; }
        public string AddToPlaylist { get; set; }
        public List<ivm> indexview { get; set; }
        public List<player> StreamPlayer { get; set; }
        public List<playlist> PlayList { get; set; }
        public List<genrechart> Chart { get; set; }
        public List<genrecharts> Charts { get; set; }
        public IndexViewModel()
        {
            indexview = new List<ivm>();
            StreamPlayer = new List<player>();
            PlayList = new List<playlist>();
            Chart = new List<genrechart>();
            Charts = new List<genrecharts>();
        }
    }

    public class ivm
    {
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public int? tracknumber { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string format { get; set; }
        public string location { get; set; }
        public int? ID { get; set; }
        public int? PlayCount { get; set; }
        public string Art { get; set; }
        public string AddToPlaylist { get; set; }
        public string LastPlayed { get; set; }
    }
    public class player
    {
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public int? tracknumber { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string format { get; set; }
        public string location { get; set; }
        public int? ID { get; set; }
    }
    public class playlist
    {
        public string Username { get; set; }
        public string Playlist { get; set; }
    }
    public class genrechart
    {
        public string name { get; set; }
        public int? y { get; set; }
    }
    public class genrecharts
    {
        public string Genre { get; set; }
        public int? PlayCount { get; set; }
    }
}