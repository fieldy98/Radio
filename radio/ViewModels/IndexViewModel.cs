using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace radio.ViewModels
{
    public class IndexViewModel
    {
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public int? tracknumber { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string format { get; set; }
        public string location { get; set; }
        public List<ivm> indexview { get; set; }
        public IndexViewModel()
        {
            indexview = new List<ivm>();
        }
    }

    public class ivm
    {
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public int? tracknumber { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string format { get; set; }
        public string location { get; set; }
    }
}