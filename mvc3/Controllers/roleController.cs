using Data_access_lyer.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc3.viewmodels;

namespace mvc3.Controllers
{
    [Authorize(Roles ="superadmin")]
    public class roleController : Controller

    {
        private readonly UserManager<applicationuser> _usermanager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        public roleController(UserManager<applicationuser> usermanager, RoleManager<IdentityRole> rolemanager)
        {
            _usermanager = usermanager;
           _rolemanager = rolemanager;
        }
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> create (roleviewmodel model)
        {
            if (!ModelState.IsValid) return View(model);
            var role = new IdentityRole
            {
                Name = model.name
            };
            var result = await _rolemanager.CreateAsync(role);
            if (result.Succeeded) return RedirectToAction(nameof(Index));
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        public async Task<IActionResult> Index(string name)
        {
            List<roleviewmodel> rolesList;

            if (string.IsNullOrEmpty(name))
            {
                rolesList = await _rolemanager.Roles
                    .Select(r => new roleviewmodel
                    {
                        name = r.Name,
                        id = r.Id
                    })
                    .ToListAsync();
            }
            else
            {
                var role = await _rolemanager.FindByNameAsync(name);
                if (role is null) rolesList = new List<roleviewmodel>();
                else
                    rolesList = new List<roleviewmodel>
            {
                new roleviewmodel { id = role.Id, name = role.Name }
            };
            }

            return View(rolesList);
        }

        public async Task<IActionResult> details(string id, string viewname = nameof(details))
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var role = await _rolemanager.FindByIdAsync(id);
            if (role is null) return NotFound();
            var model = new roleviewmodel
            {
                name = role.Name,
                id = role.Id
            };
            return View(viewname, model);
        }
        public async Task<IActionResult> edit(string id) => await details(id, nameof(edit));
        [HttpPost]
        public async Task<IActionResult> edit(string id, roleviewmodel model)
        {
            if (id != model.id) return BadRequest();

            if (!ModelState.IsValid) return View(model);
            try
            {
                var roles = await _rolemanager.FindByIdAsync(model.id);
                if (roles is null) return NotFound();
             roles.Name = model.name;
               roles.Id = model.id;
                await _rolemanager.UpdateAsync(roles);
                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);


            }
            return View(model);
        }
        public async Task<IActionResult> delete(string id) => await details(id, nameof(delete));
        [HttpPost]
        [ActionName("delete")]
        public async Task<IActionResult> confirmdelete(string id)
        {
            try
            {
                var roles = await _rolemanager.FindByIdAsync(id);
                if (roles is null) return NotFound();

                await _rolemanager.DeleteAsync(roles);
                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);


            }
            return View();
        }

        public async Task<IActionResult>AddOrRemoveUsers(string roleId)
        {
            var role = await _rolemanager.FindByIdAsync(roleId);
            if (role is null) return NotFound();
            ViewBag.RoleId = roleId;
            var users =await _usermanager.Users.ToListAsync();
            var usersrole = new List<UserInRoleViewModel>();
            foreach(var user in users)
            {
                var userrole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsInRole = await _usermanager.IsInRoleAsync(user, role.Name)
                };
                usersrole.Add(userrole);
            }

            return View(usersrole);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId, List<UserInRoleViewModel> users)
        {
            var role = await _rolemanager.FindByIdAsync(roleId);
            if (role is null) return NotFound();
            if (ModelState.IsValid)
            {
                foreach(var user in users)
                {
                    var appuser = await _usermanager.FindByIdAsync(user.UserId);
                    if (appuser is null) return NotFound();
                    if (user.IsInRole && !await _usermanager.IsInRoleAsync(appuser, role.Name)) 
                    await _usermanager.AddToRoleAsync(appuser, role.Name);
                    if (!user.IsInRole && await _usermanager.IsInRoleAsync(appuser, role.Name)) 
                    await _usermanager.RemoveFromRoleAsync(appuser, role.Name);
                }
                return RedirectToAction(nameof(edit),new {id=roleId});
            }
            return View(users);
        }
    }
}
