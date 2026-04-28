using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiiScanner.Domain.Entity
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        // Demo only: store password in-memory. Use hashed passwords in production.
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
