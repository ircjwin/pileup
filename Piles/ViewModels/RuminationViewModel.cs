using Piles.Commands;
using Piles.Models;
using System.Windows.Input;

namespace Piles.ViewModels
{
    public class RuminationViewModel : ViewModelBase
    {
        private readonly Rumination _rumination;

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }

        public bool IsNotEditing
        {
            get { return !_isEditing; }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isRummage;
        public bool IsRummage
        {
            get { return _isRummage; }
            set
            {
                _isRummage = value;
                OnPropertyChanged();
            }
        }

        private bool _isRummagePick;
        public bool IsRummagePick
        {
            get { return _isRummagePick; }
            set
            {
                _isRummagePick = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateRuminationCommand { get; }
        public ICommand UpdateRuminationDescriptionCommand { get; }
        public ICommand UpdateRuminationIsCheckedCommand { get; }

        public RuminationViewModel(Rumination rumination, Pile pile, ICommandListener commandListener)
        {
            _rumination = rumination;
            _description = rumination.Description;
            _isChecked = rumination.IsSilenced;
            _rumination.RuminationChanged += OnRuminationChanged;

            UpdateRuminationCommand = new UpdateRuminationCommand(this);
            UpdateRuminationDescriptionCommand = new UpdateRuminationDescriptionCommand(_rumination, pile, commandListener);
            UpdateRuminationIsCheckedCommand = new UpdateRuminationIsCheckedCommand(_rumination, pile, commandListener);
        }

        public void CheckRumination()
        {
            IsChecked = true;
        }

        public void UncheckRumination()
        {
            IsChecked = false;
        }

        private void OnRuminationChanged(Rumination rumination)
        {
            Description = rumination.Description;
            IsChecked = rumination.IsSilenced;
        }
    }
}
