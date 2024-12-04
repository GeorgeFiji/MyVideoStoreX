using Microsoft.AspNetCore.Mvc;
using MyVideostore.Data;
using MyVideostore.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyVideostore.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the database context
        public ContactController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Contact/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Display the contact form
        }

        // POST: Contact/Index
        [HttpPost]
        public async Task<IActionResult> Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Save the contact information to the database
                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync();

                    // Send email to the user
                    await SendEmailAsync(contact);

                    // Set the success message in TempData
                    TempData["SuccessMessage"] = "Your message has been sent successfully! We'll get back to you as soon as possible.";

                    // Redirect to the same page to show the success message
                    return RedirectToAction("Index");
                }
                catch (SmtpException smtpEx)
                {
                    ModelState.AddModelError(string.Empty, $"Email sending error: {smtpEx.Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
                }
            }

            // If validation fails, return to the form with validation errors
            return View(contact);
        }

        private async Task SendEmailAsync(Contact contact)
        {
            try
            {
                // Configure the SMTP client
                using var client = new SmtpClient("live.smtp.mailtrap.io", 587)
                {
                    Credentials = new NetworkCredential("api", "3544eabe8e1156a89b40e1c9c623310e"),
                    EnableSsl = true
                };

                // Create the email message
                var mail = new MailMessage
                {
                    From = new MailAddress("hello@demomailtrap.com", "MyVideoStore"),
                    Subject = "Thank you for contacting us!",
                    Body = $"Hello {contact.Name},\n\nThank you for reaching out to MyVideoStore!\n\n The message you have sent below is stored and we will replying to you immediately:\n\n{contact.Message}\n\nWe'll get back to you as soon as possible.\n\nBest regards,\nThe MyVideoStore Team",
                    IsBodyHtml = false
                };

                // Add the recipient's email address
                mail.To.Add(contact.Email);

                // Send the email
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
