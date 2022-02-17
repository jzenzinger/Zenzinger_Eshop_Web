using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zenzinger_Eshop_Web.Areas.Admin.Controllers;
using Zenzinger_Eshop_Web.Models.ApplicationServices.Abstraction;
using Zenzinger_Eshop_Web.Models.Database;
using Zenzinger_Eshop_Web.Models.Entity;
using Zenzinger_Eshop_Web.Models.Entity.Identity;

namespace Zenzinger_Eshop_Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = nameof(Roles.Customer))]
    public class CustomerOrderNotCartController : Controller
    {
        const string totalPriceString = "TotalPrice";
        const string orderItemsString = "OrderItems";


        ISecurityApplicationService iSecure;
        EshopDbContext EshopDbContext;
        public CustomerOrderNotCartController(ISecurityApplicationService iSecure, EshopDbContext eshopDBContext)
        {
            this.iSecure = iSecure;
            EshopDbContext = eshopDBContext;
        }


        [HttpPost]
        public double AddOrderItemsToSession(int? productId)
        {
            double totalPrice = 0;
            if (HttpContext.Session.IsAvailable)
            {
                totalPrice = HttpContext.Session.GetDouble(totalPriceString).GetValueOrDefault();
            }


            Product product = EshopDbContext.Products.Where(prod => prod.ID == productId).FirstOrDefault();

            if (product != null)
            {
                OrderItem orderItem = new OrderItem()
                {
                    //Name = product.Info,
                    ProductId = product.ID,
                    Product = product,
                    Amount = 1,
                    Price = product.Price   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                };

                if (HttpContext.Session.IsAvailable)
                {

                    List<OrderItem> orderItems = HttpContext.Session.GetObject<List<OrderItem>>(orderItemsString);
                    OrderItem orderItemInSession = null;
                    if (orderItems != null)
                        orderItemInSession = orderItems.Find(oi => oi.ProductId == orderItem.ProductId);
                    else
                        orderItems = new List<OrderItem>();


                    if (orderItemInSession != null)
                    {
                        ++orderItemInSession.Amount;
                        orderItemInSession.Price += orderItem.Product.Price;   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                    }
                    else
                    {
                        orderItems.Add(orderItem);
                    }
                    HttpContext.Session.SetObject(orderItemsString, orderItems);

                    totalPrice += orderItem.Product.Price;
                    HttpContext.Session.SetDouble(totalPriceString, totalPrice);
                }
            }

            return totalPrice;
        }


        public async Task<IActionResult> ApproveOrderInSession()
        {
            if (HttpContext.Session.IsAvailable)
            {
                double totalPrice = 0;
                List<OrderItem> orderItems = HttpContext.Session.GetObject<List<OrderItem>>(orderItemsString);
                if (orderItems != null)
                {
                    foreach (OrderItem orderItem in orderItems)
                    {
                        totalPrice += orderItem.Product.Price * orderItem.Amount;
                        orderItem.Product = null; //zde musime nullovat referenci na produkt, jinak by doslo o pokus jej znovu vlozit do databaze
                    }

                    User currentUser = await iSecure.GetCurrentUser(User);

                    Order order = new Order()
                    {
                        OrderNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                        TotalPrice = totalPrice,
                        OrderItems = orderItems,
                        UserId = currentUser.Id
                    };

                    //We can add just the order; order items will be added automatically.
                    await EshopDbContext.AddAsync(order);
                    await EshopDbContext.SaveChangesAsync();
                    
                    //await CustomerInvoiceController.MailSender(order);    // Await

                    HttpContext.Session.Remove(orderItemsString);
                    HttpContext.Session.Remove(totalPriceString);
                    
                    // Create PDF File for sending by email
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
                        var product = await EshopDbContext.Products.FirstOrDefaultAsync(p => p.ID == item.ProductId);
                        i++;
                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("<td class='center'>" + i + "</td>");
                        stringBuilder.Append("<td class='left'>" + product.Info + "</td>");
                        stringBuilder.Append("<td class='left'>" + product.Name + "</td>");
                        stringBuilder.Append("<td class='right'>" + item.Price + "</td>");
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
                    
                    // Sending EMAIL with pdf attachments
                    var emailUser = order.User;
                    var emailAddress = "";
                    var emailAddressPassword = "";

                    var subject = $"Order: {order.OrderNumber}";

                    var body = $@"
                        <html lang=""en"">
                        <head>
                        </head>
                        <body>
                        <div>
                           <p>Hello {order.User.FirstName} {order.User.LastName},<p>
                           <br>
                           <p>your order <i>Invoice #{order.Id}</i> with total price of <strong>$ {order.TotalPrice}</strong> was successful.<br>
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
                    
                    MailAddress to = new MailAddress(emailUser.Email);
                    MailAddress from = new MailAddress(emailAddress);
                    MailMessage mail = new MailMessage(from, to);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    
                    //PDF File creating from private method below
                    MemoryStream stream = memoryStream;
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

                    return RedirectToAction(nameof(CustomerOrdersController.Index), nameof(CustomerOrdersController).Replace("Controller", ""), new { Area = nameof(Customer) });

                }
            }
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""), new { Area = "" });
        }
    }
}
