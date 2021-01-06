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
    [Authorize]
    [MUJICustomAuthorize(Roles = "16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31")]
    public class DeployStatusController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: DeployStatus
        public ActionResult Index()
        {
            var tblDeployStatus = db.tblDeployStatus.Include(t => t.tblFormat);
            return View(tblDeployStatus.ToList());
        }

        // GET: DeployStatus/Details/5
        [NonAction]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeployStatu tblDeployStatu = db.tblDeployStatus.Find(id);
            if (tblDeployStatu == null)
            {
                return HttpNotFound();
            }
            return View(tblDeployStatu);
        }

        // GET: DeployStatus/Create
        public ActionResult Create()
        {
            ViewBag.FormatID = new SelectList(db.tblFormats, "FormatID", "Name");
            return View();
        }

        // POST: DeployStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeployStatusID,DSServer,MediaID,FormatID,IsExists,DateTime,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,UserIPAddress")] tblDeployStatu tblDeployStatu)
        {
            if (ModelState.IsValid)
            {
                db.tblDeployStatus.Add(tblDeployStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FormatID = new SelectList(db.tblFormats, "FormatID", "Name", tblDeployStatu.FormatID);
            return View(tblDeployStatu);
        }

        // GET: DeployStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeployStatu tblDeployStatu = db.tblDeployStatus.Find(id);
            if (tblDeployStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.FormatID = new SelectList(db.tblFormats, "FormatID", "Name", tblDeployStatu.FormatID);
            return View(tblDeployStatu);
        }

        // POST: DeployStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeployStatusID,DSServer,MediaID,FormatID,IsExists,DateTime,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,UserIPAddress")] tblDeployStatu tblDeployStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblDeployStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FormatID = new SelectList(db.tblFormats, "FormatID", "Name", tblDeployStatu.FormatID);
            return View(tblDeployStatu);
        }

        // GET: DeployStatus/Delete/5
        [NonAction]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeployStatu tblDeployStatu = db.tblDeployStatus.Find(id);
            if (tblDeployStatu == null)
            {
                return HttpNotFound();
            }
            return View(tblDeployStatu);
        }

        // POST: DeployStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NonAction]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDeployStatu tblDeployStatu = db.tblDeployStatus.Find(id);
            db.tblDeployStatus.Remove(tblDeployStatu);
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
