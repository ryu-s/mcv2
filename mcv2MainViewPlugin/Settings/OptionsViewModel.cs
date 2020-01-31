using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpenrecSitePlugin;

namespace mcv2.MainViewPlugin
{
    enum OptionsViewResult
    {
        Ok,
        Cancel,
    }
    class OptionsViewModel : ViewModelBase
    {
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public MainPanelViewModel MainPanelViewModel { get; }
        public YouTubeLiveOptionsViewModel YouTubeViewModel { get; }
        public TwitchSiteOptionsViewModel TwitchViewModel { get; }
        public WhowatchSiteOptionsViewModel WhowatchViewModel { get; }
        public TwicasSiteOptionsViewModel TwicasViewModel { get; }
        public LineLiveSiteOptionsViewModel LineLiveViewModel { get; }
        public BigoOptionsViewModel BigoViewModel { get; }
        public MildomSiteOptionsViewModel MildomViewModel { get; }
        public MirrativSiteOptionsViewModel MirrativViewModel { get; }
        public NicoSiteOptionsViewModel NicoViewModel { get; }
        public OpenrecOptionsViewModel OpenrecViewModel { get; }
        public ShowRoomSiteOptionsViewModel ShowroomViewModel { get; }
        public PeriscopeSiteOptionsViewModel PeriscopeViewModel { get; }
        public OptionsViewModel(IOptions mainOptions, IYouTubeLiveSiteOptions youTubeOptions, ITwitchSiteOptions twitchOptions, IWhowatchSiteOptions whowatchOptions, ITwicasSiteOptions twicasSiteOptions, ILineLiveSiteOptions lineLiveSiteOptions, IBigoSiteOptions bigoSiteOptions, IMildomSiteOptions mildomSiteOptions, IMirrativSiteOptions mirrativSiteOptions, INicoSiteOptions nicoSiteOptions, IOpenrecSiteOptions openrecSiteOptions, IShowRoomSiteOptions showroomSiteOptions, IPeriscopeSiteOptions periscopeSiteOptions)
        {
            MainPanelViewModel = new MainPanelViewModel(mainOptions);
            YouTubeViewModel = new YouTubeLiveOptionsViewModel(youTubeOptions);
            TwitchViewModel = new TwitchSiteOptionsViewModel(twitchOptions);
            WhowatchViewModel = new WhowatchSiteOptionsViewModel(whowatchOptions);
            TwicasViewModel = new TwicasSiteOptionsViewModel(twicasSiteOptions);
            LineLiveViewModel = new LineLiveSiteOptionsViewModel(lineLiveSiteOptions);
            BigoViewModel = new BigoOptionsViewModel(bigoSiteOptions);
            MildomViewModel = new MildomSiteOptionsViewModel(mildomSiteOptions);
            MirrativViewModel = new MirrativSiteOptionsViewModel(mirrativSiteOptions);
            NicoViewModel = new NicoSiteOptionsViewModel(nicoSiteOptions);
            OpenrecViewModel = new OpenrecOptionsViewModel(openrecSiteOptions);
            ShowroomViewModel = new ShowRoomSiteOptionsViewModel(showroomSiteOptions);
            PeriscopeViewModel = new PeriscopeSiteOptionsViewModel(periscopeSiteOptions);

            OkCommand = new RelayCommand(Ok);
            CancelCommand = new RelayCommand(Cancel);
        }
        public OptionsViewModel()
        {
            if (Tools.IsInDesignMode())
            {
                MainPanelViewModel = new MainPanelViewModel();
                YouTubeViewModel = new YouTubeLiveOptionsViewModel();
                TwitchViewModel = new TwitchSiteOptionsViewModel();
                WhowatchViewModel = new WhowatchSiteOptionsViewModel();

            }
            else
            {
                throw new NotSupportedException();
            }
        }
        /// <summary>
        /// OKボタンとCancelボタンのどちらを押したか
        /// </summary>
        public OptionsViewResult Result { get; private set; } = OptionsViewResult.Cancel;
        private void Ok()
        {
            Result = OptionsViewResult.Ok;
            MessengerInstance.Send(new CloseOptionsViewMessage());
        }
        private void Cancel()
        {
            Result = OptionsViewResult.Cancel;
            MessengerInstance.Send(new CloseOptionsViewMessage());
        }
    }
}
