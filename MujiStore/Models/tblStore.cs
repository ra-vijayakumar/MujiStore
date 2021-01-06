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

    public partial class tblStore
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblStore()
        {
            this.tblStoreSubnets = new HashSet<tblStoreSubnet>();
        }
        [Display(Name = nameof(MujiStore.Resources.Resource.storeid), ResourceType = typeof(MujiStore.Resources.Resource))]
        public int StoreID { get; set; }


        [Required(ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.ModtblStoreStoreNameDataAnnaValida1))]
        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreStoreName), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string StoreName { get; set; }



        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreAddressLine1), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string AddressLine1 { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreAddressLine2), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string AddressLine2 { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreCity), ResourceType = typeof(MujiStore.Resources.Resource))]
        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        public string City { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreState), ResourceType = typeof(MujiStore.Resources.Resource))]
        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        public string State { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreZip), ResourceType = typeof(MujiStore.Resources.Resource))]
        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        public string Zip { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreCountry), ResourceType = typeof(MujiStore.Resources.Resource))]
        [MaxLength(250, ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.CommonDataAnna0of1))]
        public string Country { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonDelFlg), ResourceType = typeof(MujiStore.Resources.Resource))]
        public bool DELFG { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonCRTDT), ResourceType = typeof(MujiStore.Resources.Resource))]
        public Nullable<System.DateTime> CRTDT { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonCRTCD), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string CRTCD { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonUPDDT), ResourceType = typeof(MujiStore.Resources.Resource))]
        public Nullable<System.DateTime> UPDDT { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonUPDCD), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string UPDCD { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonIpAddress), ResourceType = typeof(MujiStore.Resources.Resource))]
        public string IPAddress { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.CommonIpAddress), ResourceType = typeof(MujiStore.Resources.Resource))]
        [RegularExpression(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$", ErrorMessageResourceType = typeof(MujiStore.Resources.Resource), ErrorMessageResourceName = nameof(MujiStore.Resources.Resource.ModtblStoreStoreIPAddressDataAnnaValida1))]
        public string StoreIPAddress { get; set; }

        [Display(Name = nameof(MujiStore.Resources.Resource.ModtblStoreStoreGroupID), ResourceType = typeof(MujiStore.Resources.Resource))]
        public Nullable<int> StoreGroupID { get; set; }

        public virtual tblStoreGroup tblStoreGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblStoreSubnet> tblStoreSubnets { get; set; }
    }
}