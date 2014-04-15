using System;
using System.Web.Mvc;
using System.Web.Security;
using DevTrends.MvcDonutCaching.Demo.Models;
using DevTrends.MvcDonutCaching.Demo.Mvc;

namespace DevTrends.MvcDonutCaching.Demo.Controllers
{
    [Authorize]
    public class AccountController : ApplicationController
    {
        //
        // GET: /Account/LogIn
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            return View();
        }

        //
        // POST: /Account/LogIn
        [AllowAnonymous, HttpPost]
        public ActionResult LogIn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Le nom d'utilisateur ou mot de passe fourni est incorrect.");
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/LogOut
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [AllowAnonymous, HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Tentative d'inscription de l'utilisateur
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null,
                                      out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword lève une exception plutôt
                // que de retourner false dans certains scénarios de défaillance.
                bool changePasswordSucceeded;
                try
                {
                    var currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }

                ModelState.AddModelError(
                    "",
                    "Le mot de passe actuel est incorrect ou le nouveau mot de passe n'est pas valide."
                );
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Consultez http://go.microsoft.com/fwlink/?LinkID=177550 pour
            // obtenir la liste complète des codes d'état.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Le nom d'utilisateur existe déjà. Entrez un nom d'utilisateur différent.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "Un nom d'utilisateur pour cette adresse de messagerie existe déjà. Entrez une adresse de messagerie différente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Le mot de passe fourni n'est pas valide. Entrez une valeur de mot de passe valide.";

                case MembershipCreateStatus.InvalidEmail:
                    return "L'adresse de messagerie fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidAnswer:
                    return
                        "La réponse de récupération du mot de passe fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidQuestion:
                    return
                        "La question de récupération du mot de passe fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Le nom d'utilisateur fourni n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "Le fournisseur d'authentification a retourné une erreur. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "La demande de création d'utilisateur a été annulée. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";

                default:
                    return
                        "Une erreur inconnue s'est produite. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";
            }
        }

        #endregion
    }
}
