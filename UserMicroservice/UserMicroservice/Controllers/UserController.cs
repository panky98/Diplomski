using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.UserMicroservice;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using UserMicroservice.Repositories;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public UserController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this._unitOfWork = unitOfWork;
            this._config = configuration;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            return new JsonResult(_unitOfWork.Users.GetAll());
        }

        [HttpPost]
        public IActionResult AddOne([FromBody]User newUser)
        {
            newUser.Password = Models.UserMicroservice.User.HashPassword(newUser.Password);
            if (_unitOfWork.Users.FindOneByExpression(x => x.Username == newUser.Username) != null)
                return BadRequest("Username already exists!");


            _unitOfWork.Users.Add(newUser);
            _unitOfWork.Commit();
            return new OkResult();
        }

        [HttpGet]
        [Route("LogIn/{username}/{password}")]
        public IActionResult LogIn([FromRoute(Name = "username")] string username, [FromRoute(Name = "password")] string pass)
        {
            int idUser = _unitOfWork.Users.Exist(username, pass);
            if (idUser!=-1)
            {
                var response = new TokenResponse()
                {
                    Token = GenerateJWTToken(_unitOfWork.Users.GetOne(idUser))
                };
                return Ok(response);
            }
            else
                return NotFound();
        }

        string GenerateJWTToken(User userInfo)
        {
            //IssuerSigningKey, to su bajtovi u sustini postavljenog security key-a
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            //nakon toga se vrsi kodiranje tih bajtova, i ta kodirana verzija se smeta u token
            //taj SecretKey je u sustini jedan stepen zastite, bez da se zna on, niko sa strane ne moze da generise pravi token!!
            //nakon prijave kad se salje token, prilikom authorizacije se taj proces verovatno obavlja obrnuto, vrsi se dekodiranje do niza bajtova koji predstavljaju SecretKey
            //i proverava se jel se poklapa sa ocekivanim secretkey-om
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //prateci podaci u tokenu
            var claims = new[]
            {
                   new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                   new Claim(JwtRegisteredClaimNames.NameId,userInfo.Id.ToString())
            };
            var token = new JwtSecurityToken(
            //drugi stepen zastite issuer
            issuer: _config["Jwt:Issuer"],
            //treci audience
            audience: _config["Jwt:Audience"],
            claims: claims,
            //duzina trajanja tokena
            expires: DateTime.Now.AddMinutes(30),
            //kljuc
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
