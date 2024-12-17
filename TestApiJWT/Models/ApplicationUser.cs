﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TestApiJWT.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required,MaxLength(50)]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
