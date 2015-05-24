using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using MDAM.Models;
using MDAM.Models.Dialogs;
using MDAM.Models.Messages;
using MDAM.Models.Attachments;

namespace Web.Controllers
{
    public class DialogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dialog
        public async Task<ActionResult> Index()
        {
            var id = User.Identity.GetUserId();
            var dialogs = db.Dialogs
                .Include(d => d.CreatorUser)
                .Include(d => d.DialogType)
                .Include(d => d.ApplicationUsers)
                .Where(t => t.ApplicationUsers.Select(a => a.Id).Contains(id));
            ViewBag.Count = dialogs.Count();
            if (new ArrayList { 2, 3, 4 }.Contains(ViewBag.Count % 10))
                ViewBag.Count += " диалога";
            else if (ViewBag.Count % 10 == 1)
                ViewBag.Count += " диалог";
            else ViewBag.Count += " диалогов";

            return View(await dialogs.ToListAsync());
        }

        // GET: Dialog/Messages/5
        public ActionResult Messages(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var messages = db.Messages
                .Include(m => m.Attachment)
                .Include(m => m.Sender)
                .Where(m => m.DialogId == id);
            if (messages == null)
            {
                return HttpNotFound();
            }
            ViewBag.Count = messages.Count();
            if (new ArrayList { 2, 3, 4 }.Contains(ViewBag.Count % 10))
                ViewBag.Count += " сообщения";
            else if (ViewBag.Count % 10 == 1)
                ViewBag.Count += " сообщение";
            else ViewBag.Count += " сообщений";
            ViewBag.DialogId = id;
            return View();
        }

        public async Task<ActionResult> MessagesList(string dialogId)
        {
            var messages = db.Messages
                .Include(m => m.Attachment)
                .Include(m => m.Sender)
                .Where(m => m.DialogId == dialogId).OrderByDescending(t => t.DateTime);
            ViewBag.Count = messages.Count();
            if (new ArrayList { 2, 3, 4 }.Contains(ViewBag.Count % 10))
                ViewBag.Count += " сообщения";
            else if (ViewBag.Count % 10 == 1)
                ViewBag.Count += " сообщение";
            else ViewBag.Count += " сообщений";
            return PartialView("_Messages", await messages.ToListAsync());
        }

        [HttpPost]
        public ActionResult AddMessage(HttpPostedFileBase upload, string dialogId, string text)
        {
            var dialog = db.Dialogs.Include(t => t.Messages).FirstOrDefault(t => t.Id == dialogId);
            if (dialog == null || (String.IsNullOrWhiteSpace(text) && upload == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Text = text,
                SenderId = User.Identity.GetUserId(),
                DateTime = DateTime.Now
            };
            if (upload != null)
            {
                var attachmentPath = "/Attachments/" + message.Id;
                var attachment = new Attachment
                {
                    FileName = System.IO.Path.GetFileName(upload.FileName),
                    Size = Decimal.Round(upload.ContentLength / 1024, 2).ToString(),
                    Path = attachmentPath
                };
                if (UploadFile(upload, attachmentPath))
                {
                    message.Attachment = attachment;
                }
            }
            dialog.Messages.Add(message);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult RemoveMessage(string id)
        {
            var message = db.Messages
                .Include(t => t.Attachment)
                .FirstOrDefault(t => t.Id == id);
            if (message == null)
            {
                return HttpNotFound();
            }
            if (message.SenderId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (message.Attachment != null)
            {
                RemoveFile(message.Attachment.Path);
            }
            var dialogId = message.DialogId;
            db.Messages.Remove(message);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
            //return RedirectToAction("Messages", new { Id = dialogId });
        }

        public FileResult Download(string id)
        {
            var file = db.Attachments.FirstOrDefault(t => t.AttachmentId == id);
            if (file != null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(file.Path));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
            return null;
        }

        private bool UploadFile(HttpPostedFileBase upload, string path)
        {
            try
            {
                // Сохранение файла в папку Attachments проекта
                upload.SaveAs(Server.MapPath(path));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void RemoveFile(string path)
        {
            //using (System.IO)
            //{
            //    if (File.Ex)
            //        File.Delete(Server.MapPath(path));
            //}
        }

        // GET: Dialog/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dialog dialog = await db.Dialogs.FindAsync(id);
            if (dialog == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView("_DetailsFormPartial", dialog);
            return View(dialog);
        }

        // GET: Dialog/Create
        public ActionResult Create()
        {
            var currentUserId = User.Identity.GetUserId(); // ID текущего пользователя
            ViewBag.Users = new MultiSelectList(db.Users.Where(t => t.Id != currentUserId), "Id", "UserName"); // Загрузка пользователей в список для выбора, кроме создателя
            ViewBag.DialogTypeId = new SelectList(db.DialogTypes, "Id", "Title"); // Загрузка типов диалогов в выпадающий список
            if (Request.IsAjaxRequest())
                return PartialView("_CreateFormPartial");
            return View();
        }

        // POST: Dialog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Dialog dialog, string[] selectedUsers)
        {
            if (ModelState.IsValid)
            {
                selectedUsers = selectedUsers ?? new string[0]; // Если массив NULL - инициализируется пустым
                dialog.Id = Guid.NewGuid().ToString(); // Генерация ID
                dialog.CreatorUserId = User.Identity.GetUserId(); // ID создателя (текущий пользователь)
                dialog.ApplicationUsers = db.Users.Where(t => selectedUsers.Contains(t.Id)).ToList(); // Запись в коллекцию всех выбранных пользователей
                dialog.ApplicationUsers.Add(db.Users.FirstOrDefault(t => t.Id == dialog.CreatorUserId)); // Добавление создателя в список пользователей
                db.Dialogs.Add(dialog);
                await db.SaveChangesAsync();
                if (Request.IsAjaxRequest())
                {
                    var currentUserId = User.Identity.GetUserId(); // ID текущего пользователя
                    // Загрузка пользователей в список для выбора, кроме создателя, с передачей массива уже выбранных пользователей
                    ViewBag.Users = new MultiSelectList(db.Users.Where(t => t.Id != currentUserId), "Id", "UserName", dialog.ApplicationUsers.Select(t => t.Id).ToArray());
                    ViewBag.DialogTypeId = new SelectList(db.DialogTypes, "Id", "Title", dialog.DialogTypeId); 
                    return PartialView("_CreateFormPartial", dialog);
                }
                return RedirectToAction("Index");
            }

            ViewBag.DialogTypeId = new SelectList(db.DialogTypes, "Id", "Title", dialog.DialogTypeId);
            if (Request.IsAjaxRequest())
                return PartialView("_CreateFormPartial", dialog);
            return View(dialog);
        }

        // GET: Dialog/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dialog dialog = await db.Dialogs.FindAsync(id);
            if (dialog == null)
            {
                return HttpNotFound();
            }
            if (dialog.CreatorUserId != User.Identity.GetUserId())
            {
                if (Request.IsAjaxRequest())
                    return PartialView("_DetailsFormPartial", dialog);
                return RedirectToAction("Details", new { Id = id });
            }
            var currentUserId = User.Identity.GetUserId(); // ID текущего пользователя
            // Загрузка пользователей в список для выбора, кроме создателя, с передачей массива уже выбранных пользователей
            ViewBag.Users = new MultiSelectList(db.Users.Where(t => t.Id != currentUserId), "Id", "UserName", dialog.ApplicationUsers.Select(t => t.Id).ToArray());
            ViewBag.DialogTypeId = new SelectList(db.DialogTypes, "Id", "Title", dialog.DialogTypeId);
            if (Request.IsAjaxRequest())
                return PartialView("_EditFormPartial", dialog);
            return View(dialog);
        }

        // POST: Dialog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Dialog dialog, string[] selectedUsers)
        {
            if (ModelState.IsValid)
            {
                var originalDialog = db.Dialogs.AsNoTracking().FirstOrDefault(t => t.Id == dialog.Id);
                if (originalDialog == null)
                {
                    return HttpNotFound();
                }
                if (originalDialog.CreatorUserId != User.Identity.GetUserId())
                {
                    return new HttpUnauthorizedResult();
                }

                selectedUsers = selectedUsers ?? new string[0]; // Если массив NULL - инициализируется пустым
                db.Entry(dialog).State = EntityState.Modified;
                db.Entry(dialog).Collection(t => t.ApplicationUsers).Load(); // Загрузка связанных данных (пользователей)
                dialog.CreatorUserId = originalDialog.CreatorUserId; // Оригинальное значение ID создателя диалога
                dialog.ApplicationUsers = db.Users.Where(t => selectedUsers.Contains(t.Id)).ToList(); // Запись в коллекцию всех выбранных пользователей
                dialog.ApplicationUsers.Add(db.Users.FirstOrDefault(t => t.Id == originalDialog.CreatorUserId)); // Добавление создателя в список пользователей
                await db.SaveChangesAsync();
                if (Request.IsAjaxRequest())
                {
                    var currentUserId = User.Identity.GetUserId(); // ID текущего пользователя
                    // Загрузка пользователей в список для выбора, кроме создателя, с передачей массива уже выбранных пользователей
                    ViewBag.Users = new MultiSelectList(db.Users.Where(t => t.Id != currentUserId), "Id", "UserName", dialog.ApplicationUsers.Select(t => t.Id).ToArray());
                    ViewBag.DialogTypeId = new SelectList(db.DialogTypes, "Id", "Title", dialog.DialogTypeId);
                    return PartialView("_EditFormPartial", dialog);
                }
                return RedirectToAction("Index");
            }
            ViewBag.DialogTypeId = new SelectList(db.DialogTypes, "Id", "Title", dialog.DialogTypeId);
            if (Request.IsAjaxRequest())
                return PartialView("_EditFormPartial", dialog);
            return View(dialog);
        }

        // GET: Dialog/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dialog dialog = await db.Dialogs.FindAsync(id);
            if (dialog == null)
            {
                return HttpNotFound();
            }
            if (dialog.CreatorUserId != User.Identity.GetUserId())
            {
                return View(dialog);
            }
            var usrId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(t => t.Id == usrId);
            db.Entry(dialog).State = EntityState.Modified;
            db.Entry(dialog).Collection(t => t.ApplicationUsers).Load(); // Загрузка связанных данных (пользователей)
            dialog.ApplicationUsers.Remove(user); // Удаление текущего пользователя из списка участников
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Dialog/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Dialog dialog = await db.Dialogs.FindAsync(id);
            db.Dialogs.Remove(dialog);
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