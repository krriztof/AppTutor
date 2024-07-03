// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AppTutor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.EntityFrameworkCore;
using AppTutor.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppTutor.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            [Required]
            [Display(Name = "Imię")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Display(Name = "Dzień")]
            public int DayOfBirth { get; set; }
            [Display(Name = "Miesiąc")]
            public int MonthOfBirth { get; set; }
            [Display(Name = "Rok")]
            public int YearOfBirth { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Data urodzenia")]
            public DateTime DateOfBirth { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Numer telefonu")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} musi mieć co najmiej {2} znaków", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasła się nie zgadzają.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Specjalizacja")]
            public string Specialization { get; set; }

            [Display(Name = "Dokument potwierdzający kwalifikacje")]
            public IFormFile Document { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null, string role = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            var days = Enumerable.Range(1, 31).Select(d => new SelectListItem { Value = d.ToString(), Text = d.ToString() }).ToList();
            ViewData["Days"] = new SelectList(days, "Value", "Text");


            var months = Enumerable.Range(1, 12).Select(m => new SelectListItem { Value = m.ToString(), Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m) }).ToList();
            ViewData["Months"] = new SelectList(months, "Value", "Text");


            int currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(currentYear - 100, 101).Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() }).ToList();
            ViewData["Years"] = new SelectList(years, "Value", "Text");

            ViewData["Specializations"] = new SelectList(new List<string>
                {
                    "Biologia", "Chemia", "Fizyka", "Geografia", "Historia", "Informatyka", "Język Polski", "Język Angielski", "Matematyka"
                });


            if (!string.IsNullOrWhiteSpace(role) && (role == "Teacher" || role == "Student"))
            {
                Input = new InputModel
                {
                    Role = role
                };
            }
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, Input.Role);
                    user.DateOfBirth = new DateTime(Input.YearOfBirth, Input.MonthOfBirth, Input.DayOfBirth);
                    if (Input.Role == "Teacher" && Input.Document != null)
                    {
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "documents", Input.Document.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Input.Document.CopyToAsync(fileStream);
                        }
                        if (Input.Role == "Teacher" && Input.Document != null)
                        {
                            var teacher = new Teacher
                            {
                                UserId = user.Id,
                                Specialization = Input.Specialization,
                                DocumentPath = filePath,
                                IsApproved = false
                            };
                            _context.Teachers.Add(teacher);
                        }
                    }
                    else if (Input.Role == "Student")
                    {
                        var student = new Student { UserId = user.Id };
                        _context.Students.Add(student);
                    }
                    else
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["RegistrationSuccess"] = "Dziękujemy za utworzenie konta. Proszę się zalogować.";
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                var user = Activator.CreateInstance<ApplicationUser>();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.DateOfBirth = Input.DateOfBirth;
                user.PhoneNumber = Input.PhoneNumber;
                user.Role = Input.Role;
                return user;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
