using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDAM.Models;
using MDAM.Models.DialogTypes;

namespace Web.Controllers
{
    public class DialogTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DialogType
        public async Task<ActionResult> Index()
        {
            return View(await db.DialogTypes.ToListAsync());
        }

        // GET: DialogType/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DialogType dialogType = await db.DialogTypes.FindAsync(id);
            if (dialogType == null)
            {
                return HttpNotFound();
            }
            return View(dialogType);
        }

        // GET: DialogType/Create
        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
                return PartialView("_CreateFormPartial");
            return View();
        }

        // POST: DialogType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title")] DialogType dialogType)
        {
            if (ModelState.IsValid)
            {
                dialogType.Id = Guid.NewGuid().ToString(); // Генерация ID
                db.DialogTypes.Add(dialogType);
                await db.SaveChangesAsync();
                if (Request.IsAjaxRequest())
                    return PartialView("_CreateFormPartial", dialogType);
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
                return PartialView("_CreateFormPartial", dialogType);
            return View(dialogType);
        }

        // GET: DialogType/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DialogType dialogType = await db.DialogTypes.FindAsync(id);
            if (dialogType == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView("_EditFormPartial", dialogType);
            return View(dialogType);
        }

        // POST: DialogType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title")] DialogType dialogType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dialogType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (Request.IsAjaxRequest())
                    return PartialView("_EditFormPartial", dialogType);
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
                return PartialView("_EditFormPartial", dialogType);
            return View(dialogType);
        }

        // GET: DialogType/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DialogType dialogType = await db.DialogTypes.FindAsync(id);
            if (dialogType == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("DeleteConfirmed", dialogType.Id);
        }

        // POST: DialogType/Delete/5
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DialogType dialogType = await db.DialogTypes.FindAsync(id);
            db.DialogTypes.Remove(dialogType);
            await db.SaveChangesAsync();
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
