﻿using Blight.Entieties;
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
        private readonly IGenericRepository<PhoneNumber> _phoneNumberRepos;

        public PhoneNumbersController(IGenericRepository<PhoneNumber> phoneNumberRepos)
        {
            _phoneNumberRepos = phoneNumberRepos;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> GetAll(bool onlyBullyNumbers)
        {
            if (onlyBullyNumbers)
            {
                return Ok(await _phoneNumberRepos.GetAll(c => c.IsBully == true));
            }
            return Ok(await _phoneNumberRepos.GetAll());
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhoneNumberDto>> Get(int id)
        {
            var result = await _phoneNumberRepos.GetById(id);

            return Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult<PhoneNumber>> Post([FromBody] PhoneNumberDto dto)
        {
            var result = await _phoneNumberRepos.Create(dto);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _phoneNumberRepos.Delete(id);

            return NoContent();
        }
    }
}
