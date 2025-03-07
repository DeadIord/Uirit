﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSystemTwo.Models
{
    public class ApplicationModel
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string ServiceNumber { get; set; }

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Required]
        public string Body { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        public long StatusId { get; set; }

        [ForeignKey("StatusId")]
        public StatusModel Status { get; set; }
    }

}