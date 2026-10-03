using Piles.Models;
using Piles.ViewModels;
using System;
using System.Linq;

namespace Piles.Commands
{
    public class ReorderRuminationCommand : UndoableCommandBase
    {
        private readonly Pile _pile;

        private OperationType _operationType = OperationType.Add;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private Tuple<Rumination, Pile> _target;
        public override Tuple<Rumination, Pile> Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.Rumination;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        private int _oldIndex;
        private int _newIndex;

        public ReorderRuminationCommand(Pile pile, Tuple<Rumination, Pile> ruminationPile, int oldIndex, int newIndex)
        {
            _pile = pile;
            _target = ruminationPile;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        public ReorderRuminationCommand(Pile pile, ICommandListener commandListener)
        {
            _pile = pile;

            commandListener.Listen(this);
        }

        public override void Execute(object parameter)
        {
            (_oldIndex, _newIndex) = parameter as Tuple<int, int>;
            _target = new Tuple<Rumination, Pile>(_pile.Ruminations[_oldIndex], _pile);
            _pile.RemoveRuminationAt(_oldIndex);
            _pile.InsertRumination(_newIndex, _target.Item1);

            OnExecuted();
        }

        public override void Redo()
        {
            _pile.RemoveRuminationAt(_oldIndex);
            _pile.InsertRumination(_newIndex, _target.Item1);
        }

        public override void Undo()
        {
            _pile.RemoveRuminationAt(_newIndex);
            _pile.InsertRumination(_oldIndex, _target.Item1);
        }

        public override ReorderRuminationCommand Clone()
        {
            return new ReorderRuminationCommand(_pile, _target, _oldIndex, _newIndex);
        }
    }
}
