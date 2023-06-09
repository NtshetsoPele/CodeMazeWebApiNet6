﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Company
{
    // These data annotation attributes serve the purpose to validate our
    // model object while creating or updating resources in the database.
    // But we are not making use of them yet.
    
    // The [Column] attribute will specify that the Id property is going to be
    // mapped with a different name in the database.
    [Column("CompanyId")]
    public Guid Id { get; set; }
    
    // The [Required] and [MaxLength] properties are here for validation purposes.
    [Required(ErrorMessage = "Company name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; set; }
    
    // These attributes serve the purpose to validate our model object while 
    // creating or updating resources in the database.
    [Required(ErrorMessage = "Company address is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")]
    public string? Address { get; set; }
    
    public string? Country { get; set; }
    
    public ICollection<Employee>? Employees { get; set; }
}
