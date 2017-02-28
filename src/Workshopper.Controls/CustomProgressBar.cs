﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Workshopper.Core;
using Steamworks;
using Workshopper.UI;

namespace Workshopper.Controls
{
    public partial class CustomProgressBar : UserControl
    {
        private static Image m_pImageProgressBar = Globals.GetTexture("bar");
        private Timer _timFrame;
        private PublishedFileId_t _fileID;
        private UGCUpdateHandle_t _progressHandle;
        private string _progressText;
        private DynamicLayoutLoader _layout;
        private bool _bDrawProgressReport;
        private ulong _bytesProcessed;
        private ulong _bytesTotal;
        public CustomProgressBar(PublishedFileId_t fileID)
        {
            InitializeComponent();

            _fileID = fileID;

            _timFrame = new Timer();
            _timFrame.Enabled = false;
            _timFrame.Interval = 5;
            _timFrame.Tick += new EventHandler(OnTick);

            _layout = DynamicLayoutLoader.LoadLayoutForControl("progressbar", this, true);
        }

        public void ShowProgress(UGCUpdateHandle_t handle)
        {
            _progressText = null;
            _progressHandle = handle;
            Visible = true;
            _timFrame.Enabled = true;
        }

        public void HideProgress()
        {
            _progressText = null;
            _timFrame.Enabled = false;
            Visible = false;
        }

        private void OnTick(object sender, EventArgs e)
        {
            EItemUpdateStatus uploadStatus = SteamUGC.GetItemUpdateProgress(_progressHandle, out _bytesProcessed, out _bytesTotal);
            Console.WriteLine(uploadStatus.ToString());

            _progressText = "Loading...";
            switch (uploadStatus)
            {
                case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig:
                    _progressText = "Preparing Content...";
                    break;
                case EItemUpdateStatus.k_EItemUpdateStatusPreparingContent:
                    _progressText = string.Format("Uploading Content: {0} / {1} bytes", _bytesProcessed, _bytesTotal);
                    break;
                case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent:
                case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile:
                    _progressText = "Configuring Content...";
                    break;
                case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges:
                    _progressText = "Committing Changes...";
                    break;
            }

            _bDrawProgressReport = ((uploadStatus == EItemUpdateStatus.k_EItemUpdateStatusPreparingContent) && (_bytesTotal > 0));

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            StringFormat formatter = new StringFormat();
            formatter.LineAlignment = StringAlignment.Center;
            formatter.Alignment = StringAlignment.Near;

            e.Graphics.DrawString(_progressText, Font, new SolidBrush(Color.White), _layout.GetResItemBounds("Label"), formatter);
            if (_bDrawProgressReport)
            {
                Rectangle progressBounds = _layout.GetResItemBounds("Bar");
                e.Graphics.DrawImage(m_pImageProgressBar, progressBounds);

                double flPercent = ((double)_bytesProcessed) / ((double)_bytesTotal);
                double flWide = ((double)(progressBounds.Width)) * flPercent;
                e.Graphics.FillRectangle(new SolidBrush(_layout.GetResItemFgColor("Bar")), new Rectangle(progressBounds.X, progressBounds.Y, (int)flWide, progressBounds.Height));
                e.Graphics.DrawRectangle(Pens.Black, progressBounds);
            }
        }
    }
}
