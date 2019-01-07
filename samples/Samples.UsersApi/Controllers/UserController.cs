using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Samples.UsersApi.Database;

namespace Samples.UsersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(UserContext db, ILogger<UserController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            var allUsers = _db.Users.ToList();
            return Ok(allUsers);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            _logger.LogDebug("Getting user by {id}", id);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var user = _db.Users.FirstOrDefault(a => a.Id == id);

            return Ok(user);
        }

        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            try
            {
                _logger.LogInformation("Creating user {first_name}", user.FirstName);

                _db.Users.Add(user);
                _db.SaveChanges();
                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create user with {first_name}", user.FirstName);
                return BadRequest("Failed to create user");
            }
        }
    }
}
