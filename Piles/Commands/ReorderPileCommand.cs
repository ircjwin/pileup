using Piles.Models;
using Piles.ViewModels;
using System;
using System.Linq;

namespace Piles.Commands
{
    public class ReorderPileCommand : UndoableCommandBase
    {
        private readonly Pileup _pileup;

        private OperationType _operationType = OperationType.Add;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private Pile _target;
        public override Pile Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.Pile;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        private int _oldIndex;
        private int _newIndex;

        public ReorderPileCommand(Pileup pileup, Pile pile, int oldIndex, int newIndex)
        {
            _pileup = pileup;
            _target = pile;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        public ReorderPileCommand(Pileup pileup, ICommandListener commandListener)
        {
            _pileup = pileup;

            commandListener.Listen(this);
        }

        public override void Execute(object parameter)
        {
            (_oldIndex, _newIndex) = parameter as Tuple<int, int>;
            _target = _pileup.Piles[_oldIndex];
            _pileup.RemovePileAt(_oldIndex);
            _pileup.InsertPile(_newIndex, _target);

            OnExecuted();
        }

        public override void Redo()
        {
            _pileup.RemovePileAt(_oldIndex);
            _pileup.InsertPile(_newIndex, _target);
        }

        public override void Undo()
        {
            _pileup.RemovePileAt(_newIndex);
            _pileup.InsertPile(_oldIndex, _target);
        }

        public override ReorderPileCommand Clone()
        {
            return new ReorderPileCommand(_pileup, _target, _oldIndex, _newIndex);
        }
    }
}

