using System;
using System.Drawing;
using Forms = System.Windows.Forms;
using Resx = PoeBadMapsMod.resources.lang.Resources;
using AutoUpdaterDotNET;

namespace PoeBadMapsMod
{
    class MyNotifyIcon
    {
        private readonly Forms.NotifyIcon _notifyIcon;

        private Program program;

        private KeysManager keysManager;

        public MyNotifyIcon(Program program,KeysManager keysManager)
        {
            this.program = program;
            this.keysManager = keysManager;
            _notifyIcon = new();
            _notifyIcon.Icon = new System.Drawing.Icon("resources/icon/LogoText.ico");
            _notifyIcon.Text = "PoeBadMapsMod";
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconCheckUpdates, null, onUpdatesClick);
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconSettings, null, onSettingsClick);
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconGitHubPage, null, onGitHubClick);
            _notifyIcon.ContextMenuStrip.Items.Add(Resx.NotifyIconExit, null, onExitClick);
           
        }

        private void onGitHubClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void onUpdatesClick(object sender, EventArgs e)
        {
            AutoUpdater.Start("https://raw.githubusercontent.com/Mussk/PoeMapFilter/master/Update/Version.xml");
        }

        private void onSettingsClick(object sender, EventArgs e)
        {
            new Settings(keysManager, program);
        }

        private void onExitClick(object sender, EventArgs e)
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
