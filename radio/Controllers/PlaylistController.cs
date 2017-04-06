using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TagLib;
using radio.ViewModels;
using System.IO;
using System.Collections;
using System.Net;
using System.Data.Entity;
using System.Data;
using System.Security.Cryptography;
using PagedList;

namespace radio.Controllers
{
    public class PlaylistController : Controller
    {
        private SongListEntities db = new SongListEntities();
        private static Random rng = new Random();

        static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(rng.NextDouble() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        // GET: Playlist
        public ActionResult Playlist(string playlist, string shuffled)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            var thisweek = DateTime.Now.AddDays(-6);

                var songs = db.PlaylistNames.Where(x => x.PlaylistName1 == playlist).ToArray();

                if (shuffled != null)
                {
                    Shuffle(songs);
                }


                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in songs)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = songs.Count();



                var c = 1;
                foreach (var item in songs)
                {
                    ivm tl = new ivm();

                    var track = db.PlaylistNames.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                pl.ID = item.TL_ID;

                invm.StreamPlayer.Add(pl);

                    c++;
                }
            

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.Playlist = playlist;
            invm.PlayList = invm.PlayList.ToList();

            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }

        // GET: Playlist
        public ActionResult RandomPlaylist(string playlist, string basedOn, string shuffled)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            var thisweek = DateTime.Now.AddDays(-6);
            var RejectedSongs = db.TrackLists.Where(x => x.Title.Contains("interview") || x.Title.Contains("interlude") || x.Title.Contains("intro") || x.Title.Contains("outro") || x.Title.Contains("skit"));
            if (playlist == "thisweek")
            {
                var songs = db.TrackLists.Where(x => x.TimeAdded >= thisweek).ToArray();

                if (shuffled != null)
                {
                    Shuffle(songs);
                }


                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in songs)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = songs.Count();



                var c = 1;
                foreach (var item in songs)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "metal")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("metal") || x.Genre.Contains("stoner rock") || x.Genre.Contains("doom") || x.Genre.Contains("visual kei") || x.Genre.Contains("nintendocore") || x.Genre.Contains("grindcore") || x.Genre.Contains("industrial")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "electornic")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("elec")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "comedy")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("comedy")).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "rock")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("rock")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "punk")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("punk")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "folk")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("folk")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "indie")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("indie")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "hiphop")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("hip hop") || x.Genre.Contains("rap") || x.Genre.Contains("hip-hop")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "country")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("country")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;


                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "christmas")
            {
                var songs = db.TrackLists.Where(x => x.Album.Contains("christmas") || x.Title.Contains("christmas") || x.Album.Contains("Oi to the World") || x.Title.Contains("oi to the world")).Except(RejectedSongs).ToArray();
                Shuffle(songs);
                var tracks = songs.Take(15);

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();



                var c = 1;
                foreach (var item in tracks)
                {
                    ivm tl = new ivm();

                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;


                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else if (playlist == "predict")
            {
                var genre = db.PlayedSongs.Where(x=>x.Genre != null).GroupBy(x=>x.Genre).OrderByDescending(x=>x.Sum(y=>y.SongPlayCount)).Take(5);

                foreach (var item in genre)
                {
                    genrecharts gc = new genrecharts();
                    gc.Genre = item.Key;
                    gc.PlayCount = item.Sum(x => x.SongPlayCount);

                    invm.Charts.Add(gc);
                }

                var genres = invm.Charts.ToArray();

                var count = 5;
                for (var i = 0; i < 5; i++)
                {
                    var predict = genres[i].Genre;
                    var test = db.TrackLists.Where(x => x.Genre == predict).ToArray();
                    Shuffle(test);
                    var shuffledSongs = test.Take(count);
                    foreach(var s in shuffledSongs)
                    {
                        ivm t = new ivm();
                        var track = db.TrackLists.FirstOrDefault(x => x.ID == s.ID).Location;
                        track = @"\\51-DBA\radio\music\" + track.Substring(7);
                        TagLib.File file = TagLib.File.Create(track);

                        if (file.Tag.Pictures.Length >= 1)
                        {
                            t.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                        }

                        t.artist = s.Artist;
                        t.album = s.Album;
                        t.title = s.Title;
                        t.duration = s.Duration.Substring(0, s.Duration.Length - 8);
                        t.genre = s.Genre;
                        t.location = s.Location;
                        t.ID = s.ID;

                        invm.indexview.Add(t);

                        
                    }

                    count--;
                }

                var favartist = db.PlayedSongs.OrderByDescending(x => x.SongPlayCount).First();
                var fav = db.TrackLists.Where(x => x.Artist == favartist.Artist && x.ID != favartist.ID).ToArray();
                Shuffle(fav);
                var topartist = fav.Take(1);

                foreach (var item in topartist)
                {
                    ivm t = new ivm();
                    var track = db.TrackLists.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        t.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }

                    t.artist = item.Artist;
                    t.album = item.Album;
                    t.title = item.Title;
                    t.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    t.genre = item.Genre;
                    t.location = item.Location;
                    t.ID = item.ID;

                    invm.indexview.Add(t);
                }

                var songs = invm.indexview.ToArray();
                Shuffle(songs);
                var tracks = songs;

                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in tracks)
                {
                    string[] duration = item.duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = tracks.Count();

                var place = 1;
                foreach (var item in tracks)
                {
                    if (item.tracknumber == null)
                    {
                        item.tracknumber = place;
                    }
                    place++;
                }

                var c = 1;
                foreach (var item in tracks)
                {
                    player pl = new player();
                    pl.artist = item.artist;
                    pl.album = item.album;
                    pl.tracknumber = item.tracknumber;
                    pl.title = item.title;
                    pl.duration = item.duration.Substring(0, item.duration.Length - 8);
                    pl.genre = item.genre;
                    pl.location = item.location;
                    pl.ID = item.ID;

                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }
            else
            {
                var songs = db.PlaylistNames.Where(x => x.PlaylistName1 == playlist).ToArray();

                if (shuffled != null)
                {
                    Shuffle(songs);
                }


                var hours = 0;
                var minutes = 0;
                var seconds = 0;

                foreach (var item in songs)
                {
                    string[] duration = item.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }
                for (var i = seconds; i >= 60; i = i - 60)
                {
                    seconds = seconds - 60;
                    minutes = minutes + 1;
                }
                for (var i = minutes; i >= 60; i = i - 60)
                {
                    minutes = minutes - 60;
                    hours = hours + 1;
                }
                var timeframe = hours.ToString() + " hours " + minutes.ToString() + " minutes " + seconds.ToString() + " seconds";
                invm.duration = timeframe;
                invm.tracknumber = songs.Count();



                var c = 1;
                foreach (var item in songs)
                {
                    ivm tl = new ivm();

                    var track = db.PlaylistNames.FirstOrDefault(x => x.ID == item.ID).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }


                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                    tl.ID = item.ID;
                    tl.tracknumber = c;

                    invm.indexview.Add(tl);

                    player pl = new player();
                    pl.artist = item.Artist;
                    pl.album = item.Album;
                    pl.tracknumber = item.TrackNumber;
                    pl.title = item.Title;
                    pl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    pl.genre = item.Genre;
                    pl.location = item.Location;
                    pl.ID = item.ID;


                    invm.StreamPlayer.Add(pl);

                    c++;
                }
            }

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.Playlist = playlist;
            invm.PlayList = invm.PlayList.ToList();

            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }

        public ActionResult PlaylistRemove(int? id)
        {
            IndexViewModel invm = new IndexViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var SelectedTrack = db.PlaylistNames.Find(id);
            if (SelectedTrack == null)
            {
                return HttpNotFound();
            }

            invm.album = SelectedTrack.Album;
            invm.artist = SelectedTrack.Artist;
            invm.duration = SelectedTrack.Duration;
            invm.genre = SelectedTrack.Genre;
            invm.ID = id;
            invm.location = SelectedTrack.Location;
            invm.title = SelectedTrack.Title;
            invm.tracknumber = SelectedTrack.TrackNumber;
            invm.Playlist = SelectedTrack.PlaylistName1;

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();

            return View(invm);
        }
        // POST: Teacher/Delete/5
        [HttpPost, ActionName("PlaylistRemove")]
        [ValidateAntiForgeryToken]
        public ActionResult PlaylistRemove(int id)
        {
            PlaylistName tracklist = db.PlaylistNames.Find(id);

            db.PlaylistNames.Remove(tracklist);
            db.SaveChanges();
            return RedirectToAction("Playlist", new { playlist = tracklist.PlaylistName1 });
        }

        public ActionResult _AddToPlaylist(int? id, string album, string artist, int? modalID)
        {
            var SelectedTrack = db.TrackLists.FirstOrDefault(i => i.ID == id);

            if (album != null || artist != null)
            {
                SelectedTrack = db.TrackLists.FirstOrDefault(i => i.Artist == artist && i.Album == album);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IndexViewModel invm = new IndexViewModel();

            invm.album = SelectedTrack.Album;
            invm.artist = SelectedTrack.Artist;
            invm.duration = SelectedTrack.Duration;
            invm.genre = SelectedTrack.Genre;
            invm.ID = id;
            invm.location = SelectedTrack.Location;
            invm.title = SelectedTrack.Title;
            invm.tracknumber = SelectedTrack.TrackNumber;
            invm.Art = SelectedTrack.Art;
            invm.modal = modalID;
            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();

            return PartialView(invm);
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PlaylistAdd(string playlist, int? id)
        {
            IndexViewModel invm = new IndexViewModel();
            PlaylistName pln = new PlaylistName();

            var tracklist = db.TrackLists.FirstOrDefault(x => x.ID == id);

                pln.Album = tracklist.Album;
                pln.Artist = tracklist.Artist;
                pln.Duration = tracklist.Duration;
                pln.Genre = tracklist.Genre;
                pln.Location = tracklist.Location;
                pln.PlaylistName1 = playlist;
                pln.Title = tracklist.Title;
                pln.TL_ID = tracklist.ID;
                pln.TrackNumber = tracklist.TrackNumber;
                pln.Username = User.Identity.Name;

                db.PlaylistNames.Add(pln);
                db.SaveChanges();

                return Json(new { success = true});
            

        }
    }
}
