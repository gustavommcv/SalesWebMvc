using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SalesWebMvc.Controllers {
    public class SellersController : Controller {

        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService) {
            _sellerService = sellerService; _departmentService = departmentService;
        }

        public async Task<IActionResult> Index() {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create() {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller) {

            bool isNameValid = ValidateProperty(seller, nameof(Seller.Name));
            bool isEmailValid = ValidateProperty(seller, nameof(Seller.Email));
            bool isBirthDateValid = ValidateProperty(seller, nameof(Seller.BirthDate));
            bool isBaseSalaryValid = ValidateProperty(seller, nameof(Seller.BaseSalary));

            bool isValid = isNameValid && isEmailValid && isBirthDateValid && isBaseSalaryValid;

            if (!isValid) {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id) {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); };

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            try {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e) {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        public async Task<IActionResult> Details(int? id) {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); };

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); };

            return View(obj);
        }

        public async Task<IActionResult> Edit(int? id) {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments};

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller) {
            bool isNameValid = ValidateProperty(seller, nameof(Seller.Name));
            bool isEmailValid = ValidateProperty(seller, nameof(Seller.Email));
            bool isBirthDateValid = ValidateProperty(seller, nameof(Seller.BirthDate));
            bool isBaseSalaryValid = ValidateProperty(seller, nameof(Seller.BaseSalary));

            bool isValid = isNameValid && isEmailValid && isBirthDateValid && isBaseSalaryValid;

            if (!isValid) { 
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel); 
            }

            if (id != seller.Id) { return RedirectToAction(nameof(Error), new { message = "Id mismatch" }); }

            try {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e) {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        private bool ValidateProperty(object model, string propertyName) {
            var context = new ValidationContext(model, serviceProvider: null, items: null) { MemberName = propertyName };
            var results = new List<ValidationResult>();
            return Validator.TryValidateProperty(model.GetType().GetProperty(propertyName).GetValue(model), context, results);
        }

        public IActionResult Error(string message) {
            var viewModel = new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            return View(viewModel);
        }
    }
}
