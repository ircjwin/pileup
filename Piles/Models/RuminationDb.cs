using Microsoft.EntityFrameworkCore;
using System;

namespace Piles.Models
{
    [PrimaryKey(nameof(Origin), nameof(CreatedOn))]
    public class RuminationDb
    {
        public int Origin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public bool IsSilenced { get; set; }

        public int PileOrigin { get; set; }
        public DateTime PileCreatedOn { get; set; }

        public virtual PileDb Pile { get; set; } = null!;
    }
}
