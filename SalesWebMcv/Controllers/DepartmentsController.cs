using Microsoft.AspNetCore.Mvc;
using SalesWebMcv.Models;

namespace SalesWebMcv.Controllers {

    public class DepartmentsController : Controller {

        public IActionResult Index() {
            List<Department> departments = new List<Department> {
                new Department { Id = 1, Name = "Eletronics" },
                new Department { Id = 2, Name = "Fashion" }
            };

            return View(departments);
        }
    }
}
