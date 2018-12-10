using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Timescales.Models
{
    public class Timescale
    {
        [Key]      
        public Guid Id { get; set; }

        [DisplayName("Placeholder")]
        [Required(ErrorMessage = "Placeholder Required")]
        [MaxLength(60, ErrorMessage = "Placeholder Too Long")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Placeholder may not contain spaces")]
        [Remote("CheckPlaceholderExist", "Validation", ErrorMessage = "Placeholder already taken", HttpMethod = "POST")]
        public string Placeholder { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(256, ErrorMessage = "Name Too Long")]
        public string Name { get; set; }

        [DisplayName("Updated Date")]
        [Required(ErrorMessage = "Updated Date Required")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Owners")]
        [Required(ErrorMessage = "Owners Required")]
        [MinLength(7, ErrorMessage = "Owners Too Short")]
        [RegularExpression("^\\d{7}(,\\d{7})*$", ErrorMessage = "Owners must be single PID or comma separated PIDs eg. 1111111,2222222")]
        public string Owners { get; set; }

        [DisplayName("Oldest Work Date")]
        [Required(ErrorMessage = "Oldest Work Date Required")]
        [DataType(DataType.Date)]
        public DateTime OldestWorkDate { get; set; }

        [DisplayName("Days")]
        [Required(ErrorMessage = "Days Required")]
        public int Days { get; set; }

        [DisplayName("Basis")]
        [Required(ErrorMessage = "Basis Required")]
        [MaxLength(10, ErrorMessage = "Basis Too Long")]
        public string Basis { get; set; }

        [DisplayName("Site")]
        [Required(ErrorMessage = "Site Required")]
        [MaxLength(10, ErrorMessage = "Site Too Long")]
        public string Site { get; set; }

        private string _lineOfBusiness;

        [DisplayName("Line Of Business")]
        [Required(ErrorMessage = "Line Of Business Required")]
        [MinLength(3, ErrorMessage = "Line Of Business Too Short")]
        [MaxLength(3, ErrorMessage = "Line Of Business Too Long")] 
        public string LineOfBusiness
        {
            get
            {
                return _lineOfBusiness;
            }
            set
            {
                if (_lineOfBusiness != value.ToUpper())
                {
                    _lineOfBusiness = value.ToUpper();
                }
            }
        }

        [JsonIgnore]
        [XmlIgnore]
        public virtual List<Audit> Audit { get; set; }
    }
}

