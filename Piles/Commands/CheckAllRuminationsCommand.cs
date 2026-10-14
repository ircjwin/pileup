using Piles.Models;
using Piles.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Piles.Commands
{
    public class CheckAllRuminationsCommand : UndoableCommandBase
    {
        private readonly PileViewModel _pileViewModel;

        private OperationType _operationType = OperationType.Modify;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private ICollection<RuminationViewModel> _target;
        public override object Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.RuminationCollection;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        public override bool CanSave(object parameter)
        {
            return false;
        }

        public CheckAllRuminationsCommand(ICollection<RuminationViewModel> target)
        {
            _target = target;
        }

        public CheckAllRuminationsCommand(PileViewModel pileViewModel, ICommandListener commandListener)
        {
            _pileViewModel = pileViewModel;

            commandListener.Listen(this);
        }

        public override void Execute(object parameter)
        {
            _target = new List<RuminationViewModel>();

            foreach (RuminationViewModel ruminationViewModel in _pileViewModel.Ruminations)
            {
                if (ruminationViewModel.IsChecked) continue;

                ruminationViewModel.IsChecked = true;
                _target.Add(ruminationViewModel);
            }

            OnExecuted();
        }

        public override void Redo()
        {
            foreach (RuminationViewModel ruminationViewModel in _target)
            {
                ruminationViewModel.IsChecked = true;
            }
        }

        public override void Undo()
        {
            foreach (RuminationViewModel ruminationViewModel in _target)
            {
                ruminationViewModel.IsChecked = false;
            }
        }

        public override object Clone()
        {
            return new CheckAllRuminationsCommand(_target);
        }
    }
}
