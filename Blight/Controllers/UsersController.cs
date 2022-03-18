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
        private readonly IGenericRepository<User> _userRepos;

        public UsersController(IUserService userService, IGenericRepository<User> userRepos)
        {
            _userService = userService;
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
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] UserDto dto)
        {
            var result = await _userRepos.Create(dto);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto dto)
        {
            await _userRepos.CreateOrUpdate(id, dto);

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

            var result = await _userService.Delete(id);

            if(!result)
            {
                BadRequest();
            }
            return NoContent();
        }
    }
}
