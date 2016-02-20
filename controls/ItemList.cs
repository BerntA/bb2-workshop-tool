//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: A custom item list.
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using workshopper.core;
using workshopper.gui;

namespace workshopper.controls
{
    public partial class ItemList : UserControl
    {
        public event EventHandler OnItemClick;
        public bool bUseFixedWidth = true;
        private bool bDrawOverlay = false;
        private Rectangle overlayRect;
        private string m_TexturePath;
        public ItemList()
        {
            InitializeComponent();

            this.SetStyle(
System.Windows.Forms.ControlStyles.UserPaint |
System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
true);

            DoubleBuffered = true;

            BackColor = Color.FromArgb(22, 22, 22);
            ForeColor = Color.Transparent;
        }

        public void AddItem(string text)
        {
            int iControls = 0;
            int iIndex = -1;
            int iWidth = 0;
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is Label)
                {
                    iIndex = i;
                    iControls++;
                    if (Controls[i].Bounds.Width > iWidth)
                        iWidth = Controls[i].Bounds.Width;
                }
            }

            Label textItem = new Label();
            textItem.Parent = this;
            Font eFont = new System.Drawing.Font("Calibri", 7, FontStyle.Regular);
            textItem.Name = text;
            textItem.Text = text;
            textItem.Cursor = Cursors.Hand;
            textItem.BackColor = Color.Transparent;
            textItem.ForeColor = Color.White;
            textItem.TextAlign = ContentAlignment.MiddleCenter;
            textItem.Font = eFont;
            textItem.AutoSize = false;

            // ADD Events:
            textItem.Click += new EventHandler(ItemClicked);
            textItem.MouseEnter += new EventHandler(ItemEnter);
            textItem.MouseLeave += new EventHandler(ItemLeave);

            SizeF textSize = TextRenderer.MeasureText(textItem.Text, textItem.Font);

            if (iIndex >= 0)
                textItem.Bounds = new Rectangle(0, Controls[iIndex].Bounds.Height + Controls[iIndex].Bounds.Y, bUseFixedWidth ? (int)textSize.Width : Width, 20);
            else
                textItem.Bounds = new Rectangle(0, 0, bUseFixedWidth ? (int)textSize.Width : Width, 20);

            if (bUseFixedWidth)
            {
                if ((int)textSize.Width > iWidth)
                    Width = (int)textSize.Width;
                else
                    Width = iWidth;
            }

            int iNewHeight = 20 + (iControls * 20);
            int iParentHeight = (this.Parent != null) ? this.Parent.Height : 0;

            if (iNewHeight + 40 >= iParentHeight)
            {
                AutoScroll = true;
                HorizontalScroll.Enabled = false;
                HorizontalScroll.Visible = false;
                AutoScrollMinSize = new Size(800, Parent.Height - 40);
            }
            else
                Height = iNewHeight;

            Invalidate();
        }

        public void RemoveAllItems()
        {
            for (int ix = this.Controls.Count - 1; ix >= 0; ix--)
            {
                if (this.Controls[ix] is Label) this.Controls[ix].Dispose();
            }
        }

        private void ItemClicked(object sender, EventArgs e)
        {
            OnItemClick(sender, e);
        }

        private void ItemEnter(object sender, EventArgs e)
        {
            overlayRect = ((Label)sender).Bounds;
            overlayRect.Width = Width;
            bDrawOverlay = true;
            Invalidate();
        }

        private void ItemLeave(object sender, EventArgs e)
        {
            bDrawOverlay = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!DesignMode)
            {
                if (bDrawOverlay)
                {
                    m_TexturePath = string.Format("{0}\\workshopper\\Controls\\GroupSelection_ActiveHover.png", globals.GetTexturePath());

                    if (m_TexturePath != null && File.Exists(m_TexturePath))
                        e.Graphics.DrawImage(Image.FromFile(m_TexturePath), overlayRect);
                }
            }
        }
    }
}
