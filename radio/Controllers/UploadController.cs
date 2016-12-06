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
    public class UploadController : Controller
    {
        private SongListEntities db = new SongListEntities();
        private static Random rng = new Random();

        // GET: Upload
        public ActionResult Upload()
        {
            IndexViewModel invm = new IndexViewModel();
            List<ivm> ivm = new List<ivm>();
            DateTime from_date = DateTime.Now.AddDays(-1);
            DateTime to_date = DateTime.Now;
            List<String> todaysFiles = new List<String>();

            foreach (String file in Directory.GetDirectories(@"\\51-DBA\radio\Music\", "*.*", SearchOption.AllDirectories))
            {
                DirectoryInfo di = new DirectoryInfo(file);
                if (di.LastWriteTime >= from_date)
                {
                    todaysFiles.Add(file);
                }
            }

            foreach (var item in todaysFiles)
            {
                string[] files = Directory.GetFiles(@"\\" + item, "*.*", SearchOption.AllDirectories).ToArray();
                foreach (var song in files)
                {
                    TrackList index = new TrackList();
                    if (song.Contains(".mp3") || song.Contains(".flac") || song.Contains(".m4a") || song.Contains(".m4p") || song.Contains(".wma") || song.Contains(".aiff") || song.Contains(".wav") || song.Contains(".alac") ||
                        song.Contains(".ogg")) //|| item.Contains(".png") || item.Contains(".jpg"))
                    {
                        TagLib.File file = TagLib.File.Create(song);
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
                            index.TimeAdded = DateTime.Now;
                            db.TrackLists.Add(index);
                            db.SaveChanges();
                        }
                    }
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
            invm.indexview = invm.indexview.ToList();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UploadArt(int? id)
        {

            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];
                pic.SaveAs(@"E:\radio\image\" + id + ".png");
            }

            var song = db.TrackLists.FirstOrDefault(x => x.ID == id).Location;
            song = @"E:\radio\music\" + song.Substring(7);
            TagLib.File tagFile = TagLib.File.Create(song);
            IPicture newArt = new Picture(@"E:\radio\image\" + id + ".png");
            tagFile.Tag.Pictures = new IPicture[1] { newArt };
            tagFile.Save();

            System.IO.File.Delete(@"E:\radio\music\" + id + ".png");
            return Json(new { success = true });
        }
    }
}
