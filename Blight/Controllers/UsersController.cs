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
using Microsoft.AspNetCore.Authorization;

namespace Blight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]

    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepos;

        public UsersController(IUserRepository userRepos)
        {
            _userRepos = userRepos;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return Ok(await _userRepos.GetAll());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var result = await _userRepos.GetById(id);

            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromBody] RegisterUserDto dto)
        {
            var result = await _userRepos.Create(dto);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="User,Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUserDto dto)
        {
            await _userRepos.Update(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepos.Delete(id);

            return NoContent();
        }

        [HttpPost("ban/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> UserBanStatus_Change(int id)
        {
            var result = await _userRepos.BanUser_Change(id);

            return Ok(result);
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Login([FromBody] LoginUserDto dto)
        {
            var result = await _userRepos.Login(dto);

            return Ok(result);
        }

    }
}
