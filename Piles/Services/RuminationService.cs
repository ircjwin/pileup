using Microsoft.EntityFrameworkCore;
using Piles.DbContexts;
using Piles.Models;
using Piles.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piles.Services
{
    public class RuminationService : IRuminationService
    {
        private readonly IPilesDbContextFactory _pilesDbContextFactory;

        public RuminationService(IPilesDbContextFactory pilesDbContextFactory)
        {
            _pilesDbContextFactory = pilesDbContextFactory;
        }

        public async Task<ICollection<Rumination>> GetRuminationsByPileKeyAsync(int pileOrigin, DateTime pileCreatedOn)
        {
            ICollection<RuminationDb> ruminationDbs;
            ICollection<Rumination> ruminations = new List<Rumination>();

            using (PilesDbContext pilesDbContext = _pilesDbContextFactory.CreateDbContext())
            {
                ruminationDbs = await pilesDbContext.Ruminations
                    .Where(r => r.PileOrigin == pileOrigin && r.PileCreatedOn == pileCreatedOn)
                    .OrderBy(r => r.SequenceNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }

            foreach (RuminationDb ruminationDb in ruminationDbs)
            {
                ruminations.Add(ToDomain(ruminationDb));
            }

            return ruminations;
        }

        public async Task<Rumination> GetRuminationByKeyAsync(int origin, DateTime createdOn)
        {
            RuminationDb ruminationDb;

            using (PilesDbContext pilesDbContext = _pilesDbContextFactory.CreateDbContext())
            {
                ruminationDb = await pilesDbContext.Ruminations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Origin == origin && r.CreatedOn == createdOn);
            }

            return ToDomain(ruminationDb);
        }

        public void CreateRumination(Rumination rumination, Pile pile, PilesDbContext pilesDbContext)
        {
            pilesDbContext.Ruminations.Add(ToDb(rumination, pile));
        }

        public void UpdateRumination(Rumination rumination, Pile pile, PilesDbContext pilesDbContext)
        {
            foreach (var entry in pilesDbContext.ChangeTracker.Entries<RuminationDb>())
            {
                if (entry.Entity.Origin == rumination.Origin && entry.Entity.CreatedOn == rumination.CreatedOn) return;
            }

            pilesDbContext.Ruminations.Entry(ToDb(rumination, pile)).State = EntityState.Modified;
        }

        public void DeleteRumination(Rumination rumination, Pile pile, PilesDbContext pilesDbContext)
        {
            pilesDbContext.Ruminations.Remove(ToDb(rumination, pile));
        }

        public void Save(ICollection<(OperationType, (Rumination, Pile))> unsavedRuminations)
        {
            using (PilesDbContext pilesDbContext = _pilesDbContextFactory.CreateDbContext())
            {
                foreach ((OperationType operationType, (Rumination rumination, Pile pile)) in unsavedRuminations)
                {
                    switch (operationType)
                    {
                        case (OperationType.Add):
                            CreateRumination(rumination, pile, pilesDbContext);
                            break;
                        case (OperationType.Modify):
                            UpdateRumination(rumination, pile, pilesDbContext);
                            break;
                        case (OperationType.Remove):
                            DeleteRumination(rumination, pile, pilesDbContext);
                            break;
                    }
                }
                pilesDbContext.SaveChangesAsync();
            }
        }

        private Rumination ToDomain(RuminationDb ruminationDb)
        {
            return new Rumination(ruminationDb.Origin, ruminationDb.CreatedOn, ruminationDb.SequenceNumber, ruminationDb.Description, ruminationDb.IsSilenced);
        }

        private RuminationDb ToDb(Rumination rumination, Pile pile)
        {
            return new RuminationDb()
            {
                Origin = rumination.Origin,
                CreatedOn = rumination.CreatedOn,
                SequenceNumber = rumination.Layer,
                Description = rumination.Description,
                IsSilenced = rumination.IsSilenced,

                PileOrigin = pile.Origin,
                PileCreatedOn = pile.CreatedOn,
            };
        }
    }
}
