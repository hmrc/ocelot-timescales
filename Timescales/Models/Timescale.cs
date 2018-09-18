using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Timescales.Models
{
    public class Timescale
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50, ErrorMessage = "Name Too Long")]
        public string Name { get; set; }

        [DisplayName("Updated Date")]
        [Required(ErrorMessage = "Updated Date Required")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Description Required")]
        [MaxLength(256, ErrorMessage = "Description Too Long")]
        public string Description { get; set; }

        [DisplayName("Owners")]
        [Required(ErrorMessage = "Owners Required")]
        [MinLength(7, ErrorMessage = "Owners Too Short")]
        public String Owners { get; set; }

        [DisplayName("Oldest Work Date")]
        [Required(ErrorMessage = "Oldest Work Date Required")]
        [DataType(DataType.Date)]
        public DateTime OldestWorkDate { get; set; }

        [DisplayName("Days")]
        [Required(ErrorMessage = "Days Required")]
        public int Days { get; set; }

        [DisplayName("Basis")]
        [MaxLength(10, ErrorMessage = "Basis Too Long")]
        public string Basis { get; set; }
    }
}

