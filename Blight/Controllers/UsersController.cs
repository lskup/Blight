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


namespace Blight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepos;

        public UsersController(IGenericRepository<User> userRepos)
        {
            _userRepos = userRepos;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return Ok(await _userRepos.GetAll());
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var result = await _userRepos.GetById(id);

            return Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Post([FromBody] RegisterUserDto dto)
        {
            var result = await _userRepos.Create(dto);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUserDto dto)
        {
            await _userRepos.Update(id, dto);

            return NoContent();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepos.Delete(id);

            return NoContent();
        }
    }
}
