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

        public ActionResult AddToPlaylist(int? id, string album, string artist)
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

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToPlaylist([Bind(Include = "ID,artist,duration,location,genre,album,title,tracknumber,art")] TrackList tracklist)
        {
            IndexViewModel invm = new IndexViewModel();
            PlaylistName pln = new PlaylistName();
            if (ModelState.IsValid)
            {
                pln.Album = tracklist.Album;
                pln.Artist = tracklist.Artist;
                pln.Duration = tracklist.Duration;
                pln.Genre = tracklist.Genre;
                pln.Location = tracklist.Location;
                pln.PlaylistName1 = tracklist.Art;
                pln.Title = tracklist.Title;
                pln.TL_ID = tracklist.ID;
                pln.TrackNumber = tracklist.TrackNumber;
                pln.Username = User.Identity.Name;

                db.PlaylistNames.Add(pln);
                db.SaveChanges();

                return RedirectToAction("Album", "Home", new { artist = tracklist.Artist, album = tracklist.Album });
            }
            return View(invm);
        }
    }
}
