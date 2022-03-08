using Blight.Entieties;
using Blight.Interfaces;
using Blight.Models;
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
    public class PhoneNumbersController : ControllerBase
    {
        IPhoneNumberService _phoneNumberService;

        public PhoneNumbersController(IPhoneNumberService userService)
        {
            _phoneNumberService = userService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> GetAll(bool onlyBullyNumbers)
        {
            return Ok(await _phoneNumberService.GetAll(onlyBullyNumbers));
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhoneNumberDto>> Get(int id)
        {
            var result = await _phoneNumberService.Get(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult<PhoneNumber>> Post([FromBody] PhoneNumberDto dto)
        {
            var result = await _phoneNumberService.Post(dto);

            if (result == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);
        }


        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _phoneNumberService.Delete(id);

            if (!result)
            {
                BadRequest();
            }
            return NoContent();
        }
    }
}
