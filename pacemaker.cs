using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DualsensePacemaker
{
    public partial class DSForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private NotifyIcon notifyIcon;
        private Timer timer;
        private string targetProcessName = "";
        private string targetWindowTitle = "";

        public DSForm()
        {
            InitializeComponent();

            // Load an icon from a resource file or directly from a file
            Icon icon = new Icon("icon.png");

            // Create a NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = "Dualsense Pacemaker";
            notifyIcon.Icon = icon;
            notifyIcon.Visible = true;

            // Create a context menu for the NotifyIcon
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;

            // Create a timer to check for the target process and send keystrokes
            timer = new Timer();
            timer.Interval = 1000; // Check every second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(targetProcessName);
            if (processes.Length > 0)
            {
                IntPtr hWnd = GetForegroundWindow();
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hWnd, windowText, 256);

                if (windowText.ToString() == targetWindowTitle)
                {
                    SendKeys.SendWait("J");
                }
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            notifyIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
