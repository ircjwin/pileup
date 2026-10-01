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

        private Tuple<Rumination, Pile> _target;
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

        public UpdateRuminationIsCheckedCommand(Rumination rumination, Pile pile, ICommandListener commandListener)
        {
            _target = Tuple.Create(rumination, pile);

            commandListener.Listen(this);
        }

        public UpdateRuminationIsCheckedCommand(Tuple<Rumination, Pile> ruminationPile, bool oldIsChecked, bool newIsChecked)
        {
            _target = ruminationPile;
            _oldIsChecked = oldIsChecked;
            _newIsChecked = newIsChecked;
        }

        public override void Execute(object parameter)
        {
            RuminationViewModel ruminationViewModel = parameter as RuminationViewModel;

            _oldIsChecked = _target.Item1.IsSilenced;
            _newIsChecked = ruminationViewModel.IsChecked;
            _target.Item1.IsSilenced = _newIsChecked;

            OnExecuted();
        }

        public override void Redo()
        {
            _target.Item1.IsSilenced = _newIsChecked;
        }

        public override void Undo()
        {
            _target.Item1.IsSilenced = _oldIsChecked;
        }

        public override UpdateRuminationIsCheckedCommand Clone()
        {
            return new UpdateRuminationIsCheckedCommand(_target, _oldIsChecked, _newIsChecked);
        }
    }
}
