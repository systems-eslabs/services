using System;
using System.Collections.Generic;

namespace EmailService.DbEntities
{
    public partial class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Template1 { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
