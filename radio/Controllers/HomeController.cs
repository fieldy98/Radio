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
    public class HomeController : Controller
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
        public ActionResult Index(int? page, string searchString)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> indexview = new List<ivm>();
            var TrackArtists = db.TrackLists.Select(x => x.Artist).Distinct().OrderBy(x=>x).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                TrackArtists = db.TrackLists.Where(s => s.Artist.Contains(searchString)
                                       || s.Album.Contains(searchString)
                                       || s.Title.Contains(searchString)
                                       || s.Genre.Contains(searchString)).Select(x=>x.Artist).Distinct().ToList();

            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePageOfProducts = TrackArtists.ToPagedList(pageNumber, 24); // will only contain 25 products max because of the pageSize

            foreach (var item in onePageOfProducts)
            {
                var track = db.TrackLists.FirstOrDefault(x => x.Artist == item).Location;
                track = @"\\51-DBA\radio\music\" + track.Substring(7);
                TagLib.File file = TagLib.File.Create(track);

                ivm tl = new ivm();
                tl.artist = item;
                if (file.Tag.Pictures.Length >= 1)
                {
                    tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                }

                invm.indexview.Add(tl);
            }

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();
            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();

            ViewBag.OnePageOfProducts = onePageOfProducts;

            return View(invm);
        }

        public ActionResult Catalog(string artist, string shuffled)
        {
            //string[] files = Directory.GetFiles(@"//51-DBA\radio/Music/", "*.*", SearchOption.AllDirectories);
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();


            if (artist == "random")
            {
                var TrackArtists = db.TrackLists.ToArray();
                Shuffle(TrackArtists);
                artist = TrackArtists.FirstOrDefault().Artist;
            }

                var albums = db.TrackLists.Where(x => x.Artist == artist).GroupBy(x => x.Album).ToList();

                invm.artist = artist;

                foreach (var item in albums)
                {
                    ivm tl = new ivm();

                    tl.album = item.Key;
                    tl.artist = db.TrackLists.FirstOrDefault(x => x.Artist == artist).Artist;
                    tl.tracknumber = db.TrackLists.Where(x => x.Album == item.Key).Count();

                    var track = db.TrackLists.FirstOrDefault(x => x.Album == tl.album && x.Artist == artist).Location;
                    track = @"\\51-DBA\radio\music\" + track.Substring(7);
                    TagLib.File file = TagLib.File.Create(track);

                    if (file.Tag.Pictures.Length >= 1)
                    {
                        tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                    }

                    invm.indexview.Add(tl);
                }

                var artists = db.TrackLists.Where(x => x.Artist == artist).OrderBy(x => x.Album).ToArray();
                if (shuffled != null)
                {
                    Shuffle(artists);
                }
                foreach (var item in artists)
                {
                    player tl = new player();
                    tl.artist = item.Artist;
                    tl.album = item.Album;
                    tl.tracknumber = item.TrackNumber;
                    tl.title = item.Title;
                    tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                    tl.genre = item.Genre;
                    tl.location = item.Location;
                tl.ID = item.ID;

                    invm.StreamPlayer.Add(tl);
                }
            
            

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();

            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }

        public ActionResult Album(string artist, string album, string shuffled)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            if(album == "random")
            {
                var TrackArtists = db.TrackLists.ToArray();
                Shuffle(TrackArtists);
                artist = TrackArtists.FirstOrDefault().Artist;
                album = TrackArtists.FirstOrDefault().Album;
            }

            var songs = db.TrackLists.Where(x => x.Album == album && x.Artist == artist).OrderBy(x => x.TrackNumber).ToArray();
            var track = db.TrackLists.FirstOrDefault(x => x.Album == album && x.Artist == artist).Location;
            track = @"\\51-DBA\radio\music\" + track.Substring(7);
            TagLib.File file = TagLib.File.Create(track);

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
            var timeframe = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            invm.duration = timeframe;
            invm.Playlist = songs.Count().ToString();

            invm.artist = artist;
            invm.album = album;
            if (file.Tag.Pictures.Length >= 1)
            {
                invm.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
            }

            foreach (var item in songs)
            {
                ivm tl = new ivm();
                tl.artist = item.Artist;
                tl.album = item.Album;
                tl.tracknumber = item.TrackNumber;
                tl.title = item.Title;
                tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                tl.genre = item.Genre;
                tl.location = item.Location;
                tl.ID = item.ID;

                invm.indexview.Add(tl);
            }

            foreach (var item in songs)
            {
                player tl = new player();
                tl.artist = item.Artist;
                tl.album = item.Album;
                tl.tracknumber = item.TrackNumber;
                tl.title = item.Title;
                tl.duration = item.Duration.Substring(0, item.Duration.Length - 8);
                tl.genre = item.Genre;
                tl.location = item.Location;
                tl.ID = item.ID;

                invm.StreamPlayer.Add(tl);
            }

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();
            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }

        public ActionResult _Playcount(string ID)
        {
            IndexViewModel invm = new IndexViewModel();
            PlayCount pc = new PlayCount();

            var id = Convert.ToInt32(ID);
            var Song = db.TrackLists.FirstOrDefault(x => x.ID == id);

            if(db.PlayCounts.Any(x=>x.TrackListID == id))
            {
                PlayCount existingPlayCount = db.PlayCounts.FirstOrDefault(x => x.TrackListID == id);
                existingPlayCount.SongPlayCount = existingPlayCount.SongPlayCount + 1;
                db.SaveChanges();
            }
            else
            {
                pc.TrackListID = id;
                pc.SongPlayCount = 1;
                db.PlayCounts.Add(pc);
                db.SaveChanges();
            }


            // var song = db.
            return Json(new { success = true });
        }
        public ActionResult _History(string ID)
        {
            IndexViewModel invm = new IndexViewModel();
            History pc = new History();

            var id = Convert.ToInt32(ID);

                pc.TrackListID = id;
            pc.IsComplete = 0;
                pc.Timestamp = DateTime.Now;

                db.Histories.Add(pc);
                db.SaveChanges();


            // var song = db.
            return Json(new { success = true });
        }
        public ActionResult _UpdateHistory(string ID)
        {
            var id = Convert.ToInt32(ID);
            History update = db.Histories.OrderByDescending(x=>x.Timestamp).FirstOrDefault(x => x.TrackListID == id);

            update.IsComplete = 1;

            db.Entry(update).State = EntityState.Modified;
            db.SaveChanges();


            // var song = db.
            return Json(new { success = true });
        }

        public ActionResult Agg()
        {
            IndexViewModel invm = new IndexViewModel();
            var SongList = db.PlayCounts.OrderByDescending(x=>x.SongPlayCount).ToList();

            var thismonth = DateTime.Now.Month;
            var played = db.Histories.Where(x => x.IsComplete == 1 && x.Timestamp.Value.Month == thismonth);

            if (SongList == null)
            {

            }
            else
            {
                foreach (var item in SongList)
                {
                    ivm indexview = new ivm();
                    var song = db.TrackLists.FirstOrDefault(x => x.ID == item.TrackListID);
                    indexview.artist = song.Artist;
                    indexview.album = song.Album;
                    indexview.genre = song.Genre;
                    indexview.title = song.Title;
                    indexview.PlayCount = item.SongPlayCount;
                    if (db.Histories.Any(x=>x.TrackListID == item.TrackListID && x.IsComplete == 1))
                    {
                        indexview.LastPlayed = db.Histories.OrderByDescending(x => x.Timestamp).FirstOrDefault(x => x.TrackListID == item.TrackListID && x.IsComplete == 1).Timestamp.ToString();
                    }
                    else
                    {
                        indexview.LastPlayed = "Unknown";
                    }

                    invm.indexview.Add(indexview);
                }
            }
            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            foreach (var item in SongList)
            {
                player tl = new player();
                var songs = db.TrackLists.FirstOrDefault(x => x.ID == item.TrackListID);
                tl.artist = songs.Artist;
                tl.album = songs.Album;
                tl.tracknumber = songs.TrackNumber;
                tl.title = songs.Title;
                tl.duration = songs.Duration.Substring(0, songs.Duration.Length - 8);
                tl.genre = songs.Genre;
                tl.location = songs.Location;
                tl.ID = songs.ID;

                invm.StreamPlayer.Add(tl);
            }

            invm.PlayList = invm.PlayList.ToList();
            invm.StreamPlayer = invm.StreamPlayer.Take(25).ToList();
            invm.indexview = invm.indexview.Take(25).ToList();
            return View(invm);
        }
        public ActionResult History()
        {
            IndexViewModel invm = new IndexViewModel();
            var SongList = db.PlayCounts.OrderByDescending(x => x.SongPlayCount).ToList();

            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            var played = db.Histories.Where(x => x.IsComplete == 1).OrderBy(x=>x.Timestamp);
            var unplayed = db.Histories.Where(x => x.IsComplete == 0).OrderBy(x => x.Timestamp);

            foreach (var item in SongList)
            {
                var song = db.TrackLists.FirstOrDefault(x => x.ID == item.TrackListID);
                for (var i = 0; i < item.SongPlayCount; i++)
                {
                    string[] duration = song.Duration.Split(':', '.');
                    hours = hours + Convert.ToInt32(duration[0]);
                    minutes = minutes + Convert.ToInt32(duration[1]);
                    seconds = seconds + Convert.ToInt32(duration[2]);
                }

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
            var timeframe = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            invm.duration = timeframe;

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            foreach (var item in SongList)
            {
                genrecharts gc = new genrecharts();
                var songs = db.TrackLists.FirstOrDefault(x => x.ID == item.TrackListID);
                var playcount = 0;
                if (songs.Genre == null)
                {
                    gc.Genre = "Other";
                }
                else
                {
                    gc.Genre = songs.Genre;
                }
                for (var i = 0; i < item.SongPlayCount; i++)
                {
                    playcount++;
                }
                gc.PlayCount = playcount;
                invm.Charts.Add(gc);
            }
            foreach (var item in invm.Charts.GroupBy(x => x.Genre))
            {
                genrechart gc = new genrechart();
                gc.name = item.Key;
                gc.y = item.Sum(x=>x.PlayCount);
                invm.Chart.Add(gc);
            }
            
            foreach (var item in played)
            {
                historychart hc = new historychart();
                hc.Date = item.Timestamp.Value.ToShortDateString();

                invm.PlayHistorys.Add(hc);
            }

            foreach (var item in invm.PlayHistorys.GroupBy(x=>x.Date))
            {
                historycharts hc = new historycharts();
                hc.Date = item.Key;
                hc.Count = item.Count();

                invm.PlayHistory.Add(hc);
            }
            foreach (var item in unplayed)
            {
                unplayedchart hc = new unplayedchart();
                hc.Date = item.Timestamp.Value.ToShortDateString();

                invm.Unplayedhistories.Add(hc);
            }

            foreach (var item in invm.Unplayedhistories.GroupBy(x => x.Date))
            {
                unplayedcharts hc = new unplayedcharts();
                hc.Date = item.Key;
                hc.Count = item.Count();

                invm.Unplayedhistory.Add(hc);
            }

            invm.Chart = invm.Chart.ToList();
            invm.PlayList = invm.PlayList.ToList();
            invm.PlayHistory = invm.PlayHistory.ToList();
            invm.Unplayedhistory = invm.Unplayedhistory.ToList();
            invm.indexview = invm.indexview.ToList();
            return View(invm);
        }
    }
}