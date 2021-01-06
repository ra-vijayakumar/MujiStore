//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MujiStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class tblDeployStatu
    {
        public int DeployStatusID { get; set; }
        [Display(Name = nameof(MujiStore.Resources.Resource.DSServer), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string DSServer { get; set; }
        public Nullable<int> MediaID { get; set; }
        public Nullable<int> FormatID { get; set; }
        public Nullable<bool> IsExists { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public bool DELFG { get; set; }
        public Nullable<System.DateTime> CRTDT { get; set; }
        public string CRTCD { get; set; }
        public Nullable<System.DateTime> UPDDT { get; set; }
        public string UPDCD { get; set; }
        public string UserIPAddress { get; set; }


        public bool Recommend { get; set; }
        public string IPAddress { get; set; }
        [Display(Name = nameof(MujiStore.Resources.Resource.FormatName), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string FormatName { get; set; }
        public virtual tblFormat tblFormat { get; set; }

        public Nullable<int> Duration { get; set; }
        public Nullable<long> FileSize { get; set; }
        [Required(ErrorMessage ="Select Result")]
        public int Result { get; set; }
        public int DeployLogID { get; set; }
        public Int64 MediaCount { get; set; }
        public Int64 TotalFileSize { get; set; }
        public float RequiredBandWidth { get; set; }
    }
}