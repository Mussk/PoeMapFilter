using PoeMapFIlter;
using System;
using Forms = System.Windows.Forms;
using Resx = PoeMapFIlter.resources.lang.Resources;

namespace PoeMapFilter
{
    class MyNotifyIcon
    {
        private readonly Forms.NotifyIcon _notifyIcon;

        private MapWindowLogic MapWindowLogic;

        private KeysManager keysManager;

        public MyNotifyIcon(MapWindowLogic MapWindowLogic,KeysManager keysManager)
        {
            this.MapWindowLogic = MapWindowLogic;
            this.keysManager = keysManager;
            _notifyIcon = new();
            _notifyIcon.Icon = new System.Drawing.Icon("resources/icon/LogoText.ico");
            _notifyIcon.Text = "PoeMapFilter";
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconCheckUpdates, null, OnUpdatesClick);
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconSettings, null, OnSettingsClick);
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconGitHubPage, null, OnGitHubClick);
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconExit, null, OnExitClick);
           
        }

        private void OnGitHubClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Links.GitHubPage);
        }

        private void OnUpdatesClick(object sender, EventArgs e)
        {
            new UpdateHandler();
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            new Settings(keysManager, MapWindowLogic);
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            MyDispose();

            System.Windows.Application.Current.Shutdown();
        }

        public void MyDispose()
        {
            _notifyIcon.Dispose();
        }
    }
}
