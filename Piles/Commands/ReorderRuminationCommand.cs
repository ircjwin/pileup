using Piles.Models;
using Piles.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piles.Commands
{
    public class ReorderRuminationCommand : UndoableCommandBase
    {
        private readonly Pile _pile;

        private OperationType _operationType = OperationType.Modify;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private ICollection<(Rumination, Pile)> _target;
        public override ICollection<(Rumination, Pile)> Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.RuminationCollection;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        private int _oldIndex;
        private int _newIndex;

        public ReorderRuminationCommand(Pile pile, ICollection<(Rumination, Pile)> ruminationPile, int oldIndex, int newIndex)
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
            _target = new List<(Rumination, Pile)>();
            (_oldIndex, _newIndex) = parameter as Tuple<int, int>;

            Rumination reorderedRumination = _pile.Ruminations[_oldIndex];

            _pile.RemoveRuminationAt(_oldIndex);
            _pile.InsertRumination(_newIndex, reorderedRumination);

            foreach (Rumination rumination in _pile.Ruminations)
            {
                _target.Add((rumination, _pile));
            }

            OnExecuted();
        }

        public override void Redo()
        {
            Rumination reorderedRumination = _pile.Ruminations[_oldIndex];

            _pile.RemoveRuminationAt(_oldIndex);
            _pile.InsertRumination(_newIndex, reorderedRumination);
        }

        public override void Undo()
        {
            Rumination reorderedRumination = _pile.Ruminations[_newIndex];

            _pile.RemoveRuminationAt(_newIndex);
            _pile.InsertRumination(_oldIndex, reorderedRumination);
        }

        public override ReorderRuminationCommand Clone()
        {
            return new ReorderRuminationCommand(_pile, _target, _oldIndex, _newIndex);
        }
    }
}
