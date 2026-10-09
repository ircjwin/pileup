using Piles.Models;
using Piles.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Piles.Commands
{
    public class CheckAllRuminationsCommand : UndoableCommandBase
    {
        private readonly Pile _pile;

        private OperationType _operationType = OperationType.Modify;
        public override OperationType OperationType
        {
            get { return _operationType; }
        }

        private ICollection<(Rumination, Pile)> _target;
        public override object Target
        {
            get { return _target; }
        }

        private TargetType _targetType = TargetType.RuminationCollection;
        public override TargetType TargetType
        {
            get { return _targetType; }
        }

        public CheckAllRuminationsCommand(ICollection<(Rumination, Pile)> target)
        {
            _target = target;
        }

        public CheckAllRuminationsCommand(Pile pile, ICommandListener commandListener)
        {
            _pile = pile;

            commandListener.Listen(this);
        }

        public override void Execute(object parameter)
        {
            _target = new List<(Rumination, Pile)>();

            foreach (Rumination rumination in _pile.Ruminations)
            {
                if (rumination.IsSilenced) continue;

                rumination.IsSilenced = true;
                _target.Add((rumination, _pile));
            }

            OnExecuted();
        }

        public override void Redo()
        {
            foreach ((Rumination, Pile) ruminationPile in _target)
            {
                ruminationPile.Item1.IsSilenced = true;
            }
        }

        public override void Undo()
        {
            foreach ((Rumination, Pile) ruminationPile in _target)
            {
                ruminationPile.Item1.IsSilenced = false;
            }
        }

        public override object Clone()
        {
            return new CheckAllRuminationsCommand(_target);
        }
    }
}
