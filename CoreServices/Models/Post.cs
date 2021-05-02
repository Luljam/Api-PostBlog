using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreServices.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        [Column(TypeName = "varchar(20)")]
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public Category Category { get; set; }
    }
}
