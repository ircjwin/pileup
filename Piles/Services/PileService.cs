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
    public class PileService : IPileService
    {
        private readonly IPilesDbContextFactory _pilesDbContextFactory;

        public PileService(IPilesDbContextFactory pilesDbContextFactory)
        {
            _pilesDbContextFactory = pilesDbContextFactory;
        }

        public async Task<IList<Pile>> GetAllPilesAsync()
        {
            ICollection<PileDb> pileDbs;
            IList<Pile> piles = new List<Pile>();

            using (PilesDbContext pilesDbContext = _pilesDbContextFactory.CreateDbContext())
            {
                pileDbs = await pilesDbContext.Piles
                    .OrderBy(p => p.SequenceNumber)
                    .Include(p => p.Ruminations.OrderBy(r => r.SequenceNumber))
                    .AsNoTracking()
                    .ToListAsync();
            }

            foreach (PileDb pileDb in pileDbs)
            {
                piles.Add(ToDomain(pileDb));
            }

            return piles;
        }

        public async Task<Pile> GetPileByKeyAsync(int origin, DateTime createdOn)
        {
            PileDb pileDb;

            using (PilesDbContext pilesDbContext = _pilesDbContextFactory.CreateDbContext())
            {
                pileDb = await pilesDbContext.Piles
                    .Include(p => p.Ruminations.OrderBy(r => r.SequenceNumber))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Origin == origin && p.CreatedOn == createdOn);
            }

            return ToDomain(pileDb);
        }

        public void CreatePile(Pile pile, PilesDbContext pilesDbContext)
        {
            pilesDbContext.Piles.Add(ToDb(pile));
        }

        public void UpdatePile(Pile pile, PilesDbContext pilesDbContext)
        {
            foreach (var entry in pilesDbContext.ChangeTracker.Entries<PileDb>())
            {
                if (entry.Entity.Origin == pile.Origin && entry.Entity.CreatedOn == pile.CreatedOn) return;
            }

            pilesDbContext.Piles.Entry(ToDb(pile)).State = EntityState.Modified;
        }

        public void DeletePile(Pile pile, PilesDbContext pilesDbContext)
        {
            pilesDbContext.Piles.Remove(ToDb(pile));
        }

        public void Save(ICollection<(OperationType, Pile)> unsavedPiles)
        {
            using (PilesDbContext pilesDbContext = _pilesDbContextFactory.CreateDbContext())
            {
                foreach ((OperationType operationType, Pile pile) in unsavedPiles)
                {
                    switch (operationType)
                    {
                        case (OperationType.Add):
                            CreatePile(pile, pilesDbContext);
                            break;
                        case (OperationType.Modify):
                            UpdatePile(pile, pilesDbContext);
                            break;
                        case (OperationType.Remove):
                            DeletePile(pile, pilesDbContext);
                            break;
                    }
                }
                pilesDbContext.SaveChangesAsync();
            }
        }

        private Pile ToDomain(PileDb pileDb)
        {
            IList<Rumination> ruminations = new List<Rumination>();

            foreach (RuminationDb ruminationDb in pileDb.Ruminations)
            {
                Rumination rumination = new Rumination(ruminationDb.Origin, ruminationDb.CreatedOn, ruminationDb.SequenceNumber, ruminationDb.Description, ruminationDb.IsSilenced);
                ruminations.Add(rumination);
            }

            return new Pile(pileDb.Origin, pileDb.CreatedOn, pileDb.SequenceNumber, pileDb.Title, ruminations);
        }

        private PileDb ToDb(Pile pile)
        {
            return new PileDb()
            {
                Origin = pile.Origin,
                CreatedOn = pile.CreatedOn,
                SequenceNumber = pile.Proximity,
                Title = pile.Title,
            };
        }
    }
}
