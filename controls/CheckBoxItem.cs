//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: A more appealing checkbox control
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
using workshopper.core;

namespace workshopper.controls
{
    public partial class CheckBoxItem : UserControl
    {
        enum CheckBoxStates
        {
            STATE_DEF = 0,
            STATE_HOVER = 1,
            STATE_ACTIVATED = 2,
        }

        // This will check if the item will be enabled or disabled
        enum CheckBoxEnabled
        {
            STATE_ENABLED = 0,
            STATE_DISABLED = 1,
        }

        public bool IsItemChecked() { return (m_iItemState >= 2); } // Is it already activated?
        public bool IsItemEnabled() { return (m_iItemEnabled <= 0); } // Lets check if its enabled, so we can use it

        // Lets activate the item
        public void ActiviateItem(bool value)
        {
            if (value)
                m_iItemState = (int)CheckBoxStates.STATE_ACTIVATED;
            else
                m_iItemState = (int)CheckBoxStates.STATE_DEF;

            Invalidate();
        }

        // Lets enable the item
        public void EnableItem(bool value)
        {
            if (value)
                m_iItemEnabled = (int)CheckBoxEnabled.STATE_ENABLED;
            else
                m_iItemEnabled = (int)CheckBoxEnabled.STATE_DISABLED;

            Invalidate();
        }

        // Whats the checkbox text?
        public string GetText() { return pszText; }
        public string SetText(string text)
        {
            pszText = text;
            return pszText;
        }

        private int m_iItemState;
        private int m_iItemEnabled;
        private string pszText;
        public CheckBoxItem(string text)
        {
            InitializeComponent();
            pszText = text;
            m_iItemState = (int)CheckBoxStates.STATE_DEF;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // Lets check if its enabled, so we can use it
            if (m_iItemEnabled == (int)CheckBoxEnabled.STATE_DISABLED)
                return;

            // Lets check if its already pressed
            if (m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED)
                return;

            base.OnMouseEnter(e);
            m_iItemState = (int)CheckBoxStates.STATE_HOVER;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Lets check if its enabled, so we can use it
            if (m_iItemEnabled == (int)CheckBoxEnabled.STATE_DISABLED)
                return;

            // Lets check if its already pressed
            if (m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED)
                return;

            base.OnMouseLeave(e);
            m_iItemState = (int)CheckBoxStates.STATE_DEF;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            // Lets check if its enabled, so we can use it
            if (m_iItemEnabled == (int)CheckBoxEnabled.STATE_DISABLED)
                return;

            // Lets check the states if we are hovering or if its activated
            if (m_iItemState != (int)CheckBoxStates.STATE_ACTIVATED)
                m_iItemState = (int)CheckBoxStates.STATE_ACTIVATED;
            else
                m_iItemState = (int)CheckBoxStates.STATE_HOVER;

            Invalidate();

            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // If Activated
            if (m_iItemEnabled == (int)CheckBoxEnabled.STATE_DISABLED)
                e.Graphics.DrawImage(globals.GetTexture("CBox_Check_Disabled"), new Rectangle(0, 0, Height, Height));
            else
            {
                if (m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED)
                    e.Graphics.DrawImage(globals.GetTexture("CBox_Check"), new Rectangle(0, 0, Height, Height));
                else // If now activated, lets uncheck it
                    e.Graphics.DrawImage(globals.GetTexture("CBox_UnCheck"), new Rectangle(0, 0, Height, Height));
            }

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Near;

            e.Graphics.DrawString(pszText, Font, (m_iItemState == 0 ? Brushes.White : Brushes.DarkRed), new Rectangle(Height, 0, Width - Height, Height), format);
        }
    }
}
