﻿using System.ComponentModel.DataAnnotations;

namespace EMS.Application.DTOs.Account
{
    public class UserRequestDto
    {
        string? Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        // Optional: Constructor for initialization
        public UserRequestDto() { }

        public UserRequestDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
