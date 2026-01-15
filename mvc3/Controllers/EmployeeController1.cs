using Business_acess_lyer.interfaces;
using Data_access_lyer.models;
using Microsoft.AspNetCore.Mvc;
using Business_acess_lyer.repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using mvc3.viewmodels;
using System.Reflection;
using mvc3.utilities;
using Microsoft.AspNetCore.Authorization;

namespace mvc3.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper _mapper;
        public EmployeeController(IUnitofwork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string ? searchvalue)
        {
            // ViewData["message"] = "hello view1";
            // ViewData["message"] = new employee { name = "yomna";
            // ViewBag.message = new employee { name = "mona" };
            var emp = Enumerable.Empty<employee>();
            if(string.IsNullOrEmpty(searchvalue))
             emp =  await _unitofwork.employees.getallwithdepartmentasync();
            else
                emp =await _unitofwork.employees.Getallasync(searchvalue);
            var emp1 = _mapper.Map<IEnumerable<employee>,IEnumerable<Employeeviewmodel>>(emp);
            return View(emp1);
        }
        public async Task<IActionResult> Create()

        {
            var department =await _unitofwork.data.Getallasync();
            SelectList list = new SelectList(department,"id","name");
            ViewBag.department = list;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employeeviewmodel employeevm)
        {
            if (!ModelState.IsValid) return View(employeevm);
            if(employeevm.image is not null)
            employeevm.imagename = documentsetting.updatefiles(employeevm.image, "images");
            var employee1 = _mapper.Map<Employeeviewmodel, employee>(employeevm);
           await _unitofwork.employees.createasync(employee1);
           await _unitofwork.SaveChangesasync();
            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> details(int? id) =>await Employeecontrollerhandler(id, nameof(details));
        public async Task<IActionResult> EditAsync(int? id) => await Employeecontrollerhandler(id, nameof(Edit));
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id , Employeeviewmodel employeevm)
        {
            if (id != employeevm.Id) return BadRequest();
            if(ModelState.IsValid)
            {
                try

                {
                    if (employeevm.image is not null)
                        employeevm.imagename = documentsetting.updatefiles(employeevm.image, "images");
                    var employee1 = _mapper.Map<Employeeviewmodel,employee>(employeevm);
                    _unitofwork.employees.update(employee1);
                    if ( await _unitofwork.SaveChangesasync()> 0)
                        TempData["message"] = "update successfully";

                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(employeevm);
        }
        public async  Task<IActionResult> Delete(int? id) =>await  Employeecontrollerhandler(id, nameof(Delete));
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> confirmdelete(int ?id)
        {
            if (!id.HasValue) { return BadRequest(); }
            var repo1 = await _unitofwork.employees.Getasync(id.Value);
            if (repo1 is null) { return NotFound(); }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        _unitofwork.employees.Delete(repo1);
                        if(await _unitofwork.SaveChangesasync()>0&&repo1.imagename is not null)
                           documentsetting.deletefiles("images" ,repo1.imagename);
                        return RedirectToAction(nameof(Index));

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }

                return View(repo1);
            }
        }
        private async Task<IActionResult> Employeecontrollerhandler(int? id, string viewname)
        {
            if (viewname == nameof(Edit))
            {
                var department =await  _unitofwork.data.Getallasync();
                SelectList list = new SelectList(department, "id", "name");
                ViewBag.department = list;
            }
            if (!id.HasValue)
            {
                return BadRequest();
            }
            else
            {
                var Employee = await _unitofwork.employees.Getasync(id.Value);
                if (Employee is null)
                {
                    return NotFound();
                }
                else
                {
                    var empvm = _mapper.Map<Employeeviewmodel>(Employee);
                    return View(empvm);
                }
            }
        }
    }
}
