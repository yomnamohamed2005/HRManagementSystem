using Data_access_lyer.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc3.viewmodels;

namespace mvc3.Controllers
{
	[Authorize(Roles = "superadmin")]
	public class UserController : Controller
	{
		private readonly UserManager<applicationuser> _usermanager;

		public UserController(UserManager<applicationuser> usermanager)
		{
			_usermanager = usermanager;
		}
	
		public IActionResult Create()
		{
			var model = new createdusermodel();
			return View(model);
		}

	
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(createdusermodel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = new applicationuser
			{
				UserName = model.username,
				Email = model.email,
				firstname = model.firstname,
				lastname = model.lastname
			};

			var result = await _usermanager.CreateAsync(user, "Default@123"); 
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(Index));
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}


		
		public async Task<IActionResult> Index(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				var usersList = await _usermanager.Users.ToListAsync();
				var users = new List<userviewmodel>();

				foreach (var u in usersList)
				{
					users.Add(new userviewmodel
					{
						email = u.Email,
						firstname = u.firstname,
						lastname = u.lastname,
						username = u.UserName,
						id = u.Id,
						roles = await _usermanager.GetRolesAsync(u)
					});
				}
				return View(users);
			}

			var user = await _usermanager.FindByEmailAsync(email);
			if (user is null) return View(new List<userviewmodel>());

			var model = new userviewmodel
			{
				email = user.Email,
				firstname = user.firstname,
				lastname = user.lastname,
				username = user.UserName,
				id = user.Id,
				roles = await _usermanager.GetRolesAsync(user)
			};

		
			return View(new List<userviewmodel> { model });
		}

	
		public async Task<IActionResult> Details(string id, string viewname = nameof(Details))
		{
			if (string.IsNullOrEmpty(id)) return BadRequest();

			var user = await _usermanager.FindByIdAsync(id);
			if (user is null) return NotFound();

			var model = new userviewmodel
			{
				email = user.Email,
				firstname = user.firstname,
				lastname = user.lastname,
				username = user.UserName,
				id = user.Id,
				roles = await _usermanager.GetRolesAsync(user)
			};

			return View(viewname, model);
		}

      

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new userviewmodel
            {
                email = user.Email,
                firstname = user.firstname,
                lastname = user.lastname,
                username = user.UserName,
                id = user.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, userviewmodel model)
        {
            if (id != model.id) return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.firstname = model.firstname;
            user.lastname = model.lastname;
            user.UserName = model.username;
            user.Email = model.email;

            var result = await _usermanager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

       
        public async Task<IActionResult> Delete(string id) => await Details(id, nameof(Delete));

	
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("delete")]
		public async Task<IActionResult> ConfirmDelete(string id)
		{
			try
			{
				var user = await _usermanager.FindByIdAsync(id);
				if (user is null) return NotFound();

				await _usermanager.DeleteAsync(user);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
			}

			return View();
		}
	}
}

