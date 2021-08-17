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
    public class InterestByUserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InterestByUserController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(this._unitOfWork.InterestsByUsers.GetAll());
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetOne([FromRoute(Name = "id")] int id)
        {
            InterestByUser retVal = _unitOfWork.InterestsByUsers.GetOne(id);
            return retVal == null ? NotFound() : new JsonResult(retVal);
        }
        [HttpPost]
        public IActionResult PostOne([FromBody]InterestByUser newRow)
        {
            if (newRow.UserId == 0 || newRow.InterestId == 0)
                return BadRequest();

            _unitOfWork.InterestsByUsers.Add(newRow);
            _unitOfWork.Commit();
            return Ok();
        }
    }
}
