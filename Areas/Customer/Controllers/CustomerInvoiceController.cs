using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenzinger_Eshop_Web.Models.ApplicationServices.Abstraction;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.Entity;
using Zenzinger_Eshop_Web.Models.Entity.Identity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using IronPdf;

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
            var emailAddress = "zenzinger.eshop@gmail.com";
            var emailAddressPassword = "hovnokleslo";

            var subject = $"Order: {order.OrderNumber}";

            var body = $@"
                <html lang=""en"">
                <head>
                </head>
                <body>
                <div>
                   <p>Hello {order.User.FirstName} {order.User.LastName},<p>
                   <br>
                   <p>your order was successful, you can find your <i>Invoice #{order.Id}</i> with total price of <strong>$ {order.TotalPrice}</strong>.<br>
                     Below you can find attached invoice or you can create it manually on our website in section <i>My Orders</i>.<p>
                   <br>
                   <p>Thank you for ordering.<br><br>
                    Your <b>Zenzinger Eshop</b></p>
                </div>
                </body>
                </html>
                ";

            var server = "smtp.gmail.com";
            var port = 587;
            
            MailAddress to = new MailAddress(currentUser.Email);
            MailAddress from = new MailAddress(emailAddress);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            
            //PDF File creating from private method below
            MemoryStream stream = CreatePdf(order);
            mail.Attachments.Add(new Attachment(stream, "faktura.pdf", "app/pdf"));
            
            SmtpClient client = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(emailAddress, emailAddressPassword),
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

        private static MemoryStream CreatePdf(Order order)
        {
            //<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">
            var stringBuilder = new StringBuilder();
            
            stringBuilder.Append("<title>Invoice " + order.Id + "</title>");
            stringBuilder.Append("<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>");
            stringBuilder.Append("<section class='container-fluid'>");
            stringBuilder.Append("<div id='ui-view' data-select2-id='ui-view'>");
            stringBuilder.Append("<div>");
            stringBuilder.Append("<div class='card'>");
            // CARD - HEADER
            stringBuilder.Append("<div class='card-header'>Invoice");
            stringBuilder.Append("<strong>" + order.Id + "</strong>");
            stringBuilder.Append("</div>");
            // CARD - BODY
            stringBuilder.Append("<div class='card-body'>");
            stringBuilder.Append("<div class='row mb-4'");
            stringBuilder.Append("<div class='col-sm-4'");
            stringBuilder.Append("<h6 class='mb-3'>From:</h6>");    // FROM:
            stringBuilder.Append("<div>");
            stringBuilder.Append("<strong>Zenzinger_Eshop_Web</strong>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<p>42, Zlinskeho</p>");
            stringBuilder.Append("<p>Zlin City, New Zlin, 10394</p>");
            stringBuilder.Append("<p>Email: zenzinger.eshop@gmail.com</p>");
            stringBuilder.Append("<p>Phone: +48 123 456 789</p>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class='col-sm-4'>");     // TO:
            stringBuilder.Append("<h6 class='mb-3'>To:</h6>");
            stringBuilder.Append("<div>");
            stringBuilder.Append("<strong>" + order.User.FirstName + " " + order.User.LastName + "</strong>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<p>Address</p>");
            stringBuilder.Append("<p>City Address</p>");
            stringBuilder.Append("<p>Email: " + order.User.Email + "</p>");
            stringBuilder.Append("<p>Phone: " + order.User.PhoneNumber + "</p>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class='col-sm-4'>");     // DETAILS:
            stringBuilder.Append("<h6 class='mb-4'>Details:</h6>");
            stringBuilder.Append("<div>Invoice <strong>" + order.Id + "</strong></div>");
            stringBuilder.Append("<p>" + order.DateTimeCreated + "</p>");
            stringBuilder.Append("<p>Account name: " + order.User.UserName + "</p>");
            stringBuilder.Append("<p>Order number: <strong>" + order.OrderNumber + "</strong></p>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            // TABLE-RESPONSIVE
            stringBuilder.Append("<div class='table-responsive-sm'>");
            stringBuilder.Append("<table class='table table-stripped'>");
            stringBuilder.Append("<thead>");        // tHEADER
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th class='center'>#</th>");
            stringBuilder.Append("<th>Product name</th>");
            stringBuilder.Append("<th>Description</th>");
            stringBuilder.Append("<th class='right'>Unit cost</th>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("</thead>");
            stringBuilder.Append("<tbody>");        // tBODY
            int i = 0;
            foreach (var item in order.OrderItems)
            {
                i++;
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<td class='center'>" + i + "</td>");
                stringBuilder.Append("<td class='left'>" + item.Product.Info + "</td>");
                stringBuilder.Append("<td class='left'>" + item.Product.Name + "</td>");
                stringBuilder.Append("<td class='right'>" + item.Product.Price + "</td>");
                stringBuilder.Append("</tr>");
            }
            stringBuilder.Append("</tr>");
            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class='row'");
            stringBuilder.Append("<div class='col-lg-4 col-sm-5'>Lorem Thanksium.</div");
            stringBuilder.Append("<div class='col-lg-4 col-sm-5 ml-auto'>");
            stringBuilder.Append("<table class=''table table-clear'>");
            stringBuilder.Append("<tbody>");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<td class='left'>");
            stringBuilder.Append("<strong>Total</strong>");
            stringBuilder.Append("</td>");
            stringBuilder.Append("<td class='right'>");
            stringBuilder.Append("<strong>$ " + order.TotalPrice + "</strong>");
            stringBuilder.Append("</td>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("</tbody>");
            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</section>");

            var render = new IronPdf.ChromePdfRenderer();
            var memoryStream = render.RenderHtmlAsPdf(stringBuilder.ToString()).Stream;

            return memoryStream;
        }
    }
}