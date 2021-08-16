using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;
using UserMicroservice.Repositories;

namespace UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult getAll()
        {
            return new JsonResult(_unitOfWork.Users.GetAll());
        }

        [HttpPost]
        public IActionResult AddOne([FromBody]User newUser)
        {
            newUser.Password = Models.User.HashPassword(newUser.Password);
            _unitOfWork.Users.Add(newUser);
            _unitOfWork.Commit();
            return new OkResult();
        }

        [HttpGet]
        [Route("LogIn/{username}/{password}")]
        public IActionResult LogIn([FromRoute(Name = "username")] string username, [FromRoute(Name = "password")] string pass)
        {
            if (_unitOfWork.Users.Exist(username, pass))
                return Ok();
            else
                return NotFound();
        }
    }
}
