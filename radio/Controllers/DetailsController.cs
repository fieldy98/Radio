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
    public class DetailsController : Controller
    {
        private SongListEntities db = new SongListEntities();
        private static Random rng = new Random();

        // GET: Details
        public ActionResult Details(int? id)
        {
            IndexViewModel invm = new IndexViewModel();

            var SelectedTrack = db.TrackLists.FirstOrDefault(i => i.ID == id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var track = db.TrackLists.FirstOrDefault(x => x.ID == id).Location;
            track = @"\\51-DBA\radio\music\" + track.Substring(7);
            TagLib.File file = TagLib.File.Create(track);


            if (file.Tag.Pictures.Length >= 1)
            {
                invm.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
            }

            invm.album = SelectedTrack.Album;
            invm.artist = SelectedTrack.Artist;
            invm.duration = SelectedTrack.Duration;
            invm.genre = SelectedTrack.Genre;
            invm.ID = id;
            invm.location = SelectedTrack.Location;
            invm.title = SelectedTrack.Title;
            invm.tracknumber = SelectedTrack.TrackNumber;

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

        public ActionResult Edit(int? id)
        {
            var SelectedTrack = db.TrackLists.FirstOrDefault(i => i.ID == id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IndexViewModel invm = new IndexViewModel();
            var track = db.TrackLists.FirstOrDefault(x => x.ID == id).Location;
            track = @"\\51-DBA\radio\music\" + track.Substring(7);
            TagLib.File file = TagLib.File.Create(track);


            if (file.Tag.Pictures.Length >= 1)
            {
                invm.Art = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
            }

            invm.album = SelectedTrack.Album;
            invm.artist = SelectedTrack.Artist;
            invm.duration = SelectedTrack.Duration;
            invm.genre = SelectedTrack.Genre;
            invm.ID = id;
            invm.location = SelectedTrack.Location;
            invm.title = SelectedTrack.Title;
            invm.tracknumber = SelectedTrack.TrackNumber;

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
        public ActionResult Edit([Bind(Include = "ID,artist,duration,location,genre,album,title,tracknumber")] TrackList tracklist)
        {
            IndexViewModel invm = new IndexViewModel();
            if (ModelState.IsValid)
            {
                db.Entry(tracklist).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Album", "Home", new { artist = tracklist.Artist, album = tracklist.Album });
            }
            return View(invm);
        }

        public ActionResult Delete(int? id)
        {
            IndexViewModel invm = new IndexViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var SelectedTrack = db.TrackLists.Find(id);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrackList tracklist = db.TrackLists.Find(id);
            db.TrackLists.Remove(tracklist);
            db.SaveChanges();
            return RedirectToAction("Album", "Home", new { artist = tracklist.Artist, album = tracklist.Album });
        }
    }
}
