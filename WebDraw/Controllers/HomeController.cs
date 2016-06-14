using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDraw.Models;

namespace WebDraw.Controllers
{
    public class HomeController : Controller
    {
        private WebDrawDbContext db = new WebDrawDbContext();

        public ActionResult Index()
        {
            int UId = UserID();
            List<Chain> chainList = db.Chains.Where(chain => chain.Open == false && chain.Entries.Any(entry => entry.UserId == UId)).ToList();
            
            return View(chainList);
        }
        

        [NonAction]
        public int UserID()
        {
            Guid userGuid;
            if (Guid.TryParse(User.Identity.GetUserId(), out userGuid))
            {
                var user = db.Users.Where(u => u.IdentityId == userGuid).SingleOrDefault();
                if (user == null)
                {
                    // valid guid but no user in the table? that means a user should be created!
                    User newUser = new Models.User();
                    newUser.IdentityId = userGuid;
                    newUser.VisibleName = User.Identity.Name;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return newUser.Id;
                }
                else
                {
                    return user.Id;
                }
            }
            else
            {
                return 1; // default user, will be impossible to hit once I turn on authentication (supposedly)
            }
        }
    }
}