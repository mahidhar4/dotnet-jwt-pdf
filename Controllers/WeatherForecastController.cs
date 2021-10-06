using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnet.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using Microsoft.Extensions.Hosting;

namespace dotnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DbMemContext _context;
        private IConfiguration _config;
        private readonly IHostEnvironment _HostEnvironment;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DbMemContext context, IConfiguration config, IHostEnvironment hosting)
        {
            _logger = logger;
            _context = context;

            _config = config;
            _HostEnvironment = hosting;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<User> Get()
        {
            return _context.Users.ToArray();
        }

        [HttpPost]
        [Authorize]

        public User Insert([FromBody] User user)

        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/login")]
        public IActionResult Login([FromBody] User login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User AuthenticateUser(User login)
        {
            User user = null;

            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            if (login.Username == "Jignesh")
            {
                user = new User { Username = "Jignesh Trivedi", EmailAddress = "test.btest@gmail.com" };
            }
            return user;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("/pdf")]
        public IActionResult createPdf([FromBody] List<Paths> pathsImages)
        {

            string fileName = $"myfiles/hello{DateTime.Now.ToString("dd-MM-yyyyHH:mm:ss")}.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream($"{_HostEnvironment.ContentRootPath}/{fileName}", FileMode.Create, FileAccess.Write)));
            Document document = new Document(pdfDocument, iText.Kernel.Geom.PageSize.LETTER);
            int page = 0;

            foreach (var item in pathsImages)
            {
                ImageData imageData = ImageDataFactory.Create(item.FilePath);
                Image image = new Image(imageData);
                image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth() - 50);
                image.SetAutoScaleHeight(true);

                document.Add(image);
                page++;
                if (page != pathsImages.Count())
                {
                    document.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                }
            }


            document.Close();

            pdfDocument.Close();

            return Ok(fileName);
        }
    }
}


/*
Questions to ask:
will there be product team
Work from ofce policy -> any schedule ?
will there be career opportunities 
to move to JAVA?



hire quality people
lot of ownership
team size : 2-4 max
e2e product life cycle
after 2008 crisis
engineering driven is on business seat
forums, product standardization group
core engg - cloudification
-> compute, scalability
-> automate the process
-> 30yrs back
-> proprietary system
-> financial services cloud products
-> Fitness Subsidy Pro










*/
