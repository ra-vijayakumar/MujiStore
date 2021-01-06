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
    
    public partial class tblDeployLog
    {
        public int DeployLogID { get; set; }
        public string Server { get; set; }
        public Nullable<int> MediaID { get; set; }
        public Nullable<int> FormatID { get; set; }
        public Nullable<int> ElapsedTime { get; set; }
        public Nullable<long> CopiedBytes { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<int> Result { get; set; }
        public bool DELFG { get; set; }
        public Nullable<System.DateTime> CRTDT { get; set; }
        public string CRTCD { get; set; }
        public Nullable<System.DateTime> UPDDT { get; set; }
        public string UPDCD { get; set; }
        public string IPAddress { get; set; }
    
        public virtual tblMedia tblMedia { get; set; }
        public virtual tblRobocopyExitcode tblRobocopyExitcode { get; set; }
    }
}