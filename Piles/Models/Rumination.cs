using System;

namespace Piles.Models
{
    public class Rumination
    {
        public int Origin { get; init; }

        public DateTime CreatedOn { get; init; }

        private string _description;
        public string Description
        { 
            get { return _description; }
            set
            {
                _description = value;
                OnRuminationChanged();
            }
        }

        public int Layer { get; set; }

        public event Action<Rumination> RuminationChanged;

        public Rumination(int origin, DateTime createdOn, int layer, string description)
        {
            Origin = origin;
            CreatedOn = createdOn;
            Layer = layer;
            Description = description;
        }

        private void OnRuminationChanged()
        {
            RuminationChanged?.Invoke(this);
        }
    }
}
