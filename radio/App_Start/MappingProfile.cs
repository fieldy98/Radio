using AutoMapper;
using radio.Dtos;
using radio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace radio.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
            {
                Mapper.CreateMap<TrackList, SongDto>();
                Mapper.CreateMap<SongDto, TrackList>();
                Mapper.CreateMap<TrackList, ArtistDto>();
                Mapper.CreateMap<ArtistDto, TrackList>();
                Mapper.CreateMap<TrackList, AlbumDto>();
                Mapper.CreateMap<AlbumDto, TrackList>();
        }
    }
}