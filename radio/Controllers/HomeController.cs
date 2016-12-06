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
            var albums = db.TrackLists.Where(x => x.Artist == artist).GroupBy(x=>x.Album).ToList();
            invm.artist = artist;

            foreach (var item in albums)
            {
                ivm tl = new ivm();
                
                tl.album = item.Key;
                tl.artist = db.TrackLists.FirstOrDefault(x=>x.Artist == artist).Artist;
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

            var artists = db.TrackLists.Where(x => x.Artist == artist).OrderBy(x=>x.Album).ToArray();
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
    }
}