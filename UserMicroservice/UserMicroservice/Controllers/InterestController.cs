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
    public class InterestController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public InterestController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return new JsonResult(_unitOfWork.Interests.GetAll());
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetOne([FromRoute(Name ="id")]int id)
        {
            return new JsonResult(_unitOfWork.Interests.GetOne(id));
        }
        [HttpPost]
        public IActionResult PostOne([FromBody]Interest newInterest)
        {
            this._unitOfWork.Interests.Add(newInterest);
            this._unitOfWork.Commit();
            return Ok();
        }
    }
}
