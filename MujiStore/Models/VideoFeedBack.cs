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
    
    public partial class VideoFeedBack
    {
        public int VideoId { get; set; }
        public int FolderID { get; set; }
        public string VideoTitle { get; set; }
    }
    public partial class BulkUploadMain
    {
        public string UserName { get; set; }
        public IEnumerable<BulkUploadVideo> uploadList { get; set; }
    }
    public partial class BulkUploadVideo
    {
        public bool IsUpload { get; set; }
        public string UploadFileName { get; set; }
        public string UploadTitle { get; set; }
        public string UploadDescription { get; set; }
        public int UploadFolderID { get; set; }
        public bool IsDelete { get; set; }
    }

    public class ViewLogDetails
    {
        public int TableID { get; set; }
        public string TableDescription { get; set; }
        public string MethodName { get; set; }
        public int TableCount { get; set; }
    }

    public partial class MediaDeployStatus
    {
        public tblMedia tblMedia { get; set; }
        public IEnumerable<tblDeployStatu> deployStatus { get; set; }
    }
    //public partial class Feedback
    //{
    //    public int ID { get; set; }
    //    public int MovieID { get; set; }
    //    public string Comments { get; set; }
    //    public string FileName { get; set; }
    //    public string IPAddress { get; set; }
    //    public bool DELFG { get; set; }
    //    public System.DateTime CRTDT { get; set; }
    //    public string CRTCD { get; set; }
    //    public Nullable<System.DateTime> UPDDT { get; set; }
    //    public string UPDCD { get; set; }
    //}
}