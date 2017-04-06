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
    public class BrowseController : Controller
    {
        private SongListEntities db = new SongListEntities();
        private static Random rng = new Random();
        // GET: Browse

        public ActionResult Genre()
        {
            IndexViewModel invm = new IndexViewModel();

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
        public ActionResult SubGenre(string genre, string genre2)
        {
            IndexViewModel invm = new IndexViewModel();

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            List<string> songs = new List<string>();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();

            if (genre == "metal")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("metal") || x.Genre.Contains("stoner rock") || x.Genre.Contains("doom") || x.Genre.Contains("visual kei") || x.Genre.Contains("nintendocore") || x.Genre.Contains("grindcore") || x.Genre.Contains("industrial")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "elec")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("elec")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "comedy")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("comedy")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "rock")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("rock")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "punk")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("punk") || x.Genre.Contains("hardcore")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "folk")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("folk")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "indie")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("indie")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "hip hop")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("hip hop") || x.Genre.Contains("rap") || x.Genre.Contains("hip-hop")).Select(x => x.Genre).Distinct().ToList();
            }
            else if (genre == "country")
            {
                songs = db.TrackLists.Where(x => x.Genre.Contains("country")).Select(x => x.Genre).Distinct().ToList();
            }
            else
            {
                songs = db.TrackLists.Where(x => !x.Genre.Contains("hardcore") && !x.Genre.Contains("grindcore") && !x.Genre.Contains("industrial") && !x.Genre.Contains("nintendocore") && !x.Genre.Contains("country") && !x.Genre.Contains("visual kei") && !x.Genre.Contains("hip-hop") && !x.Genre.Contains("doom") && !x.Genre.Contains("comedy") && !x.Genre.Contains("stoner rock") && !x.Genre.Contains("metal") && !x.Genre.Contains("elec") && !x.Genre.Contains("rock") && !x.Genre.Contains("punk") && !x.Genre.Contains("folk") && !x.Genre.Contains("indie") && !x.Genre.Contains("hip hop") && !x.Genre.Contains("rap")).Select(x => x.Genre).Distinct().ToList();    
            }
            foreach (var item in songs)
            {
                ivm tl = new ivm();
                player pl = new player();

                tl.genre = item;
                invm.indexview.Add(tl);
            }
            invm.indexview = invm.indexview.ToList();

            return View(invm);
        }

        public ActionResult GenreArtist(string genre, int? page, string searchString)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> indexview = new List<ivm>();
            var TrackArtists = db.TrackLists.Where(x => x.Genre == genre).Select(x => x.Artist).Distinct().ToList();

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
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
                tl.genre = genre;
                if (file.Tag.Pictures.Length >= 1)
                {
                    tl.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                }

                invm.indexview.Add(tl);
            }

            invm.genre = genre;
            invm.PlayList = invm.PlayList.ToList();
            invm.indexview = invm.indexview.ToList();
            invm.StreamPlayer = invm.StreamPlayer.ToList();

            ViewBag.OnePageOfProducts = onePageOfProducts;

            return View(invm);
        }

        public ActionResult Catalog(string artist, string shuffled, string genre)
        {
            //string[] files = Directory.GetFiles(@"//51-DBA\radio/Music/", "*.*", SearchOption.AllDirectories);
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();

            var albums = db.TrackLists.Where(x => x.Artist == artist && x.Genre == genre).GroupBy(x => x.Album).ToList();

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
    }
}
