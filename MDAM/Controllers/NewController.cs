using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MDAM.Models;
using MDAM.Models.News;
using System.Collections;
using System.Globalization;

namespace MDAM.Controllers
{
    public class NewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: New
        public ActionResult Index()
        {
            var News = db.News.Include(b => b.CreatorUser);
            return View(News.ToList());
        }

        // GET: New/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New New = db.News.Find(id);
            if (New == null)
            {
                return HttpNotFound();
            }
            return View(New);
        }
        // GET: New/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: New/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var New = new New();
                New.Text = model.Text;
                New.Title = model.Title;
                New.CreatorUserId = User.Identity.GetUserId();
                New.Date = DateTime.Now.ToString("dd MMMM yyyy в HH:mm:ss", CultureInfo.CreateSpecificCulture("ru-RU"));
                db.News.Add(New);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: New/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New New = db.News.Find(id);
            if (New == null)
            {
                return HttpNotFound();
            }
            return View(New);
        }

        // POST: New/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(New New)
        {
            if (ModelState.IsValid)
            {
                db.Entry(New).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(New);
        }

        // GET: New/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New New = db.News.Find(id);
            if (New == null)
            {
                return HttpNotFound();
            }
            return View(New);
        }

        // POST: New/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string id)
        {
            New New = db.News.Find(id);
            db.News.Remove(New);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveNew(string id)
        {
            New New = db.News.Find(id);
            db.News.Remove(New);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
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
