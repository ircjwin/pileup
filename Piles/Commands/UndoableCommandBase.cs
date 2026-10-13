using Piles.ViewModels;
using System;

namespace Piles.Commands
{
    public abstract class UndoableCommandBase : CommandBase, IUndoableCommand
    {
        public event EventHandler Executed;

        public abstract OperationType OperationType { get; }

        public abstract object Target { get; }

        public abstract TargetType TargetType { get; }

        public virtual bool CanSave(object parameter)
        {
            return true;
        }

        public abstract void Redo();

        public abstract void Undo();

        public abstract object Clone();

        protected void OnExecuted()
        {
            Executed?.Invoke(this, new EventArgs());
        }
    }
}
