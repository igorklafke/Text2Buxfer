using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Text2Buxfer.Models;
using Text2Buxfer.Services;

namespace Text2Buxfer.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController() : this(IdentityConfig.Secrets, IdentityConfig.Logins, IdentityConfig.Users, IdentityConfig.Roles) { }

        public AccountController(IUserSecretStore secrets, IUserLoginStore logins, IUserStore users, IRoleStore roles)
        {
            Secrets = secrets;
            Logins = logins;
            Users = users;
            Roles = roles;
            Buxfer = new Buxfer();
        }

        public IUserSecretStore Secrets { get; private set; }
        public IUserLoginStore Logins { get; private set; }
        public IUserStore Users { get; private set; }
        public IRoleStore Roles { get; private set; }
        public Buxfer Buxfer { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Validate the user password
                if (await Buxfer.Login(model.UserName, model.Password))
                {
                    string userId = Crypto.Encrypt(model.UserName + "|" + model.Password, "t2b20130708");
                    SignIn(userId, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(String.Empty, "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.SignOut();
            return RedirectToAction("Index", "Home");
        }

       
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool changePasswordSucceeded = false;
            if (await Secrets.Validate(userName, oldPassword))
            {
                changePasswordSucceeded = await Secrets.UpdateSecret(userName, newPassword);
            }
            return changePasswordSucceeded;
        }


        private void SignIn(string userId, bool isPersistent)
        {
            IEnumerable<Claim> claims = new Claim[0];
            IList<Claim> userClaims = IdentityConfig.RemoveUserIdentityClaims(claims);
            IdentityConfig.AddUserIdentityClaims(userId, userId, userClaims);
            IdentityConfig.SignIn(HttpContext, userClaims, isPersistent);
        }

        //private class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUrl)
        //    {
        //        LoginProvider = provider;
        //        RedirectUrl = redirectUrl;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUrl { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        context.HttpContext.Challenge(LoginProvider, new AuthenticationExtra() { RedirectUrl = RedirectUrl });
        //    }
        //}
        
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }
        
        #endregion
    }
}
