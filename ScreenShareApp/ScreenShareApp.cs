﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenShareApp
{
    public partial class ScreenShareApp : Form
    {
        private int initialStyle;
        private bool transparentState;
        private Graphics myGraphics;
        private Bitmap memoryImage;

        public ScreenShareApp()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myGraphics = this.CreateGraphics();
            initialStyle = WinAPI.User32Wrapper.GetWindowLong(this.Handle, WinAPI.GetWindowLongEnum.ExStyle);

            transparentState = false;
            SetOpaque();
            this.TopMost = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.Control && e.Alt)
            {
                SwitchState();
            }
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

        private void CaptureScreen()
        {
            Point captureLocation = GetCaptureLocation();
            Size s = this.pictureBox1.Size;
            Bitmap oldImage = memoryImage;
            memoryImage = new Bitmap(s.Width, s.Height, myGraphics);

            using (Graphics memoryGraphics = Graphics.FromImage(memoryImage))
            {
                memoryGraphics.CopyFromScreen(captureLocation.X, captureLocation.Y, 0, 0, s);
                this.pictureBox1.Image = memoryImage;
            }

            if (oldImage != null)
                oldImage.Dispose();
        }

        private void TimerScreenCapture_Tick(object sender, EventArgs e)
        {
            CaptureScreen();
        }
    }
}
