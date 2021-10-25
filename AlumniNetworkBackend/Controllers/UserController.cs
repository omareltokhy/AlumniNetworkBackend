﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlumniNetworkBackend.Models;
using AlumniNetworkBackend.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using AlumniNetworkBackend.Models.DTO;
using AlumniNetworkBackend.Models.DTO.UserDTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using AlumniNetworkBackend.Services;
using System.Net.Mime;

namespace AlumniNetworkBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UserController : ControllerBase
    {
        private readonly AlumniNetworkDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _service;

        public object HttsStatusCode { get; private set; }

        public UserController(AlumniNetworkDbContext context, IMapper mapper, IUserService service)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
        }

        /// <summary>
        /// Api endpoint api/users which returns the user info on registered user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserReadDTO>> GetUsers()
        {
            string userId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

            var user = await _service.GetUser(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserReadDTO>(user));
            
        }

        /// <summary>
        /// Api endpoint api/users/id which gives you user info by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserReadDTO>> GetUserById(string id)
        {
            var user = await _service.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserReadDTO>(user));
        }

        /// <summary>
        /// Api endpoint api/users/id which lets user make a partial update on existing user
        /// by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dtoUser"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserUpdateDTO>> UpdateUser(string id, UserUpdateDTO dtoUser)
        {
            User user = _mapper.Map<User>(dtoUser);
            var updatedUser = await _service.UpdateUser(id, user);

            if(updatedUser == null)
            {
                return Unauthorized();
            }

            return Ok(_mapper.Map<UserUpdateDTO>(updatedUser));
        }

        /// <summary>
        /// Api endpoint api/users which lets client to create a new user
        /// </summary>
        /// <param name="dtoUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<User>> PostUser(UserCreateDTO dtoUser)
        {
            User domainUser = _mapper.Map<User>(dtoUser);

            _context.Users.Add(domainUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = domainUser.Id}, _mapper.Map<UserReadDTO>(domainUser));
        }

    }
}
