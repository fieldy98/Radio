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
        public string longDuration { get; set; }
        public int NumberOfSongs { get; set; }
        public string format { get; set; }
        public string location { get; set; }
        public int? ID { get; set; }
        public int? TrackListID { get; set; }
        public int? PlayCount { get; set; }
        public string Art { get; set; }
        public string Username { get; set; }
        public string Playlist { get; set; }
        public string AddToPlaylist { get; set; }
        public string Skipped { get; set; }
        public int Skip { get; set; }
        public List<ivm> indexview { get; set; }
        public List<player> StreamPlayer { get; set; }
        public List<playlist> PlayList { get; set; }
        public List<genrechart> Chart { get; set; }
        public List<genrecharts> Charts { get; set; }
        public List<historycharts> PlayHistory { get; set; }
        public List<historychart> PlayHistorys { get; set; }
        public List<unplayedchart> Unplayedhistories { get; set; }
        public List<unplayedcharts> Unplayedhistory { get; set; }
        public List<fa> featuredArtist { get; set; }
        public List<topGenres> Genres { get; set; }
        public List<FinishedAlbums> finishedalbums { get; set; }
        public List<MostPlayed> mostplayed { get; set; }
        public List<LongestSongPlayed> longestsongplayed { get; set; }
        public List<MostSkipped> mostskipped { get; set; }
        public List<AlbumsByArtist> albumbyartist { get; set; }
        public List<AlbumsByGenre> albumbygenre { get; set; }
        public IndexViewModel()
        {
            indexview = new List<ivm>();
            featuredArtist = new List<fa>();
            StreamPlayer = new List<player>();
            PlayList = new List<playlist>();
            Chart = new List<genrechart>();
            Charts = new List<genrecharts>();
            PlayHistorys = new List<historychart>();
            PlayHistory = new List<historycharts>();
            Unplayedhistories = new List<unplayedchart>();
            Unplayedhistory = new List<unplayedcharts>();
            Genres = new List<topGenres>();
            finishedalbums = new List<FinishedAlbums>();
            mostplayed = new List<MostPlayed>();
            longestsongplayed = new List<LongestSongPlayed>();
            mostskipped = new List<MostSkipped>();
            albumbyartist = new List<AlbumsByArtist>();
            albumbygenre = new List<AlbumsByGenre>();
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
    public class fa
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
    public class historychart
    {
        public string Date { get; set; }
        public int? Count { get; set; }
    }
    public class historycharts
    {
        public string Date { get; set; }
        public int? Count { get; set; }
    }
    public class unplayedchart
    {
        public string Date { get; set; }
        public int? Count { get; set; }
    }
    public class unplayedcharts
    {
        public string Date { get; set; }
        public int? Count { get; set; }
    }
    public class topGenres
    {
        public string Genre { get; set; }
        public int? PlayCount { get; set; }
    }
    public class FinishedAlbums
    {
        public string Album { get; set; }
        public string Artist { get; set; }
        public int? Plays { get; set; }
    }
    public class MostPlayed
    {
        public string Artist { get; set; }
        public int? Plays { get; set; }
    }
    public class LongestSongPlayed
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Duration { get; set; }
    }
    public class MostSkipped
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public int? Skips { get; set; }
    }
    public class AlbumsByArtist
    {
        public string Artist { get; set; }
        public int? Plays { get; set; }
    }
    public class AlbumsByGenre
    {
        public string Genre { get; set; }
        public int? Plays { get; set; }
    }
}