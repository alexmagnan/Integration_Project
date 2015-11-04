using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCBikesWebpage.Models
{
    public class ContactUsModel

    {
         
        [Required(ErrorMessage="First Name is required!")]
        [DisplayName("First Name:")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required!")]
        [DisplayName("Last Name:")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DisplayName("Email:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Comments is required!")]
        [DisplayName("Comments:")]
        public string Comments { get; set; }
    }
}