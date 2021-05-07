using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenShareApp
{
    public partial class ScreenShareApp : Form
    {
        private int initialStyle;
        private bool transparentState;
        private Graphics myGraphics;
        private Bitmap memoryImage;
        private bool windowsSessionLocked;

        public ScreenShareApp()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myGraphics = this.CreateGraphics();
            initialStyle = WinAPI.User32Wrapper.GetWindowLong(this.Handle, WinAPI.GetWindowLongEnum.ExStyle);

            windowsSessionLocked = false;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            transparentState = false;
            SetOpaque();
            this.TopMost = true;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                windowsSessionLocked = true;
            }
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                windowsSessionLocked = false;
            }
        }

        private void ScreenShareApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.Control && e.Alt)
            {
                SwitchState();
            }
        }

        private void TimerScreenCapture_Tick(object sender, EventArgs e)
        {
            CheckSizeAndCaptureScreen();
        }

        private void SwitchState()
        {
            if (transparentState)
            {
                SetOpaque();
            }
            else
            {
                SetTransparent();
            }
            timerScreenCapture.Enabled = !timerScreenCapture.Enabled;
            transparentState = !transparentState;
        }

        private void SetTransparent()
        {
            WinAPI.User32Wrapper.SetWindowLong(
                this.Handle,
                WinAPI.GetWindowLongEnum.ExStyle,
                initialStyle | WinAPI.WindowsExtendedStyle.Layered | WinAPI.WindowsExtendedStyle.Transparent);

            WinAPI.User32Wrapper.SetLayeredWindowAttributes(
                this.Handle, 
                0, 
                20, 
                WinAPI.LayeredWindowAttribute.Alpha);

            pictureBox1.Visible = true;
            richTextBoxMessage.Visible = false;
        }

        private void SetOpaque()
        {
            WinAPI.User32Wrapper.SetWindowLong(
                this.Handle,
                WinAPI.GetWindowLongEnum.ExStyle,
                initialStyle | WinAPI.WindowsExtendedStyle.Layered);

            WinAPI.User32Wrapper.SetLayeredWindowAttributes(
                this.Handle,
                0,
                255,
                WinAPI.LayeredWindowAttribute.Alpha);

            pictureBox1.Visible = false;
            richTextBoxMessage.Visible = true;
        }

        private Point GetCaptureLocation()
        {
            Point captureLocation = new();
            int BorderWidth = (this.Width - this.ClientSize.Width) / 2;
            int TitlebarHeight = this.Height - this.ClientSize.Height - BorderWidth;

            captureLocation.X = this.Location.X + BorderWidth;
            captureLocation.Y = this.Location.Y + TitlebarHeight;

            return captureLocation;
        }

        private void CheckSizeAndCaptureScreen()
        {
            Size size = this.pictureBox1.Size;

            if (!windowsSessionLocked & size.Height > 0 & size.Width > 0)
            {
                CaptureScreen(size);
            }
        }

        private void CaptureScreen(Size size)
        {
            Point captureLocation = GetCaptureLocation();
            Bitmap oldImage = memoryImage;
            memoryImage = new Bitmap(size.Width, size.Height, myGraphics);
            
            using (Graphics memoryGraphics = Graphics.FromImage(memoryImage))
            {
                memoryGraphics.CopyFromScreen(captureLocation.X, captureLocation.Y, 0, 0, size);
                this.pictureBox1.Image = memoryImage;
            }

            if (oldImage != null)
                oldImage.Dispose();
        }

    }
}
