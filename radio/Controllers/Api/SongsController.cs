using AutoMapper;
using radio.Dtos;
using radio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace radio.Controllers.Api
{
    public class SongsController : ApiController
    {
        private SongListEntities db = new SongListEntities();

        public IEnumerable<SongDto> GetSongs()
        {

            return db.TrackLists.ToList().Select(Mapper.Map<TrackList, SongDto>);
        }
        public SongDto GetSong(int id)
        {
            var song = db.TrackLists.SingleOrDefault(x => x.ID == id);
            if (song == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return Mapper.Map<TrackList, SongDto>(song);
        }
        public IEnumerable<SongDto> GetSongs(string artist)
        {
            var catalog = db.TrackLists.Where(x => x.Artist == artist).ToList().Select(Mapper.Map<TrackList, SongDto>);
            if (catalog == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return catalog;
        }
        public IEnumerable<SongDto> GetSongs(string artist, string album)
        {
            var albums = db.TrackLists.Where(x => x.Artist == artist && x.Album == album).ToList().Select(Mapper.Map<TrackList, SongDto>);
            if (albums == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return albums;
        }
    }
}
