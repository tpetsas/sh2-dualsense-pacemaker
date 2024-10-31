using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace sh2_dualsense_pacemaker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timer;
        private string targetProcessName = "SHProto-Win64-Shipping";
        private string targetWindowTitle = "Silent Hill 2";

        public Form1()
        {
            InitializeComponent();

            // Create a NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = "Silent Hill 2 - Dualsense Pacemaker";
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Visible = true;

            // Create a context menu for the NotifyIcon
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.Load += new EventHandler(Form1_Load);

            // Create a timer to check for the target process and send keystrokes
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 15000; // Check every 15 seconds
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(targetProcessName);
            Debug.WriteLine("in timer tick! processes count: " + processes.Length);
            foreach (Process process in processes)
            {
                IntPtr hWnd = GetForegroundWindow();
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hWnd, windowText, 256);
                if (windowText.ToString() == process.MainWindowTitle && 
                    process.MainWindowTitle.StartsWith(targetWindowTitle)) 
                {
                    Debug.WriteLine("Our window found!");
                    SendKeys.SendWait("J");
                } else {
                    Debug.WriteLine(windowText.ToString());
                }
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            notifyIcon.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(0, 0);
        }
    }
}
