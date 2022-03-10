using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Interfaces;
using Blight.Models;
using Blight.Infrastructure;
using NLog;
using Microsoft.Extensions.Logging;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Blight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            _logger.LogInformation("GetAll-Users method is called");
            return Ok(await _userService.GetAll());
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            _logger.LogInformation($"Get-User{id} method is called");
            var result = await _userService.Get(id);
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] UserDto dto)
        {
            _logger.LogInformation($"Create User method is called");

            var result = await _userService.Post(dto);

            if (result == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto dto)
        {
            _logger.LogInformation($"Update User{id} method is called");

            if (dto is null)
            {
                return BadRequest();
            }

            var isUpdate = await _userService.Put(id, dto);
            if(!isUpdate)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Delete User{id} method is called");

            var result = await _userService.Delete(id);

            if(!result)
            {
                BadRequest();
            }
            return NoContent();
        }
    }
}
