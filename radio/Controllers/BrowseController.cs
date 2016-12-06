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
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();

            if (genre == "metal")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("metal") || x.Genre.Contains("stoner rock") || x.Genre.Contains("doom") || x.Genre.Contains("visual kei") || x.Genre.Contains("nintendocore") || x.Genre.Contains("grindcore") || x.Genre.Contains("industrial")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "elec")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("elec")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "comedy")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("comedy")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "rock")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("rock")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "punk")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("punk") || x.Genre.Contains("hardcore")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "folk")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("folk")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "indie")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("indie")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "hip hop")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("hip hop") || x.Genre.Contains("rap") || x.Genre.Contains("hip-hop")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else if (genre == "country")
            {
                var songs = db.TrackLists.Where(x => x.Genre.Contains("country")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }
            else
            {
                var songs = db.TrackLists.Where(x => !x.Genre.Contains("hardcore") && !x.Genre.Contains("grindcore") && !x.Genre.Contains("industrial") && !x.Genre.Contains("nintendocore") && !x.Genre.Contains("country") && !x.Genre.Contains("visual kei") && !x.Genre.Contains("hip-hop") && !x.Genre.Contains("doom") && !x.Genre.Contains("comedy") && !x.Genre.Contains("stoner rock") && !x.Genre.Contains("metal") && !x.Genre.Contains("elec") && !x.Genre.Contains("rock") && !x.Genre.Contains("punk") && !x.Genre.Contains("folk") && !x.Genre.Contains("indie") && !x.Genre.Contains("hip hop") && !x.Genre.Contains("rap")).Select(x => x.Genre).Distinct().ToList();
                foreach (var item in songs)
                {
                    ivm tl = new ivm();
                    player pl = new player();

                    tl.genre = item;
                    invm.indexview.Add(tl);
                }
                invm.indexview = invm.indexview.ToList();
            }

            return View(invm);
        }

        public ActionResult GenreArtist(string genre)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> indexview = new List<ivm>();
            var TrackArtists = db.TrackLists.Where(x => x.Genre == genre).Select(x => x.Artist).Distinct().ToList();
            foreach (var item in TrackArtists)
            {
                ivm tl = new ivm();
                tl.artist = item;
                tl.album = db.TrackLists.Where(x => x.Artist == item).Select(x => x.Album).Distinct().Count().ToString() + " Albums";

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
            return View(invm);
        }
    }
}
