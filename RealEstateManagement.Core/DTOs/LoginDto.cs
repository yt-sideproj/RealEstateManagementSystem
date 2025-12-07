using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.DTOs
{
    public class CustomerLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AgentLoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
