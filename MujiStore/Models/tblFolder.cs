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

    public partial class tblFolder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblFolder()
        {
            this.tblMedias = new HashSet<tblMedia>();
            this.tblStoreGroupFolders = new HashSet<tblStoreGroupFolder>();
        }
        [Display(Name = nameof(MujiStore.Resources.Resource.FolderId), ResourceType = typeof(MujiStore.Resources.Resource))]
        public int FolderID { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblFolderParentID), ResourceType = typeof(MujiStore.Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.ModtblFolderParentIDDataAnnaValida1))]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.ModtblFolderParentIDDataAnnaValida2))]
        public int ParentID { get; set; }

        [Required(ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.ModtblFolderParentIDDataAnnaValida3))]
        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.ModtblFolderParentIDDataAnnaValida4))]
        [Display(Name = nameof(MujiStore.Resources.Resource.CommonFileName), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonDelFlg), ResourceType = typeof(MujiStore.Resources.Resource))]
        public bool DELFG { get; set; }


        [Display(Name = nameof(MujiStore.Resources.Resource.CommonCRTDT), ResourceType = typeof(MujiStore.Resources.Resource))]
        public System.DateTime CRTDT { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonCRTCD), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string CRTCD { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonUPDDT), ResourceType = typeof(MujiStore.Resources.Resource))]
        public Nullable<System.DateTime> UPDDT { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonUPDCD), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string UPDCD { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonIpAddress), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string IPAddress { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblFolderParentFolderName), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string ParentFolderName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMedia> tblMedias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblStoreGroupFolder> tblStoreGroupFolders { get; set; }
    }
}
