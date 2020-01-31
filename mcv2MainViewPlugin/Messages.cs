using GalaSoft.MvvmLight.Messaging;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcv2MainViewPlugin.Update;

namespace mcv2.MainViewPlugin
{
    class ShowOptionsViewMessage : MessageBase
    {
        public OptionsViewModel OptionsViewModel { get; }

        public ShowOptionsViewMessage(OptionsViewModel optionsViewModel)
        {
            OptionsViewModel = optionsViewModel;
        }
    }
    class CloseOptionsViewMessage : MessageBase { }
    class ShowUserViewMessage : MessageBase
    {
        public UserViewModel Uvm { get; }
        public ShowUserViewMessage(UserViewModel uvm)
        {
            Uvm = uvm;
        }
    }
    class ShowUpdateViewMessage : MessageBase
    {
        public UpdateViewModel Vm { get; }
        public ShowUpdateViewMessage(UpdateViewModel vm)
        {
            Vm = vm;
        }
    }
    class ShowCommentPostPanelMessage : MessageBase
    {
        public ShowCommentPostPanelMessage(SiteType site)
        {
            Site = site;
        }

        public SiteType Site { get; }
    }

}
