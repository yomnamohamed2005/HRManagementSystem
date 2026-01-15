using Business_acess_lyer.interfaces;
using Data_access_lyer.models;
using Microsoft.AspNetCore.Mvc;

namespace mvc3.Controllers
{
	public class DepartmentController : Controller
	{
		private readonly IUnitofwork _unit;

		public DepartmentController(IUnitofwork unit)
		{
			_unit = unit;
		}

		// ========================= Index =========================
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var result = await _unit.data.Getallasync();
			return View(result);
		}

		// ========================= Create =========================
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(department department)
		{
			if (!ModelState.IsValid)
				return View(department);

			await _unit.data.createasync(department);
			await _unit.SaveChangesasync();

			return RedirectToAction(nameof(Index));
		}

		// ========================= Details =========================
		public async Task<IActionResult> Details(int? id)
			=> await DepartmentControllerHandler(id, nameof(Details));

		// ========================= Edit =========================
		public async Task<IActionResult> Edit(int? id)
			=> await DepartmentControllerHandler(id, nameof(Edit));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, department department)
		{
			if (id != department.id)
				return BadRequest();

			if (!ModelState.IsValid)
				return View(department);

			try
			{
				_unit.data.update(department);
				await _unit.SaveChangesasync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(department);
			}
		}

		// ========================= Delete =========================
		public async Task<IActionResult> Delete(int? id)
			=> await DepartmentControllerHandler(id, nameof(Delete));

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmDelete(int? id)
		{
			if (!id.HasValue)
				return BadRequest();

			var department = await _unit.data.Getasync(id.Value);
			if (department is null)
				return NotFound();

			try
			{
				_unit.data.Delete(department);
				await _unit.SaveChangesasync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(department);
			}
		}

		// ========================= Shared Handler =========================
		private async Task<IActionResult> DepartmentControllerHandler(int? id, string viewName)
		{
			if (!id.HasValue)
				return BadRequest();

			var department = await _unit.data.Getasync(id.Value);

			if (department is null)
				return NotFound();

			return View(viewName, department);
		}
	}
}

