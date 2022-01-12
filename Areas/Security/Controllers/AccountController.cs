using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenzinger_Eshop_Web.Controllers;
using Zenzinger_Eshop_Web.Models.ApplicationServices.Abstraction;
using Zenzinger_Eshop_Web.Models.Entity.Identity;
using Zenzinger_Eshop_Web.Models.ViewModels;

namespace Zenzinger_Eshop_Web.Areas.Security.Controllers
{
    [Area("Security")]
    public class AccountController : Controller
    {
        ISecurityApplicationService security;
        
        public AccountController(ISecurityApplicationService security)
        {
            this.security = security;
        }
        
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                string[] errors = await security.Register(registerVM, Roles.Customer);

                if (errors == null)
                {
                    LoginViewModel loginVm = new LoginViewModel()
                    {
                        Username = registerVM.Username,
                        Password = registerVM.Password
                    };
                    return await Login(loginVm);
                }
                //return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", String.Empty), new {area = String.Empty});
            }
            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                loginVM.LoginFailed = !await security.Login(loginVM);
                if (loginVM.LoginFailed == false)
                {
                    return RedirectToAction(nameof(HomeController.Index),
                        nameof(HomeController).Replace("Controller", String.Empty), new {area = String.Empty});
                }
            }
            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            await security.Logout();
            return RedirectToAction(nameof(Login));
        }
    }
}
