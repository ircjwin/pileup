using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Piles.Models
{
    [PrimaryKey(nameof(Origin), nameof(CreatedOn))]
    public class PileDb
    {
        public int Origin { get; set; }
        public DateTime CreatedOn { get; set; }
        public int SequenceNumber { get; set; }
        public string Title { get; set; }
        
        public virtual ICollection<RuminationDb> Ruminations { get; } = new List<RuminationDb>();
    }
}
