using System;
using System.Collections.Generic;

namespace Piles.Models
{
    public class Pile
    {
        public int Origin { get; init; }

        public DateTime CreatedOn { get; init; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set 
            {
                _title = value;
                OnPileChanged();
            } 
        }

        public IList<Rumination> Ruminations { get; set; }

        public int Proximity { get; set; }

        public event Action<Pile> PileChanged;

        public Pile(int origin, DateTime createdOn, int proximity, string title, IList<Rumination> ruminations)
        {
            Origin = origin;
            CreatedOn = createdOn;
            Proximity = proximity;
            Title = title;
            Ruminations = ruminations;
        }

        public void AddRumination(string description)
        {
            Rumination rumination = new Rumination(Ruminations.Count, DateTime.Now, Ruminations.Count, description, false);
            Ruminations.Add(rumination);
            OnPileChanged();
        }

        public void AddRumination(Rumination rumination)
        {
            Ruminations.Add(rumination);
            OnPileChanged();
        }

        public void InsertRumination(int ruminationIndex, Rumination rumination)
        {
            Ruminations.Insert(ruminationIndex, rumination);
            OnPileChanged();
        }

        public void RemoveRumination(Rumination rumination)
        {
            Ruminations.Remove(rumination);
            OnPileChanged();
        }

        public void RemoveRuminationAt(int ruminationIndex)
        {
            Ruminations.RemoveAt(ruminationIndex);
            OnPileChanged();
        }

        private void UpdateRuminationLayers()
        {
            if (Ruminations == null) return;

            foreach (Rumination rumination in Ruminations)
            {
                rumination.Layer = Ruminations.IndexOf(rumination);
            }
        }

        private void OnPileChanged()
        {
            UpdateRuminationLayers();

            PileChanged?.Invoke(this);
        }
    }
}
