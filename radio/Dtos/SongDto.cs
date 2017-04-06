using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace radio.Dtos
{
    public class SongDto
    {
        public int ID { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int TrackNumber { get; set; }
        public string Duration { get; set; }
        public string Location { get; set; }
    }
}