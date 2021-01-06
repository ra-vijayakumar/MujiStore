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
    public class DeployLogsController : Controller
    {
        private mujiEntities1 db = new mujiEntities1();

        // GET: DeployLogs
        public ActionResult Index()
        {
            var tblDeployLogs = db.tblDeployLogs.Include(t => t.tblMedia).Include(t => t.tblRobocopyExitcode);
            return View(tblDeployLogs.ToList());
        }

        // GET: DeployLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeployLog tblDeployLog = db.tblDeployLogs.Find(id);
            if (tblDeployLog == null)
            {
                return HttpNotFound();
            }
            return View(tblDeployLog);
        }

        // GET: DeployLogs/Create
        public ActionResult Create()
        {
            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title");
            ViewBag.Result = new SelectList(db.tblRobocopyExitcodes, "RobocopyExitcodeID", "Content");
            return View();
        }

        // POST: DeployLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeployLogID,Server,MediaID,FormatID,ElapsedTime,CopiedBytes,DateTime,Result,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblDeployLog tblDeployLog)
        {
            if (ModelState.IsValid)
            {
                db.tblDeployLogs.Add(tblDeployLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title", tblDeployLog.MediaID);
            ViewBag.Result = new SelectList(db.tblRobocopyExitcodes, "RobocopyExitcodeID", "Content", tblDeployLog.Result);
            return View(tblDeployLog);
        }

        // GET: DeployLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeployLog tblDeployLog = db.tblDeployLogs.Find(id);
            if (tblDeployLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title", tblDeployLog.MediaID);
            ViewBag.Result = new SelectList(db.tblRobocopyExitcodes, "RobocopyExitcodeID", "Content", tblDeployLog.Result);
            return View(tblDeployLog);
        }

        // POST: DeployLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeployLogID,Server,MediaID,FormatID,ElapsedTime,CopiedBytes,DateTime,Result,DELFG,CRTDT,CRTCD,UPDDT,UPDCD,IPAddress")] tblDeployLog tblDeployLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblDeployLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MediaID = new SelectList(db.tblMedias, "MediaID", "Title", tblDeployLog.MediaID);
            ViewBag.Result = new SelectList(db.tblRobocopyExitcodes, "RobocopyExitcodeID", "Content", tblDeployLog.Result);
            return View(tblDeployLog);
        }

        // GET: DeployLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDeployLog tblDeployLog = db.tblDeployLogs.Find(id);
            if (tblDeployLog == null)
            {
                return HttpNotFound();
            }
            return View(tblDeployLog);
        }

        // POST: DeployLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDeployLog tblDeployLog = db.tblDeployLogs.Find(id);
            db.tblDeployLogs.Remove(tblDeployLog);
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
