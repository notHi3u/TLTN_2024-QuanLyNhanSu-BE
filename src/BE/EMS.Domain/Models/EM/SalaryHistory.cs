﻿using System.ComponentModel.DataAnnotations;

namespace EMS.Domain.Models.EM
{
    public class SalaryHistory
    {
        [Key] // Marks Id as the primary key
        public long Id { get; set; }

        [Required] // Ensures EmployeeId is always provided
        public required string EmployeeId { get; set; }

        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; } // Month should be between 1 and 12

        [Range(1900, int.MaxValue, ErrorMessage = "Year must be a valid year.")]
        public int Year { get; set; } // Year should be a reasonable value, starting from 1900

        [Range(0, double.MaxValue, ErrorMessage = "Base salary must be a positive value.")]
        public decimal BaseSalary { get; set; } // Base salary must be a positive value

        [Range(0, double.MaxValue, ErrorMessage = "Bonus must be a positive value.")]
        public decimal Bonus { get; set; } // Bonus must be a positive value

        [Range(0, double.MaxValue, ErrorMessage = "Deductions must be a positive value.")]
        public decimal Deductions { get; set; } // Deductions must be a positive value

        [Range(0, double.MaxValue, ErrorMessage = "Net salary must be a positive value.")]
        public decimal NetSalary { get; set; } // Net salary must be a positive value

        // Navigation property
        public virtual Employee Employee { get; set; }
    }
}
