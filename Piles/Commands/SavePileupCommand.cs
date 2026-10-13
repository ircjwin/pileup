using Piles.ViewModels;

namespace Piles.Commands
{
    public class SavePileupCommand : CommandBase
    {
        private readonly ICommandListener _commandListener;
        private readonly PileupViewModel _pileupViewModel;

        public SavePileupCommand(ICommandListener commandListener, PileupViewModel pileupViewModel)
        {
            _commandListener = commandListener;
            _pileupViewModel = pileupViewModel;
        }

        public override void Execute(object parameter)
        {
            _commandListener.Save();
            _pileupViewModel.Save();
        }
    }
}
