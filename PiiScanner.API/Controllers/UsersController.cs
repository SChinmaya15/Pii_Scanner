using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using PiiScanner.Domain.Entity;
using PiiScanner.Domain.Entity.Request;

namespace CyberHound_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Simple in-memory store for demonstration. Replace with real persistence.
        private static readonly List<UserDto> _store = new()
 {
 new UserDto { Id = Guid.NewGuid(), Username = "alice", Email = "alice@example.com", FullName = "Alice Example", Password = "Password123!", CreatedAt = DateTime.UtcNow },
 new UserDto { Id = Guid.NewGuid(), Username = "bob", Email = "bob@example.com", FullName = "Bob Example", Password = "Password123!", CreatedAt = DateTime.UtcNow }
 };

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAll()
        {
            return Ok(_store.Select(u => WithoutPassword(u)));
        }

        // GET: api/users/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<UserDto> GetById(Guid id)
        {
            var user = _store.FirstOrDefault(x => x.Id == id);
            if (user == null) return NotFound();
            return Ok(WithoutPassword(user));
        }

        //// POST: api/users
        //[HttpPost]
        //public ActionResult<UserDto> Create([FromBody] CreateUserRequest request)
        //{
        //    if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        //        return BadRequest("Request is invalid.");

        //    // Note: Password handling should use secure hashing in real apps.
        //    var created = new UserDto
        //    {
        //        Id = Guid.NewGuid(),
        //        Username = request.Username,
        //        Email = request.Email,
        //        FullName = request.FullName,
        //        Password = request.Password,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _store.Add(created);

        //    return CreatedAtAction(nameof(GetById), new { id = created.Id }, WithoutPassword(created));
        //}

        //// PUT: api/users/{id}
        //[HttpPut("{id:guid}")]
        //public ActionResult Update(Guid id, [FromBody] UpdateUserRequest request)
        //{
        //    if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
        //        return BadRequest("Request is invalid.");

        //    var existing = _store.FirstOrDefault(x => x.Id == id);
        //    if (existing == null) return NotFound();

        //    existing.Username = request.Username;
        //    existing.Email = request.Email;
        //    existing.FullName = request.FullName;

        //    return NoContent();
        //}

        // PATCH: api/users/{id}
        // Accepts partial update as a dictionary of property -> value
        [HttpPatch("{id:guid}")]
        public ActionResult PartialUpdate(Guid id, [FromBody] Dictionary<string, object> updates)
        {
            if (updates == null || updates.Count == 0)
                return BadRequest("No updates provided.");

            var existing = _store.FirstOrDefault(x => x.Id == id);
            if (existing == null) return NotFound();

            foreach (var kv in updates)
            {
                var key = kv.Key?.ToLowerInvariant();
                var value = kv.Value;

                switch (key)
                {
                    case "username":
                        existing.Username = value?.ToString() ?? existing.Username;
                        break;
                    case "email":
                        existing.Email = value?.ToString() ?? existing.Email;
                        break;
                    case "fullname":
                    case "fullName":
                        existing.FullName = value?.ToString() ?? existing.FullName;
                        break;
                    case "password":
                        // In real apps, hash the password. Demo only.
                        existing.Password = value?.ToString() ?? existing.Password;
                        break;
                        // ignore unknown fields
                }
            }

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:guid}")]
        public ActionResult Delete(Guid id)
        {
            var existing = _store.FirstOrDefault(x => x.Id == id);
            if (existing == null) return NotFound();

            _store.Remove(existing);
            return NoContent();
        }

        // POST: api/users/login
        [HttpPost("login")]
        public ActionResult Login([FromBody] Login request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Invalid login request.");

            var user = _store.FirstOrDefault(u => string.Equals(u.Username, request.Username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return Unauthorized();

            // Demo password check. Replace with hashed password verification.
            if (user.Password != request.Password) return Unauthorized();

            // Create claims
            var claims = new List<Claim>
 {
 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
 new Claim(ClaimTypes.Name, user.Username),
 new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
 };

            // Read key from configuration via DI
            var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var key = config?["Jwt:Key"];

            var secret = key ?? "ChangeThisDemoSecretToAStrongKey";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, expires = token.ValidTo });
        }

        private static UserDto WithoutPassword(UserDto u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            CreatedAt = u.CreatedAt
        };
    }
}
