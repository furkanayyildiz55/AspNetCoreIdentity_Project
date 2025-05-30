﻿using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Role isim alanı boş bırakılamaz.")]
        [Display(Name = "Role ismi :")]
        public string Name { get; set; } = null!;
    }
}
