﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MDAM.Models;
using MDAM.Models.Boards;
using System.Collections;
using System.Globalization;

namespace MDAM.Controllers
{
    public class BoardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Board
        public ActionResult Index()
        {
            var all = User.IsInRole("Admin") ? true : false;
            var userId = User.Identity.GetUserId();
            var boards = db.Boards.Include(b => b.CreatorUser).Where(t => all ? true : t.Status.Equals("Активно"));
            return View(boards.ToList());
        }

        public ActionResult My()
        {
            var userId = User.Identity.GetUserId();
            var boards = db.Boards.Include(b => b.CreatorUser).Where(t => t.CreatorUserId == userId);
            return View(boards.ToList());
        }

        // GET: Board/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = db.Boards.Find(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }
        public ArrayList list = new ArrayList()
            {
                new { Title="Добавлено"},
                new { Title="Активно"},
                new { Title="Деактивировано"},
                new { Title="Архив"}
            };
        // GET: Board/Create
        public ActionResult Create()
        {
            ViewBag.Status = new SelectList(list, "Title", "Title");
            return View();
        }

        // POST: Board/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BoardViewModel model)
        {
            if (ModelState.IsValid)
            {
                var board = new Board();
                board.Text = model.Text;
                board.Title = model.Title;
                board.Status = model.Status;
                board.CreatorUserId = User.Identity.GetUserId();
                board.Date = DateTime.Now.ToString("dd MMMM yyyy в HH:mm:ss", CultureInfo.CreateSpecificCulture("ru-RU"));
                db.Boards.Add(board);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Board/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = db.Boards.Find(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(list, "Title", "Title", board.Status);
            return View(board);
        }

        // POST: Board/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Board board)
        {
            if (ModelState.IsValid)
            {
                db.Entry(board).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(board);
        }

        // GET: Board/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = db.Boards.Find(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }

        // POST: Board/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string id)
        {
            Board board = db.Boards.Find(id);
            db.Boards.Remove(board);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveBoard(string id)
        {
            Board board = db.Boards.Find(id);
            db.Boards.Remove(board);
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
