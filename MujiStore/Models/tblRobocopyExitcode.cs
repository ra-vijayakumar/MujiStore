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

    public partial class tblRobocopyExitcode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblRobocopyExitcode()
        {
            this.tblDeployLogs = new HashSet<tblDeployLog>();
        }
        [Display(Name ="ID")]
        public int RobocopyExitcodeID { get; set; }
        [Display(Name = nameof(MujiStore.Resources.Resource.Content), ResourceType = typeof(MujiStore.Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.EnterContent))]
        [MaxLength(120, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.LengthMaximum120Charactor))]
        public string Content { get; set; }
        [Display(Name = nameof(MujiStore.Resources.Resource.CommonDelFlg), ResourceType = typeof(MujiStore.Resources.Resource))]
        public bool DELFG { get; set; }
        public Nullable<System.DateTime> CRTDT { get; set; }
        public string CRTCD { get; set; }
        public Nullable<System.DateTime> UPDDT { get; set; }
        public string UPDCD { get; set; }
        public string IPAddress { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeployLog> tblDeployLogs { get; set; }
    }
}