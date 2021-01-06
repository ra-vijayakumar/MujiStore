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
namespace MujiStore.Controllers
{
    [SessionExpire]
    [Authorize]
    //[MUJICustomAuthorize(Roles = "A,U")]
    [Authorize(Roles = "1000")]
    public class StreamServerFormatsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: StreamServerFormats
        public ActionResult Index()
        {
            return View(db.tblStreamServerFormats.ToList());
        }

        // GET: StreamServerFormats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServerFormat tblStreamServerFormat = db.tblStreamServerFormats.Find(id);
            if (tblStreamServerFormat == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServerFormat);
        }

        // GET: StreamServerFormats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StreamServerFormats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StreamServerFormatID,SSFServer,FormatID,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStreamServerFormat tblStreamServerFormat)
        {
            if (ModelState.IsValid)
            {
                db.tblStreamServerFormats.Add(tblStreamServerFormat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblStreamServerFormat);
        }

        // GET: StreamServerFormats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServerFormat tblStreamServerFormat = db.tblStreamServerFormats.Find(id);
            if (tblStreamServerFormat == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServerFormat);
        }

        // POST: StreamServerFormats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StreamServerFormatID,SSFServer,FormatID,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblStreamServerFormat tblStreamServerFormat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblStreamServerFormat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblStreamServerFormat);
        }

        // GET: StreamServerFormats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStreamServerFormat tblStreamServerFormat = db.tblStreamServerFormats.Find(id);
            if (tblStreamServerFormat == null)
            {
                return HttpNotFound();
            }
            return View(tblStreamServerFormat);
        }

        // POST: StreamServerFormats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStreamServerFormat tblStreamServerFormat = db.tblStreamServerFormats.Find(id);
            db.tblStreamServerFormats.Remove(tblStreamServerFormat);
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
