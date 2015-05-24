using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MDAM.Models;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using MDAM.Infrastructure;
using System.Data.Entity;

namespace MDAM.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _applicationDbContext;
        public AccountController()
        {
        }
        public const string ADMIN_ROLE = "Admin";
        public ApplicationDbContext DbContext
        {
            get { return _applicationDbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>(); }
            private set { _applicationDbContext = value; }
        }
        [CustomAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult DetailsAdmins()
        {
            if (!DbContext.Roles.Any(t => t.Name == ADMIN_ROLE))
            {
                DbContext.Roles.Add(new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ADMIN_ROLE
                });
                DbContext.SaveChanges();
            }

            var adminRole = DbContext.Roles.Include(t => t.Users).FirstOrDefault(t => t.Name == ADMIN_ROLE);
            var usersIds = adminRole.Users.Select(t => t.UserId).ToArray();
            var users = DbContext.Users.Where(u => usersIds.Contains(u.Id)).ToList();

            return View(new AdminUsersViewModel()
            {
                Role = adminRole,
                Users = users
            });
        }
        [CustomAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ManageAdmins()
        {
            if (!DbContext.Roles.Any(t => t.Name == ADMIN_ROLE))
            {
                DbContext.Roles.Add(new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ADMIN_ROLE
                });
                DbContext.SaveChanges();
            }

            var adminRole = DbContext.Roles.Include(t => t.Users).FirstOrDefault(t => t.Name == ADMIN_ROLE);

            // Загрузка пользователей в список для выбора, кроме создателя, с передачей массива уже выбранных пользователей
            ViewBag.Users = new MultiSelectList(DbContext.Users.ToList(), "Id", "UserName", adminRole.Users.Select(t => t.UserId).ToArray());
            return View(adminRole);
        }
        [CustomAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult ManageAdmins(IdentityRole role, string[] selectedUsers)
        {
            if (ModelState.IsValid)
            {
                role = DbContext.Roles.FirstOrDefault(t => t.Name == ADMIN_ROLE);
                DbContext.Roles.Remove(role);
                DbContext.SaveChanges();

                selectedUsers = selectedUsers ?? new string[0]; // Если массив NULL - инициализируется пустым
                var newRole = new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ADMIN_ROLE
                };

                var users = DbContext.Users.Where(t => selectedUsers.Contains(t.Id));
                foreach (var applicationUser in users)
                {
                    newRole.Users.Add(new IdentityUserRole()
                    {
                        RoleId = newRole.Id,
                        UserId = applicationUser.Id
                    });
                }

                DbContext.Roles.Add(newRole);

                DbContext.SaveChanges();

                return RedirectToAction("DetailsAdmins");
            }

            return View();


            if (!DbContext.Roles.Any(t => t.Name == ADMIN_ROLE))
            {
                DbContext.Roles.Add(new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ADMIN_ROLE
                });
                DbContext.SaveChanges();
            }

            var adminRole = DbContext.Roles.Include(t => t.Users).FirstOrDefault(t => t.Name == ADMIN_ROLE);

            // Загрузка пользователей в список для выбора, кроме создателя, с передачей массива уже выбранных пользователей
            ViewBag.Users = new MultiSelectList(DbContext.Users.ToList(), "Id", "UserName", adminRole.Users.Select(t => t.UserId).ToArray());
            return View(adminRole);
        }
        public class AdminUsersViewModel
        {
            public IdentityRole Role { get; set; }
            public IEnumerable<ApplicationUser> Users { get; set; }
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        //
        // GET: /Account/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = DbContext.Users.FirstOrDefault(t => t.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserViewModel userViewModel = new UserViewModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                Image = user.Image == null ? "default.jpg" : user.Image
            };
            return View(userViewModel);
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (Request.IsAjaxRequest())
                return PartialView("_LoginFormPartial");
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("_LoginFormPartial", model);
                return View(model);
            }
            var user = UserManager.FindByEmail(model.UserName);
            var userName = model.UserName;
            if (user != null)
            {
                userName = user.UserName;
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (Request.IsAjaxRequest())
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "Неверный пользователь или пароль.");
                    if (Request.IsAjaxRequest())
                        return PartialView("_LoginFormPartial", model);
                    return View(model);
                default:
                    ModelState.AddModelError("", "Неверный пользователь или пароль.");
                    if (Request.IsAjaxRequest())
                        return PartialView("_LoginFormPartial", model);
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAjaxRequest())
                return PartialView("_RegisterFormPartial");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                user.Image = "default.jpg"; // Фото по умолчанию
                var result = await UserManager.CreateAsync(user, model.Password);
                // Если создание прошло успешно
                if (result.Succeeded)
                {
                    // Добавление роли пользователю по умолчанию
                    await UserManager.AddToRoleAsync(user.Id, "User");

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    if (Request.IsAjaxRequest())
                        return PartialView("_RegisterFormPartial", model);
                    return View("Home", "Index");

                }
                AddErrors(result);
            }
            if (Request.IsAjaxRequest())
                return PartialView("_RegisterFormPartial", model);
            return View(model);
        }

        [AllowAnonymous]
        public JsonResult CheckUserName(string userName)
        {
            var user = UserManager.FindByName(userName);
            if (user == null)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json("Пользователь с таким именем уже существует.", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}