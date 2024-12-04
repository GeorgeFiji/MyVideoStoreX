using Microsoft.AspNetCore.Mvc;
using MyVideostore.Data;
using MyVideostore.Models;


namespace MyVideostore.Controllers
{
    public class Login : Controller
    {
        public IActionResult LoginPage()
        {
            return View();
        }

       
    }
}
