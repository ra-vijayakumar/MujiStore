using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MujiStore.Models
{
    public class MediaViewLog
    {
        public int MediaViewLogID { get; set; }
        public int MediaID { get; set; }
        public string UserName { get; set; }
        public string ClientIP { get; set; }
        public string UserAgent { get; set; }
        public bool DELFG { get; set; }
        public System.DateTime CRTDT { get; set; }
        public string CRTCD { get; set; }
        public System.DateTime UPDDT { get; set; }
        public string UPDCD { get; set; }
        public string IPAddress { get; set; }
    }
}