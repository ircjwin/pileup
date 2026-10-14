using Piles.Commands;
using Piles.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Piles.ViewModels
{
    public class PileViewModel : ViewModelBase
    {
        private bool _isEditing = false;

        [XmlIgnore]
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

        [XmlIgnore]
        public bool IsNotEditing
        {
            get { return !_isEditing; }
        }

        private readonly Pile _pile;

        private string _title;

        [XmlIgnore]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private readonly ObservableCollection<RuminationViewModel> _ruminations;
        public IEnumerable<RuminationViewModel> Ruminations => _ruminations;

        private string _newRuminationDescription;

        [XmlIgnore]
        public string NewRuminationDescription
        {
            get
            {
                return _newRuminationDescription;
            }
            set
            {
                _newRuminationDescription = value;
                OnPropertyChanged();
            }
        }

        private int _tabControlIndex;

        [XmlIgnore]
        public int TabControlIndex
        {
            get
            {
                return _tabControlIndex;
            }
            set
            {
                _tabControlIndex = value;
            }
        }

        private bool _isRummagePile;
        public bool IsRummagePile
        {
            get { return _isRummagePile; }
            set
            {
                _isRummagePile = value;
                OnPropertyChanged();
            }
        }

        private int _focusedRumination;

        [XmlIgnore]
        public int FocusedRumination
        {
            get { return _focusedRumination; }
            set
            {
                _focusedRumination = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddRuminationCommand { get; }
        public ICommand RemoveCheckedRuminationsCommand { get; }
        public ICommand CheckAllRuminationsCommand { get; }
        public ICommand UncheckAllRuminationsCommand { get; }
        public ICommand ReorderRuminationCommand { get; }
        public ICommand UpdatePileCommand { get; }
        public ICommand UpdatePileTitleCommand { get; }

        private Func<Rumination, Pile, RuminationViewModel> _createRuminationViewModel;

        private PileViewModel() { }

        public PileViewModel(Pile pile, Func<Rumination, Pile, RuminationViewModel> createRuminationViewModel, ICommandListener commandListener)
        {
            _pile = pile;
            _title = pile.Title;
            _tabControlIndex = pile.Proximity;
            _ruminations = new ObservableCollection<RuminationViewModel>();
            _createRuminationViewModel = createRuminationViewModel;
            _pile.PileChanged += OnPileChanged;

            AddRuminationCommand = new AddRuminationCommand(_pile, commandListener);
            RemoveCheckedRuminationsCommand = new RemoveCheckedRuminationsCommand(_pile, _ruminations, commandListener);
            UpdatePileTitleCommand = new UpdatePileTitleCommand(_pile, commandListener);
            ReorderRuminationCommand = new ReorderRuminationCommand(_pile, commandListener);
            CheckAllRuminationsCommand = new CheckAllRuminationsCommand(_pile, commandListener);
            UncheckAllRuminationsCommand = new UncheckAllRuminationsCommand(_pile, commandListener);
            UpdatePileCommand = new UpdatePileCommand(this);

            UpdateRuminations(_pile.Ruminations);
        }

        public void UpdateRuminations(IEnumerable<Rumination> ruminations)
        {
            _ruminations.Clear();

            foreach (Rumination rumination in ruminations)
            {
                RuminationViewModel ruminationViewModel = _createRuminationViewModel(rumination, _pile);
                ruminationViewModel.PropertyChanged += OnRuminationViewModelPropertyChanged;
                _ruminations.Add(ruminationViewModel);
            }
        }

        public void UpdateRuminations(IList<RuminationViewModel> ruminationViewModels)
        {
            int i = 0;
            
            foreach (RuminationViewModel ruminationViewModel in _ruminations)
            {
                ruminationViewModel.IsRummage = ruminationViewModels[i].IsRummage;
                i++;
            }
        }

        private void OnPileChanged(Pile pile)
        {
            Title = pile.Title;
            TabControlIndex = pile.Proximity;

            UpdateRuminations(pile.Ruminations);
        }

        private void OnRuminationViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RuminationViewModel.IsRummagePick))
            {
                RuminationViewModel ruminationViewModel = sender as RuminationViewModel;

                if (ruminationViewModel.IsRummagePick)
                {
                    FocusedRumination = _ruminations.IndexOf(ruminationViewModel);
                }
            }
        }
    }
}
