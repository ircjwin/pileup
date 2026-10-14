using Piles.Models;
using Piles.ViewModels;
using System;

namespace Piles.Commands
{
    public class UpdateRuminationIsCheckedCommand : UndoableCommandBase
    {
        private OperationType _operationType = OperationType.Modify;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private RuminationViewModel _target;
        public override object Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.Rumination;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        private bool _oldIsChecked;
        private bool _newIsChecked;

        public override bool CanSave(object parameter)
        {
            return false;
        }

        public UpdateRuminationIsCheckedCommand(RuminationViewModel ruminationViewModel, ICommandListener commandListener)
        {
            _target = ruminationViewModel;

            commandListener.Listen(this);
        }

        public UpdateRuminationIsCheckedCommand(RuminationViewModel ruminationViewModel, bool oldIsChecked, bool newIsChecked)
        {
            _target = ruminationViewModel;
            _oldIsChecked = oldIsChecked;
            _newIsChecked = newIsChecked;
        }

        public override void Execute(object parameter)
        {
            _oldIsChecked = !_target.IsChecked;
            _newIsChecked = _target.IsChecked;

            OnExecuted();
        }

        public override void Redo()
        {
            _target.IsChecked = _newIsChecked;
        }

        public override void Undo()
        {
            _target.IsChecked = _oldIsChecked;
        }

        public override UpdateRuminationIsCheckedCommand Clone()
        {
            return new UpdateRuminationIsCheckedCommand(_target, _oldIsChecked, _newIsChecked);
        }
    }
}
