﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class mujiEntities1 : DbContext
    {
        public mujiEntities1()
            : base("name=mujiEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblDeployLog> tblDeployLogs { get; set; }
        public virtual DbSet<tblDeploySchedule> tblDeploySchedules { get; set; }
        public virtual DbSet<tblDeployStatu> tblDeployStatus { get; set; }
        public virtual DbSet<tblFeedback> tblFeedbacks { get; set; }
        public virtual DbSet<tblFolder> tblFolders { get; set; }
        public virtual DbSet<tblFormat> tblFormats { get; set; }
        public virtual DbSet<tblMedia> tblMedias { get; set; }
        public virtual DbSet<tblMediaFormatInfo> tblMediaFormatInfoes { get; set; }
        public virtual DbSet<tblMediaViewLog> tblMediaViewLogs { get; set; }
        public virtual DbSet<tblRobocopyExitcode> tblRobocopyExitcodes { get; set; }
        public virtual DbSet<tblStore> tblStores { get; set; }
        public virtual DbSet<tblStoreGroup> tblStoreGroups { get; set; }
        public virtual DbSet<tblStoreGroupFolder> tblStoreGroupFolders { get; set; }
        public virtual DbSet<tblStoreSubnet> tblStoreSubnets { get; set; }
        public virtual DbSet<tblStreamServer> tblStreamServers { get; set; }
        public virtual DbSet<tblStreamServerFormat> tblStreamServerFormats { get; set; }
        public virtual DbSet<tblStreamServerSubnet> tblStreamServerSubnets { get; set; }
        public virtual DbSet<tblSubnet> tblSubnets { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblVideoLogReport> tblVideoLogReports { get; set; }
    }
}
