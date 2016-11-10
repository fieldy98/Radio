using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TagLib;
using radio.ViewModels;
using System.IO;
using System.Collections;

namespace radio.Controllers
{
    public class HomeController : Controller
    {
        private SongListEntities db = new SongListEntities();
        
        public ActionResult Index()
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> indexview = new List<ivm>();
            var TrackArtists = db.TrackLists.Select(x => x.Artist).Distinct().ToList();
            foreach(var item in TrackArtists)
            {
                ivm tl = new ivm();
                tl.artist = item;
                tl.album = db.TrackLists.Where(x => x.Artist == item).Select(x => x.Album).Distinct().Count().ToString() + " Albums";

                invm.indexview.Add(tl);
            }
            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }

        public ActionResult Upload()
        {
            string[] files = Directory.GetFiles(@"\\192.168.151.58\music", "*.*", SearchOption.AllDirectories);
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            foreach (var item in files)
            {
                TrackList index = new TrackList();
                if (item.Contains(".mp3") || item.Contains(".flac") || item.Contains(".m4a") || item.Contains(".m4p") || item.Contains(".wma") || item.Contains(".aiff") || item.Contains(".wav") || item.Contains(".alac") || 
                    item.Contains(".ogg")) //|| item.Contains(".png") || item.Contains(".jpg"))
                {

                    TagLib.File file = TagLib.File.Create(item);
                    index.Location = "/music/" + file.Name.Substring(23);
                    //index.Location = file.Name;
                    var checkentries = db.TrackLists.FirstOrDefault(x => x.Location == index.Location);
                    if (checkentries == null)
                    {
                        index.Title = file.Tag.Title;
                        index.Album = file.Tag.Album;
                        index.TrackNumber = checked((int?)file.Tag.Track);
                        index.Artist = file.Tag.FirstPerformer;
                        index.Duration = file.Properties.Duration.ToString();
                        index.Genre = file.Tag.FirstGenre;

                        db.TrackLists.Add(index);
                        db.SaveChanges();
                    }

                }
                else
                {

                }
            }
            ViewBag.Files = files.ToArray();
            invm.indexview = invm.indexview.ToList();
            return Redirect("Index");
        }

        public ActionResult Catalog(string artist)
        {
            //string[] files = Directory.GetFiles(@"//192.168.151.58/Music/", "*.*", SearchOption.AllDirectories);
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            var albums = db.TrackLists.Where(x => x.Artist == artist).GroupBy(x=>x.Album).ToList();
            invm.artist = artist;
            foreach(var item in albums)
            {
                ivm tl = new ivm();
                
                tl.album = item.Key;
                tl.artist = db.TrackLists.FirstOrDefault(x=>x.Album == item.Key).Artist;
                tl.tracknumber = db.TrackLists.Where(x => x.Album == item.Key).Count();

                invm.indexview.Add(tl);
            }

            var artists = db.TrackLists.Where(x => x.Artist == artist).OrderBy(x=>x.Album).ToList();
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

            //foreach (var item in files)
            //{
            //    TrackList index = new TrackList();
            //    if (item.Contains(".mp3") || item.Contains(".flac") || item.Contains(".m4a") || item.Contains(".m4p")) //|| item.Contains(".aiff") || item.Contains(".wav") || item.Contains(".alac") || 
            //    //    item.Contains(".ogg") || item.Contains(".png") || item.Contains(".jpg"))
            //    {

            //        TagLib.File file = TagLib.File.Create(item);
            //        index.Location = file.Name;
            //        var checkentries = db.TrackLists.FirstOrDefault(x => x.Location == index.Location);
            //        if (checkentries == null)
            //        {
            //            index.Title = file.Tag.Title;
            //            index.Album = file.Tag.Album;
            //            index.TrackNumber = checked((int?)file.Tag.Track);
            //            index.Artist = file.Tag.FirstPerformer;
            //            index.Duration = file.Properties.Duration.ToString();
            //            index.Genre = file.Tag.FirstGenre;

            //            db.TrackLists.Add(index);
            //            db.SaveChanges();
            //        }

            //    }
            //    else
            //    {

            //    }
            //}
            //ViewBag.Files = files.ToArray();
            //invm.indexview = invm.indexview.ToList();

            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }

        public ActionResult Album(string artist, string album)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            var songs = db.TrackLists.Where(x => x.Album == album && x.Artist == artist).OrderBy(x => x.TrackNumber).ToList();

            invm.artist = artist;
            invm.album = album;


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

            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return View(invm);
        }
        [HttpPost]
        public ActionResult Index(string artist, string album, int ID)
        {
            IndexViewModel invm = new IndexViewModel();
            List<player> StreamPlayer = new List<player>();
            

            if(artist != null)
            {
                var artists = db.TrackLists.Where(x=>x.Artist == artist).ToList();
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
            }
            if (album != null)
            {
                var songs = db.TrackLists.Where(x => x.Album == album).ToList();
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
            }
            else if (ID != 0)
            {
                var songs = db.TrackLists.FirstOrDefault(x => x.ID == ID);
                player tl = new player();
                tl.artist = songs.Artist;
                tl.album = songs.Album;
                tl.tracknumber = songs.TrackNumber;
                tl.title = songs.Title;
                tl.duration = songs.Duration.Substring(0, songs.Duration.Length - 8);
                tl.genre = songs.Genre;
                tl.location = songs.Location;

                invm.StreamPlayer.Add(tl);
            }
            invm.StreamPlayer = invm.StreamPlayer.ToList();
            return Json(new { success = true});
        }
    }
}