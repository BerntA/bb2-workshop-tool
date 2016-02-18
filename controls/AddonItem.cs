//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: UGC Addon item
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Steamworks;
using workshopper.core;

namespace workshopper.controls
{
    public partial class AddonItem : UserControl
    {
        public PublishedFileId_t GetItemFileID() { return ulFileID; }

        private Color colOverlay;
        private bool m_bActive;
        private bool m_bWhitelisted;
        private PublishedFileId_t ulFileID;
        private string pszName;
        private string pszDescription;
        private string pszTags;
        private int m_iVisibility;
        private string pszDate;
        private ImageButton btnUpdate;

        // Upload Definitions:
        private bool m_bUploading;
        private UGCUpdateHandle_t updateHandle;

        public AddonItem(string title, string description, string tags, int visibility, PublishedFileId_t fileID, string lastChangeDate)
        {
            InitializeComponent();
            pszName = title;
            pszDescription = description;
            pszTags = tags;
            m_iVisibility = visibility;
            ulFileID = fileID;
            pszDate = lastChangeDate;
            colOverlay = Color.FromArgb(100, 25, 25, 25);
            m_bActive = false;
            m_bUploading = false;
            btnUpdate = null;
            timFrame.Enabled = false;
            m_bWhitelisted = pszTags.Contains("Whitelisted");
        }

        public void UpdateItem(string title, string description, string tags, int visibility, string lastChangeDate)
        {
            pszName = title;
            pszDescription = description;
            pszTags = tags;
            m_iVisibility = visibility;
            pszDate = lastChangeDate;
            m_bUploading = false;
            m_bWhitelisted = pszTags.Contains("Whitelisted");
            Invalidate();
        }

        public void StartUploading(UGCUpdateHandle_t updHandle)
        {
            updateHandle = updHandle;
            m_bUploading = timFrame.Enabled = true;

            if (btnUpdate != null)
                btnUpdate.Visible = btnUpdate.Enabled = false;

            Invalidate();
        }

        public void StopUploading()
        {
            m_bUploading = timFrame.Enabled = false;

            if (btnUpdate != null)
                btnUpdate.Visible = btnUpdate.Enabled = true;

            Invalidate();
        }

        public void SetupItem()
        {
            btnUpdate = new ImageButton("update", "update_hover");
            btnUpdate.Parent = this;
            btnUpdate.Bounds = new Rectangle(Width - 72, Height - 30, 70, 24);
            btnUpdate.Click += new EventHandler(OnClickUpdate);
            btnUpdate.MouseEnter += new EventHandler(OnEnterUpdateButton);
            btnUpdate.MouseLeave += new EventHandler(OnLeaveUpdateButton);

            if (m_bUploading)
                btnUpdate.Visible = false;
        }

        public bool IsActive() { return m_bActive; }
        public void Activate(bool value)
        {
            m_bActive = value;

            if (m_bActive)
                colOverlay = Color.FromArgb(100, 150, 25, 25);
            else
                colOverlay = Color.FromArgb(100, 25, 25, 25);

            if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
                DoRollover(true);

            Invalidate();
        }

        private void OnClickUpdate(object sender, EventArgs e)
        {
            if (m_bUploading)
                return;

            CreationPanel panel = new CreationPanel(pszName, pszDescription, pszTags, m_iVisibility, ulFileID);
            panel.ShowDialog(this);
        }

        private void OnEnterUpdateButton(object sender, EventArgs e)
        {
            DoRollover(true);
        }

        private void OnLeaveUpdateButton(object sender, EventArgs e)
        {
            DoRollover();
        }

        private void DoRollover(bool bOver = false)
        {
            if (m_bActive)
                return;

            if (!bOver)
                colOverlay = Color.FromArgb(100, 25, 25, 25);
            else
                colOverlay = Color.FromArgb(100, 150, 25, 25);

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            DoRollover(true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            DoRollover();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            factory_list_init._mainFormAccessor._addonList.ActivateItem(this, !m_bActive);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(globals.GetTexture(ulFileID.ToString(), "jpg"), new Rectangle(1, 1, Height - 2, Height - 2));

            if (m_bWhitelisted)
                e.Graphics.DrawImage(Properties.Resources.verified, new Rectangle(Height - 18, Height - 18, 18, 18));

            e.Graphics.FillRectangle(new SolidBrush(colOverlay), new Rectangle(0, 0, Width, Height));
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(1, 1, Height - 2, Height - 2));

            StringFormat formatter = new StringFormat();
            formatter.LineAlignment = StringAlignment.Center;
            formatter.Alignment = StringAlignment.Near;

            if (m_bUploading)
            {
                bool bDrawProgress = false;
                string statusString = "";
                ulong punBytesProcessed = 0, punBytesTotal = 0;
                EItemUpdateStatus uploadStatus = SteamUGC.GetItemUpdateProgress(updateHandle, out punBytesProcessed, out punBytesTotal);

                if (uploadStatus == EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig)
                    statusString = "Preparing Content...";
                else if (uploadStatus == EItemUpdateStatus.k_EItemUpdateStatusPreparingContent)
                {
                    statusString = string.Format("Uploading Content: {0} / {1} bytes", punBytesProcessed, punBytesTotal);
                    bDrawProgress = true;
                }
                else if (uploadStatus == EItemUpdateStatus.k_EItemUpdateStatusUploadingContent || uploadStatus == EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile)
                {
                    statusString = "Configuring Content...";
                }
                else if (uploadStatus == EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges)
                    statusString = "Committing Changes...";

                e.Graphics.DrawString(statusString, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.White), new Rectangle(Height + 2, 1, Width - Height - 2, 30), formatter);
                if (bDrawProgress && (punBytesTotal > 0))
                {
                    e.Graphics.DrawImage(globals.GetTexture("bar"), new Rectangle(Height + 2, 33, Width - Height - 24, 16));

                    double flPercent = ((double)punBytesProcessed) / ((double)punBytesTotal);
                    double flWide = (double)(Width - Height - 24) * flPercent;
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(145, 200, 15, 15)), new Rectangle(Height + 2, 33, (int)flWide, 16));
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(Height + 2, 33, Width - Height - 24, 16));
                }

                return;
            }

            e.Graphics.DrawString(pszName, new Font("Times New Roman", 20, FontStyle.Bold), new SolidBrush(Color.White), new Rectangle(Height + 2, 1, Width - Height - 2, 30), formatter);
            if (m_bWhitelisted)
                e.Graphics.DrawString("Your map is whitelisted!", new Font("Times New Roman", 10, FontStyle.Regular), new SolidBrush(Color.Green), new Rectangle(Height + 3, 33, Width - Height - 3, 20), formatter);

            formatter.LineAlignment = StringAlignment.Center;
            formatter.Alignment = StringAlignment.Far;

            e.Graphics.DrawString(pszDate, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.White), new Rectangle(Width - 142, 2, 140, 20), formatter);
        }

        private void timFrame_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
