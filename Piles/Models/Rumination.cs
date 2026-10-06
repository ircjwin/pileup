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

        private bool _isSilenced;
        public bool IsSilenced
        {
            get { return _isSilenced; }
            set
            {
                _isSilenced = value;
                OnRuminationChanged();
            }
        }

        public int Layer { get; set; }

        public event Action<Rumination> RuminationChanged;

        public Rumination(int origin, DateTime createdOn, int layer, string description, bool isSilenced)
        {
            Origin = origin;
            CreatedOn = createdOn;
            Layer = layer;
            Description = description;
            IsSilenced = isSilenced;
        }

        private void OnRuminationChanged()
        {
            RuminationChanged?.Invoke(this);
        }
    }
}
