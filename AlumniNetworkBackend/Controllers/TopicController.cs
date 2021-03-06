using System;
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
using Microsoft.AspNetCore.Cors;

namespace AlumniNetworkBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]

    [ApiConventionType(typeof(DefaultApiConventions))]
    public class TopicController : ControllerBase
    {
        private readonly AlumniNetworkDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITopicService _service;

        public TopicController(ITopicService service, IMapper mapper, AlumniNetworkDbContext context)
        {
            _mapper = mapper;
            _service = service;
            _context = context;
        }

        /// <summary>
        /// Endpoint api/Topics which returns a list of topics and their
        /// descriptions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TopicReadDTO>>> GetTopics()
        {
            List<Topic> topics = await _service.GetAllTopics();
            return _mapper.Map<List<TopicReadDTO>>(topics);
        }

        /// <summary>
        /// Endpoint api/Topics/{id} which returns a specific topic
        /// and its description
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TopicReadDTO>> GetTopic(int id)
        {
            Topic topic = await _service.GetTopicById(id);

            if (topic == null)
            {
                return NotFound(null);
            }

            return _mapper.Map<TopicReadDTO>(topic);
        }

        /// <summary>
        /// Endpoint api/Topics which posts a new Topic to database with name
        /// and description.
        /// </summary>
        /// <param name="dtoTopic"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TopicCreateDTO>> PostTopic(TopicCreateDTO dtoTopic)
        {
            string userId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value; // will give the user's userId

            Topic newTopic = new() { Name = dtoTopic.Name, Description = dtoTopic.Description};
            var updatedTopic = await _service.Create(newTopic, userId);

            if (updatedTopic == null)
            {
                return NotFound(null);
            }

            return CreatedAtAction("GetTopic", new { id = updatedTopic.Id }, _mapper.Map<TopicCreateDTO>(updatedTopic));
        }
        /// <summary>
        /// Creates a new topic membership for the requesting user to referenced Topic.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: api/Topics
        [HttpPost("{id}/join")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TopicCreateMemberDTO>> PostTopicMember([FromRoute] int id)
        {
            string userId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

            Topic topicToUpdate = await _context.Topics
                .Include(t => t.Users)
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (topicToUpdate == null)
                return NotFound(null);

            var topicNewUserList = await _service.AddUserToTopic(topicToUpdate, userId);

            if (topicNewUserList == null)
                return NotFound(null);

            return Ok(_mapper.Map<TopicCreateMemberDTO>(topicNewUserList));
        }
    }
}
