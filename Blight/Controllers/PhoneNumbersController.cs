using Blight.Entieties;
using Blight.Interfaces;
using Blight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Blight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PhoneNumbersController : ControllerBase
    {
        private readonly IPhoneRepository _phoneNumberRepos2;

        public PhoneNumbersController(IPhoneRepository phoneNumberRepos2)
        {
            _phoneNumberRepos2 = phoneNumberRepos2;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> GetAll([FromQuery] PaginationPhoneQuery paginationQuery)
        {
            return Ok(await _phoneNumberRepos2.GetAll(paginationQuery));
        }

        [HttpGet]
        [Route("getBlockedNumbers")]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> GetUserBlockedNumbers([FromQuery] PaginationPhoneQuery paginationQuery)
        {
            return Ok(await _phoneNumberRepos2.GetUserAllBlockedNumbers(paginationQuery));
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        [Route("setIsBullyTreshold/{id}")]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> SetIsBullyTreshold(int id,[FromQuery] int treshold)
        {
            return Ok(await _phoneNumberRepos2.SetIsBullyTreshold(id,treshold));
        }

        //[HttpPost]
        //[Route("syncBlockedNumbers")]
        //public async Task<ActionResult<IEnumerable<PhoneNumber>>> SyncBlockedNumbers([FromBody] object jsonBlockedNumbers)
        //{
        //    return Ok(await _phoneNumberRepos2.GetUserAllBlockedNumbers(paginationQuery));
        //}


        [HttpGet("{id}")]
        public async Task<ActionResult<PhoneNumberDto>> Get(int id)
        {
            var result = await _phoneNumberRepos2.GetById(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PhoneNumber>> Post([FromBody] PhoneNumberDto dto)
        {
            var result = await _phoneNumberRepos2.Create(dto);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _phoneNumberRepos2.Delete(id);

            return NoContent();
        }
    }
}
