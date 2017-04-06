using AutoMapper;
using Microsoft.Ajax.Utilities;
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
    public class ArtistsController : ApiController
    {
        private SongListEntities db = new SongListEntities();

        public IEnumerable<ArtistDto> GetArtists()
        {
            var artists = db.TrackLists.ToList().Select(Mapper.Map<TrackList, ArtistDto>).DistinctBy(x => x.Artist);
            return artists;
        }
        public IEnumerable<AlbumDto> GetArtist(string artist)
        {
            var catalog = db.TrackLists.Where(x => x.Artist == artist).ToList().Select(Mapper.Map<TrackList, AlbumDto>).DistinctBy(x=>x.Album);
            if (catalog == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return catalog;
        }
    }
}
