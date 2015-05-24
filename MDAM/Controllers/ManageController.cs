using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MDAM.Models;

namespace MDAM.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль был успешно изменен."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";

            var model = new IndexViewModel
            {
                ApplicationUser = db.Users.Find(User.Identity.GetUserId())
            }; 
            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        public ActionResult Edit(string Id)
        {
            ApplicationUser user = db.Users.Find(Id);
            return View(user);
        }
        //
        // POST
        [HttpPost]
        public ActionResult Edit(string Id, string UserName, string Email, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(Id);

                if (UserName != user.UserName)
                {
                    user.UserName = UserName;
                }
                else if (user.Email != Email)
                {
                    user.Email = Email;
                }

                if (upload != null)
                {
                    user.Image = UploadFile(upload);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private string UploadFile(HttpPostedFileBase upload)
        {
            string fileName = System.IO.Path.GetFileName(upload.FileName);
            // сохраняем файл в папку Images в проекте
            upload.SaveAs(Server.MapPath("~/Images/Users/" + fileName));
            return fileName;
        }

        public ActionResult DeletePhoto(string Id)
        {
            // получаем пользователя
            ApplicationUser user = db.Users.Find(Id);
            // путь к файлу
            string pathToFile = Server.MapPath("~/Images/Users/" + user.Image);

            // если файл не стандартный
            if (user.Image != "default.jpg")
            {
                // существует ли файл
                if (System.IO.File.Exists(pathToFile))
                {
                    // удаляем файл и меняем на стандартный
                    System.IO.File.Delete(pathToFile);
                    user.Image = "default.jpg";
                    db.SaveChanges();
                }
                else
                {
                    user.Image = "default.jpg";
                }
            }

            return View("Edit", user);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

#endregion
    }
}