using Piles.Models;
using Piles.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Piles.Commands
{
    public class ReorderPileCommand : UndoableCommandBase
    {
        private readonly Pileup _pileup;

        private OperationType _operationType = OperationType.Modify;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private ICollection<Pile> _target;
        public override ICollection<Pile> Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.PileCollection;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        private int _oldIndex;
        private int _newIndex;

        public ReorderPileCommand(Pileup pileup, ICollection<Pile> piles, int oldIndex, int newIndex)
        {
            _pileup = pileup;
            _target = piles;
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
            _target = _pileup.Piles;
            (_oldIndex, _newIndex) = parameter as Tuple<int, int>;

            Pile reorderedPile = _pileup.Piles[_oldIndex];

            _pileup.RemovePileAt(_oldIndex);
            _pileup.InsertPile(_newIndex, reorderedPile);
            
            OnExecuted();
        }

        public override void Redo()
        {
            Pile reorderedPile = _pileup.Piles[_oldIndex];

            _pileup.RemovePileAt(_oldIndex);
            _pileup.InsertPile(_newIndex, reorderedPile);
        }

        public override void Undo()
        {
            Pile reorderedPile = _pileup.Piles[_newIndex];

            _pileup.RemovePileAt(_newIndex);
            _pileup.InsertPile(_oldIndex, reorderedPile);
        }

        public override ReorderPileCommand Clone()
        {
            return new ReorderPileCommand(_pileup, _target, _oldIndex, _newIndex);
        }
    }
}

