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

        public event Action<Pile> PileChanged;

        public Pile(int origin, DateTime createdOn, string title, IList<Rumination> ruminations)
        {
            Origin = origin;
            CreatedOn = createdOn;
            Title = title;
            Ruminations = ruminations;
        }

        public void AddRumination(string description)
        {
            Rumination rumination = new Rumination(Ruminations.Count, DateTime.Now, description, false);
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

        private void OnPileChanged()
        {
            PileChanged?.Invoke(this);
        }
    }
}
