using Piles.ViewModels;
using System;
using System.Windows.Input;

namespace Piles.Commands
{
    public interface IUndoableCommand : ICommand
    {
        event EventHandler Executed;
        OperationType OperationType { get; }
        TargetType TargetType { get; }
        object Target { get; }
        bool CanSave(object parameter);
        void Undo();
        void Redo();
        object Clone();
    }
}
