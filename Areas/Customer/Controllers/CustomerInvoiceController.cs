using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenzinger_Eshop_Web.Models.ApplicationServices.Abstraction;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.Entity;
using Zenzinger_Eshop_Web.Models.Entity.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Zenzinger_Eshop_Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = nameof(Roles.Customer))]
    public class CustomerInvoiceController : Controller
    {
        EshopDbContext eshopDbContext;

        public CustomerInvoiceController(EshopDbContext EshopDbContext)
        {
            eshopDbContext = EshopDbContext;
        }
        // TO-DO: Create version with adding attachments in pdf format
        public static async Task MailSender(Order order)
        {
            var currentUser = order.User;
            var adminMail = "zenzinger.eshop@gmail.com";
            var adminMailPass = "hovnokleslo";
            
            var subject = $"Order: {order.OrderNumber}";
            
            var body = $@"
                <html lang=""en"">
                <head>
                </head>
                <body>
                <div class=""card"">
                   <div class=""card-header"">Hello {order.User.FirstName} {order.User.LastName},<div>
                   <br>
                   <div class=""card-body"">your order was successful, you can find your <i>Invoice #{order.Id}</i> with total price of <strong>$ {order.TotalPrice}</strong> at our website in section <i>My Orders</i>.</div>
                   <br>
                   <div class=""card-footer"">Thank you for ordering from <b>Zenzinger.Eshop</b></div>
                </div>
                </body>
                </html>
                ";
            
            var server = "smtp.gmail.com";
            var port = 587;
            
            MailAddress to = new MailAddress(currentUser.Email);
            MailAddress from = new MailAddress(adminMail);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            
            SmtpClient client = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(adminMail, adminMailPass),
                EnableSsl = true
            };
            try
            { 
                client.Send(mail);
                Console.WriteLine("Email sent.");
            }
            catch (SmtpException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // GET
        public async Task<IActionResult> Invoice(int id)
        {
            var foundItem =  eshopDbContext.Orders
                .Include(o=>o.User)
                .Include(o=>o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);
            return await (foundItem != null ? Task.FromResult<IActionResult>(View(foundItem)) : Task.FromResult<IActionResult>(NotFound())); 
        }
    }
}