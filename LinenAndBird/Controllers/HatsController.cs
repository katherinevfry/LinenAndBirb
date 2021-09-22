using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinenAndBird.Models;
using LinenAndBird.DataAccess;

namespace LinenAndBird.Controllers
{
    [Route("api/hats")] //exposed at this endpoint
    [ApiController] // an api controller so it returs json or xml
    public class HatsController : ControllerBase
    {

        HatRepository _repo;

        public HatsController(HatRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public List<Hat> GetAllHats()
        {
            return _repo.GetAll();
        }

        [HttpGet("styles/{style}")] //whats inside curly braces needs to match the parameter
        public IActionResult GetHatsByStyle(HatStyle style)
        {
            var matches = _repo.GetByStyle(style);
            return Ok(matches);
        }

        [HttpPost]
        public IActionResult AddAHat(Hat newHat)
        {
            _repo.Add(newHat);
            return Created($"/api/birds/{newHat.Id}", newHat);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteHat(Guid id)
        {
            _repo.Remove(id);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateHat(Guid id, Hat hat)
        {
            var hatToUpdate = _repo.GetById(id);

            if (hatToUpdate == null)
            {
                return NotFound($"Could not find hat with id {id} for updating");
            }

            var updatedHat = _repo.Update(id, hat);


            return Ok(updatedHat);
        }
    }
}
