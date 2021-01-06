using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace MujiStore.Models
{
    public class MediaSearch
    {
 
        [MaxLength(500, ErrorMessage = "{0} can have a max of {1} characters")]
        [Display(Name ="Title")]
        public string SearchTitle { get; set; }
        [Display(Name = "From Create Date")]
        public Nullable<System.DateTime> SearchFromCRTDT { get; set; }
        [Display(Name = "To Create Date")]
        public Nullable<System.DateTime> SearchToCRTDT { get; set; }
        public string SearchFolderName { get; set; }
        public int PageNumber { get; set; }
        public string sortOrder { get; set; }

    }

    public class FolderModel
    {
        public List<System.Web.Mvc.SelectListItem> Folders { get; set; }
        public int[] FolderIds { get; set; }
    }
}