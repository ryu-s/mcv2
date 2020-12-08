using mcv2;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcv2TestPlugin
{

    [Export(typeof(IMcvPluginV1))]
    public class TestPluginMain : IMcvPluginV1, IPluginMain
    {
        public PluginId Id { get; } = new PluginId();
        public IPluginHost Host { get; set; }
        public string Name => "テストプラグイン";
        MainViewModel _vm;
        public void OnClosing()
        {
        }

        public void OnLoaded()
        {
            _vm = new MainViewModel(this);
        }

        public void SetNotify(INotify notify)
        {
            switch (notify)
            {
                case NotifyAddConnection addConn:
                    var connId = addConn.ConnectionId;
                    var resConnSt = Host.GetData(new RequestConnectionStatus(connId)) as ResponseConnectionStatusAdded;
                    _vm.AddConnection(addConn.ConnectionId, resConnSt.Name, resConnSt.Site);
                    break;
                case NotifyConnectionStatusChanged connChanged:
                    _vm.ChangeConnectionName(connChanged.ConnectionId, connChanged.Name, connChanged.Site);
                    break;
                case NotifyUserAdded userAdded:
                    var userStatus = (ResponseUser)Host.GetData(new RequestUser(userAdded.SiteContextGuid, userAdded.UserId));
                    _vm.AddUser(userStatus.User);
                    break;
                case NotifyUserChanged userChanged:
                    _vm.ChangeUser(userChanged);
                    break;
            }
        }

        public void SetResponse(IResponse res)
        {
        }

        public void ShowSettingView()
        {
            var resPos = Host.GetData(new RequestMainViewPosition()) as ResponseMainViewPosition;
            if (resPos == null)
            {
                //バグ報告
                return;
            }
            var resConns = Host.GetData(new RequestConnectionIds()) as ResponseConnectionIds;
            if (resConns == null)
            {
                //バグ報告
                return;
            }
            var list = new List<(ConnectionId, string)>();
            foreach (var connId in resConns.Ids)
            {
                var resConnStatus = Host.GetData(new RequestConnectionStatus(connId)) as ResponseConnectionStatusAdded;
                list.Add((resConnStatus.ConnectionId, resConnStatus.Name));
            }
            var left = resPos.X;
            var top = resPos.Y;
            var view = new MainView
            {
                Left = left,
                Top = top,
                DataContext = _vm
            };
            view.Show();
        }
        public void PostComment(ConnectionId connectionId, ISiteMessage message, IMessageMetadata2 metadata)
        {
            Host.SetRequest(Id, new RequestAddComment(connectionId, message, metadata));
        }

        public void SetRequest(IRequest req)
        {
            Host.SetRequest(Id, req);
        }
        public IResponse GetData(IRequest req)
        {
            return Host.GetData(req);
        }
    }
}
