using Piles.ViewModels;
using System;
using System.Collections.Generic;

namespace Piles.Commands
{
    public class RummageCommand : CommandBase
    {
        private readonly PileupViewModel _pileupViewModel;

        public RummageCommand(PileupViewModel pileupViewModel)
        {
            _pileupViewModel = pileupViewModel;
        }

        public override void Execute(object parameter)
        {
            IList<(RuminationViewModel, PileViewModel)> rummage = new List<(RuminationViewModel, PileViewModel)>();

            foreach (PileViewModel pileViewModel in _pileupViewModel.Piles)
            {
                foreach (RuminationViewModel ruminationViewModel in pileViewModel.Ruminations)
                {
                    if (ruminationViewModel.IsRummagePick)
                    {
                        ruminationViewModel.IsRummagePick = false;
                        continue;
                    }

                    if (ruminationViewModel.IsRummage)
                    {
                        rummage.Add((ruminationViewModel, pileViewModel));
                    }
                }
            }

            if (rummage.Count == 0) return;

            Random random = new Random();
            int rummageIndex = random.Next(rummage.Count);
            rummage[rummageIndex].Item1.IsRummagePick = true;
            _pileupViewModel.TopPile = rummage[rummageIndex].Item2;
        }
    }
}
