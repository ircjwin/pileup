using Piles.Commands;
using Piles.Core;
using Piles.DbContexts;
using Piles.Models;
using Piles.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Piles.ViewModels
{
    public enum OperationType
    {
        Add,
        Modify,
        Remove,
    }

    public enum TargetType
    {
        Pile,
        PileCollection,
        Rumination,
        RuminationCollection,
    }

    public class CommandStackViewModel : ICommandListener
    {
        private readonly IPilesDbContextFactory _pilesDbContextFactory;
        private ICollection<IUndoableCommand> _commandSubscriptions = new List<IUndoableCommand>();

        private ObservableStack<IUndoableCommand> _undoStack = new ObservableStack<IUndoableCommand>();
        private ObservableStack<IUndoableCommand> _redoStack = new ObservableStack<IUndoableCommand>();

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public CommandStackViewModel(IPilesDbContextFactory pilesDbContextFactory)
        {
            _pilesDbContextFactory = pilesDbContextFactory;

            UndoCommand = new UndoCommand(_undoStack, _redoStack);
            RedoCommand = new RedoCommand(_undoStack, _redoStack);
        }

        public void Save()
        {
            ICollection<(OperationType, Pile)> unsavedPiles = new List<(OperationType, Pile)>();
            ICollection<(OperationType, (Rumination, Pile))> unsavedRuminations = new List<(OperationType, (Rumination, Pile))>();

            foreach (IUndoableCommand undoableCommand in _undoStack.Reverse())
            {
                if (undoableCommand.CanSave(null) is false) continue;

                if (undoableCommand.TargetType == TargetType.Pile)
                {
                    unsavedPiles.Add((undoableCommand.OperationType, (Pile)undoableCommand.Target));
                }
                else if (undoableCommand.TargetType == TargetType.Rumination)
                {
                    Tuple<Rumination, Pile> ruminationPile = (Tuple<Rumination, Pile>)undoableCommand.Target;
                    unsavedRuminations.Add((undoableCommand.OperationType, (ruminationPile.Item1, ruminationPile.Item2)));
                }
                else if (undoableCommand.TargetType == TargetType.PileCollection)
                {
                    foreach (Pile pile in (ICollection<Pile>)undoableCommand.Target)
                    {
                        unsavedPiles.Add((undoableCommand.OperationType, pile));
                    }
                }
                else if (undoableCommand.TargetType == TargetType.RuminationCollection)
                {
                    foreach ((Rumination, Pile) ruminationPile in (ICollection<(Rumination, Pile)>)undoableCommand.Target)
                    {
                        unsavedRuminations.Add((undoableCommand.OperationType, ruminationPile));
                    }
                }
            }

            IPileService pileService = new PileService(_pilesDbContextFactory);
            IRuminationService ruminationService = new RuminationService(_pilesDbContextFactory);
            pileService.Save(unsavedPiles);
            ruminationService.Save(unsavedRuminations);

            _undoStack.Clear();
            _redoStack.Clear();
        }

        private void OnExecuted(object sender, EventArgs e)
        {
            IUndoableCommand undoableCommand = sender as IUndoableCommand;
            IUndoableCommand undoableCommandDeepCopy = undoableCommand.Clone() as IUndoableCommand;
            _undoStack.ObservablePush(undoableCommandDeepCopy);
        }

        public void Listen(IUndoableCommand undoableCommand)
        {
            undoableCommand.Executed += OnExecuted;
            _commandSubscriptions.Add(undoableCommand);
        }
    }
}
