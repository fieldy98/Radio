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

                invm.indexview.Add(tl);
            }

            invm.indexview = invm.indexview.ToList();
            return View(invm);
        }

        public ActionResult _Catalog(string artist)
        {
            //string[] files = Directory.GetFiles(@"\\192.168.151.58\Music\", "*.*", SearchOption.AllDirectories);
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            var albums = db.TrackLists.Where(x => x.Artist == artist).GroupBy(x=>x.Album).ToList();

            foreach(var item in albums)
            {
                ivm tl = new ivm();
                
                tl.album = item.Key;
                tl.artist = db.TrackLists.FirstOrDefault(x=>x.Album == item.Key).Artist;

                invm.indexview.Add(tl);
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
            return PartialView(invm);
        }

        public ActionResult _Album(string artist, string album)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            var songs = db.TrackLists.Where(x => x.Artist == artist && x.Album == album).ToList();

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

                invm.indexview.Add(tl);
            }

            invm.indexview = invm.indexview.ToList();
            return View(invm);
        }
    }
}