using PZPP_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PZPP_Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        string roleId;
        // GET: Roles
        public ActionResult Index()
        {
            var model = new ApplicationDbContext().Roles.ToList();
            return View(model);
        }
        public ActionResult UserList(string id)
        {
            var role = new ApplicationDbContext().Roles.Where(x => x.Id == id).First();
            roleId = id;
            List<PZPP_Web.Models.ApplicationUser> model = new ApplicationDbContext().Users.Where(x=>x.Roles.Where(y=>y.RoleId.Contains(id)).Any()).ToList();            
            return View(model);
        }
        public ActionResult AddUserToRole()
        {
            var model = new ApplicationDbContext().Users.ToList();
            return View(model);
        }
        public ActionResult AddUserToRole(string id)
        {
            roleId = id;
            var model = new ApplicationDbContext().Users.ToList();
            return View(model);
        }
        public ActionResult Delete(string id)
        {
            System.Web.Security.Roles.RemoveUserFromRole(id, roleId);            
            return RedirectToAction("UserList", roleId);
        }
    }
}