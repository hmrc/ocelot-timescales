using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Timescales.Models
{
    public class Audit
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Timescale Id Required")]
        public Guid TimescaleId { get; set; }

        [DisplayName("User")]
        [Required(ErrorMessage = "User Required")]
        [MinLength(7, ErrorMessage = "User Too Short")]
        [MaxLength(7, ErrorMessage = "User Too Long")]       
        public string User { get; set; }

        [DisplayName("Action")]
        [Required(ErrorMessage = "Action Required")]      
        [MaxLength(7, ErrorMessage = "Action Too Long")]
        public string Action { get; set; }

        [DisplayName("Date/Time")]
        [Required(ErrorMessage = "Date Required")]
        public DateTime DateTime { get; set; }

        [DisplayName("Oldest Work Date")]
        [Required(ErrorMessage = "Oldest Work Date Required")]
        [DataType(DataType.Date)]
        public DateTime OldestWorkDate { get; set; }

        [DisplayName("Days")]
        [Required(ErrorMessage = "Days Required")]
        public int Days { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public virtual Timescale Timescale { get; set; }
    }
}
