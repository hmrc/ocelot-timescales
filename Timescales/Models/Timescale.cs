using System;
using System.ComponentModel.DataAnnotations;

namespace Timescales.Models
{
    public class Timescale
    {
        [Key]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime UpdatedDate { get; set; }
        public String Description { get; set; }
        public String Owners { get; set; }
        public DateTime OldestWorkDate { get; set; }
        public String Days { get; set; }
        public String Basis { get; set; }
    }
}

