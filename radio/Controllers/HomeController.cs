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
            var TrackArtists = db.TrackLists.Select(x => x.Artist).Distinct().ToList();

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
                track = @"\\192.168.151.58\music\" + track.Substring(7);
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
                    var checkentries = db.TrackLists.FirstOrDefault(x => x.Location == index.Location);
                    if (checkentries == null)
                    {
                        index.Title = file.Tag.Title;
                        index.Album = file.Tag.Album;
                        index.TrackNumber = checked((int?)file.Tag.Track);
                        index.Artist = file.Tag.FirstPerformer;
                        index.Duration = file.Properties.Duration.ToString();
                        index.Genre = file.Tag.FirstGenre;
                        //if (file.Tag.Pictures.Length >= 1)
                        //{
                        //    index.Covers = Convert.ToBase64String(file.Tag.Pictures[0].Data.Data);
                        //}

                        db.TrackLists.Add(index);
                        db.SaveChanges();
                    }

                }
                else
                {

                }
            }

            var sidebar = db.PlaylistNames.Select(x => x.PlaylistName1).Distinct().ToList();
            foreach (var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }

            invm.PlayList = invm.PlayList.ToList();

            ViewBag.Files = files.ToArray();
            invm.indexview = invm.indexview.ToList();
            return Redirect("Index");
        }

        public ActionResult Details(int? id)
        {
            IndexViewModel invm = new IndexViewModel();

            var SelectedTrack = db.TrackLists.FirstOrDefault(i => i.ID == id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var track = db.TrackLists.FirstOrDefault(x => x.ID == id).Location;
            track = @"\\192.168.151.58\music\" + track.Substring(7);
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
            track = @"\\192.168.151.58\music\" + track.Substring(7);
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

                return RedirectToAction("Album", new { artist = tracklist.Artist, album = tracklist.Album });
            }
            return View(invm);
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

                return RedirectToAction("Album", new { artist = tracklist.Artist, album = tracklist.Album });
            }
            return View(invm);
        }






        public ActionResult Catalog(string artist, string shuffled)
        {
            //string[] files = Directory.GetFiles(@"//192.168.151.58/Music/", "*.*", SearchOption.AllDirectories);
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
                track = @"\\192.168.151.58\music\" + track.Substring(7);
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
            return RedirectToAction("Album", new { artist = tracklist.Artist, album = tracklist.Album });
        }





        public ActionResult Album(string artist, string album, string shuffled)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            var songs = db.TrackLists.Where(x => x.Album == album && x.Artist == artist).OrderBy(x => x.TrackNumber).ToArray();
            var track = db.TrackLists.FirstOrDefault(x => x.Album == album && x.Artist == artist).Location;
            track = @"\\192.168.151.58\music\" + track.Substring(7);
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


        public ActionResult Playlist(string playlist, string shuffled)
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            List<player> steam = new List<player>();
            var songs = db.PlaylistNames.Where(x=>x.PlaylistName1 == playlist).ToArray();
            if (shuffled != null)
            {
                Shuffle(songs);
            }
            

            var hours = 0;
            var minutes = 0;
            var seconds = 0;
     
            foreach (var item in songs)
            {
                string[] duration = item.Duration.Split(':','.');
                hours = hours + Convert.ToInt32(duration[0]);
                minutes = minutes + Convert.ToInt32(duration[1]);
                seconds = seconds + Convert.ToInt32(duration[2]);
            }
            for(var i = seconds; i >= 60; i = i - 60)
            {
                seconds = seconds - 60;
                minutes = minutes + 1;
            }
            for(var i = minutes; i >= 60; i = i - 60)
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
                track = @"\\192.168.151.58\music\" + track.Substring(7);
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

        public ActionResult _Sidebar()
        {
            IndexViewModel invm = new IndexViewModel();
            var sidebar = db.PlaylistNames.Where(x=>x.Username == User.Identity.Name).Select(x => x.PlaylistName1).Distinct().ToList();
            foreach(var item in sidebar)
            {
                playlist pl = new playlist();
                pl.Playlist = item;

                invm.PlayList.Add(pl);
            }
            invm.PlayList = invm.PlayList.ToList();
            return PartialView(invm);
        }

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
                var songs = db.TrackLists.Where(x => x.Genre.Contains("metal") || x.Genre.Contains("stoner rock") || x.Genre.Contains("doom") || x.Genre.Contains("visual kei") || x.Genre.Contains("nintendocore") || x.Genre.Contains("grindcore") || x.Genre.Contains("industrial")).Select(x=>x.Genre).Distinct().ToList();
                foreach(var item in songs)
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
                var songs = db.TrackLists.Where(x => x.Genre.Contains("elec")).Select(x=>x.Genre).Distinct().ToList();
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
                var songs = db.TrackLists.Where(x => !x.Genre.Contains("hardcore") && !x.Genre.Contains("grindcore") && !x.Genre.Contains("industrial") && !x.Genre.Contains("nintendocore") && !x.Genre.Contains("country") && !x.Genre.Contains("visual kei") && !x.Genre.Contains("hip-hop") &&!x.Genre.Contains("doom") && !x.Genre.Contains("comedy") && !x.Genre.Contains("stoner rock") && !x.Genre.Contains("metal") && !x.Genre.Contains("elec") && !x.Genre.Contains("rock") && !x.Genre.Contains("punk") && !x.Genre.Contains("folk") && !x.Genre.Contains("indie") && !x.Genre.Contains("hip hop") && !x.Genre.Contains("rap")).Select(x => x.Genre).Distinct().ToList();
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
            var TrackArtists = db.TrackLists.Where(x=>x.Genre == genre).Select(x => x.Artist).Distinct().ToList();
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
    }
}