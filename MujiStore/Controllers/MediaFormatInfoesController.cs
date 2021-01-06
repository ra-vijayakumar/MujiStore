using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MujiStore.Models;
using MujiStore.BLL;
using PagedList;
using System.IO;
namespace MujiStore.Controllers
{
    [SessionExpire]

    //[MUJICustomAuthorize(Roles = "A,U")]
    [Authorize(Roles = "1000")]
    public class MediaFormatInfoesController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: MediaFormatInfoes
        public ActionResult Index()
        {
            var tblMediaFormatInfoes = db.tblMediaFormatInfoes.Include(t => t.tblMedia);
            return View(tblMediaFormatInfoes.ToList());
        }

        // GET: MediaFormatInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMediaFormatInfo tblMediaFormatInfo = db.tblMediaFormatInfoes.Find(id);
            if (tblMediaFormatInfo == null)
            {
                return HttpNotFound();
            }
            return View(tblMediaFormatInfo);
        }

        // GET: MediaFormatInfoes/Create
        public ActionResult Create()
        {
            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title");
            return View();
        }

        // POST: MediaFormatInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MediaFormatInfoID,MediaID,FormatID,FileSize,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblMediaFormatInfo tblMediaFormatInfo)
        {
            if (ModelState.IsValid)
            {
                db.tblMediaFormatInfoes.Add(tblMediaFormatInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title", tblMediaFormatInfo.MediaID);
            return View(tblMediaFormatInfo);
        }

        // GET: MediaFormatInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMediaFormatInfo tblMediaFormatInfo = db.tblMediaFormatInfoes.Find(id);
            if (tblMediaFormatInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title", tblMediaFormatInfo.MediaID);
            return View(tblMediaFormatInfo);
        }

        // POST: MediaFormatInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MediaFormatInfoID,MediaID,FormatID,FileSize,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblMediaFormatInfo tblMediaFormatInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblMediaFormatInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title", tblMediaFormatInfo.MediaID);
            return View(tblMediaFormatInfo);
        }

        // GET: MediaFormatInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMediaFormatInfo tblMediaFormatInfo = db.tblMediaFormatInfoes.Find(id);
            if (tblMediaFormatInfo == null)
            {
                return HttpNotFound();
            }
            return View(tblMediaFormatInfo);
        }

        // POST: MediaFormatInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblMediaFormatInfo tblMediaFormatInfo = db.tblMediaFormatInfoes.Find(id);
            db.tblMediaFormatInfoes.Remove(tblMediaFormatInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
