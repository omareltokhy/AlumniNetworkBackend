﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlumniNetworkBackend.Models;
using AlumniNetworkBackend.Models.Domain;
using AutoMapper;
using AlumniNetworkBackend.Models.DTO.TopicDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using AlumniNetworkBackend.Models.DTO.UserDTO;
using AlumniNetworkBackend.Services;
using System.Net.Mime;

namespace AlumniNetworkBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class TopicController : ControllerBase
    {
        private readonly AlumniNetworkDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITopicService _service;

        public TopicController(ITopicService service, AlumniNetworkDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _service = service;
        }

        /// <summary>
        /// Endpoint api/Topics which returns a list of topics and their
        /// descriptions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<TopicReadDTO>>> GetTopics()
        {
            var allTopics = await _service.GetAllTopics();
            return _mapper.Map<List<TopicReadDTO>>(allTopics);
        }

        /// <summary>
        /// Endpoint api/Topics/{id} which returns a specific topic
        /// and its description
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TopicReadDTO>> GetTopic(int id)
        {
            var topic = await _service.GetTopicById(id);

            if (topic == null)
            {
                return NotFound();
            }

            return _mapper.Map<TopicReadDTO>(topic);
        }

        // <summary>
        /// Endpoint api/Topics which posts a new Topic to database with name
        /// and description.
        /// </summary>
        /// <param name="dtoTopic"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TopicCreateDTO>> PostTopic(TopicCreateDTO dtoTopic)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            User tempUser = await _context.Users.Where(u => u.Id == "1").FirstAsync();

            Topic newTopic = new() { Name = dtoTopic.Name, Description = dtoTopic.Description};
            var updatedTopic = await _service.Create(newTopic, tempUser);
            return _mapper.Map<TopicCreateDTO>(updatedTopic);
        }

        // POST: api/Topics
        [HttpPost("{id}/join")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TopicCreateMemberDTO>> PostTopicMember([FromRoute] int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

            if (!TopicExists(id))
            {
                return new StatusCodeResult(404);
            }
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new StatusCodeResult(404);
            }
  
            var updatedTopicUsers = _context.Topics.Where(t => t.Id == id)
                .SelectMany(u => u.Users)
                .Append(user);
            await _context.SaveChangesAsync();
            

            return Ok(updatedTopicUsers);
        }
        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
    }
}
