//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: UGC Creation/Updating Form
//
//=============================================================================================//

using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using workshopper.controls;
using workshopper.core;
using workshopper.gui;

namespace workshopper
{
    public partial class CreationPanel : BaseForm
    {
        private RadioButton[] m_pVisibilityChoices;
        private TextBox m_pTitle;
        private RichTextBox m_pPatchNotes;
        private RichTextBox m_pDescription;
        private TextBox[] m_pLabelFields;
        private CheckBoxItem[] m_pCheckBox;
        private ComboBox m_pContestTags;
        private List<string> pszTagList;
        private string pszImagePath;
        private string pszContentPath;
        private PublishedFileId_t itemUniqueID;

        private OpenFileDialog _fileDialog;
        private FolderBrowserDialog _folderDialog;
        private float flStandardHeight;
        private bool m_bShouldUpdateItem;
        private bool m_bHasChangedImagePath;

        public CreationPanel()
        {
            pszTagList = new List<string>();
            pszImagePath = null;
            pszContentPath = null;
            m_bShouldUpdateItem = false;
            m_bHasChangedImagePath = false;
        }

        public CreationPanel(string title, string description, string tags, int visibility, PublishedFileId_t fileID)
        {
            pszTagList = new List<string>();
            m_bShouldUpdateItem = true;
            m_bHasChangedImagePath = false;
            Text = "Update Addon";

            // Set the stuff:
            pszContentPath = null;
            pszImagePath = string.Format("{0}\\workshopper\\addons\\{1}.jpg", globals.GetTexturePath(), fileID.ToString());

            m_pTitle.Text = title;
            m_pDescription.Text = description;
            itemUniqueID = fileID;

            if (visibility == 0)
                m_pVisibilityChoices[0].Select();
            else if (visibility == 1)
                m_pVisibilityChoices[1].Select();
            else if (visibility == 2)
                m_pVisibilityChoices[2].Select();

            GetTagsFromString(tags);

            /*
             0 "Story Mode",
             1 "Objective",
             2 "Elimination",
             3 "Arena",
             4 "Weapons",
             5 "NPCs",
             6 "Survivors",
             7 "Survivor Voicesets",
             8 "NPC Voicesets",
             9 "Weapons Sounds",
             10 "Misc Sounds",
             11 "Textures"
            */

            for (int i = 0; i < pszTagList.Count(); i++)
            {
                // Debugging
                //utils.LogAction(m_pCheckBox[0]);
                if (pszTagList[i] == "Story Mode")
                {
                    SetTags(0);
                    SelectType.LabelTxt = utils.GetWorkshopType[0];
                    m_pCheckBox[0].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Objective")
                {
                    SetTags(0);
                    SelectType.LabelTxt = utils.GetWorkshopType[0];
                    m_pCheckBox[1].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Elimination")
                {
                    SetTags(0);
                    SelectType.LabelTxt = utils.GetWorkshopType[0];
                    m_pCheckBox[2].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Arena")
                {
                    SetTags(0);
                    SelectType.LabelTxt = utils.GetWorkshopType[0];
                    m_pCheckBox[3].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Weapons")
                {
                    SetTags(1);
                    SelectType.LabelTxt = utils.GetWorkshopType[1];
                    m_pCheckBox[0].ActiviateItem(true);
                }
                else if (pszTagList[i] == "NPCs")
                {
                    SetTags(1);
                    SelectType.LabelTxt = utils.GetWorkshopType[1];
                    m_pCheckBox[1].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Survivors")
                {
                    SetTags(1);
                    SelectType.LabelTxt = utils.GetWorkshopType[1];
                    m_pCheckBox[2].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Survivor Voicesets")
                {
                    SetTags(2);
                    SelectType.LabelTxt = utils.GetWorkshopType[2];
                    m_pCheckBox[0].ActiviateItem(true);
                }
                else if (pszTagList[i] == "NPC Voicesets")
                {
                    SetTags(2);
                    SelectType.LabelTxt = utils.GetWorkshopType[2];
                    m_pCheckBox[1].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Weapons Sounds")
                {
                    SetTags(2);
                    SelectType.LabelTxt = utils.GetWorkshopType[2];
                    m_pCheckBox[2].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Misc Sounds")
                {
                    SetTags(2);
                    SelectType.LabelTxt = utils.GetWorkshopType[2];
                    m_pCheckBox[3].ActiviateItem(true);
                }
                else if (pszTagList[i] == "Textures")
                {
                    if (SelectType.LabelTxt == utils.GetWorkshopType[0])
                        m_pCheckBox[4].ActiviateItem(true);
                    else
                        m_pCheckBox[3].ActiviateItem(true);
                }

            }

            // Lets remove the previous tags.
            for (int i = 0; i < utils.GetAvailableTags[i].Count(); i++)
                RemoveTag(utils.GetAvailableTags[i]);

            m_pPatchNotes.Visible = m_pPatchNotes.Enabled = true;

            Invalidate();
        }

        private void SelectType_Click(object sender, EventArgs e)
        {
            SelectType_List.Visible = !SelectType_List.Visible;
            SelectType_List.BringToFront();
        }

        protected override void OnFormCreate(float percentW, float percentH)
        {
            InitializeComponent();
            Opacity = 0;
            flStandardHeight = ((float)Height * 0.075F);

            SelectType_List.Visible = false;
            SelectType_List.bUseFixedWidth = false;

            // Adds the workshop types
            for (int i = 0; i < utils.GetWorkshopType.Count(); i++)
                SelectType_List.AddItem(utils.GetWorkshopType[i]);

            SelectType.LabelTxt = "Select a workshop type";

            _fileDialog = new OpenFileDialog();
            _fileDialog.DefaultExt = ".jpg";
            _fileDialog.CheckFileExists = true;
            _fileDialog.CheckPathExists = true;
            _fileDialog.Title = "Select an image";
            _fileDialog.AddExtension = true;
            _fileDialog.Multiselect = false;
            _fileDialog.Filter = "JPG files|*.jpg";
            _fileDialog.FileOk += new CancelEventHandler(OnSelectImage);
            _fileDialog.InitialDirectory = utils.GetGameDirectory((AppId_t)346330);

            _folderDialog = new FolderBrowserDialog();
            _folderDialog.SelectedPath = utils.GetGameDirectory((AppId_t)346330);

            TextButton btnImg = new TextButton("Select Image:", Color.White, Color.Red);
            btnImg.Parent = this;
            btnImg.Bounds = new Rectangle(2, Height - 230, 100, 20);
            btnImg.Click += new EventHandler(OnOpenImageSelection);

            TextButton btnFile = new TextButton("Select File Dir:", Color.White, Color.Red);
            btnFile.Parent = this;
            btnFile.Bounds = new Rectangle(2, Height - 210, 100, 20);
            btnFile.Click += new EventHandler(OnOpenFolderDialog);

            SelectType_List.OnItemClick += new EventHandler(OnSelectType);

            m_pLabelFields = new TextBox[2];
            for (int i = 0; i < 2; i++)
            {
                m_pLabelFields[i] = new TextBox();
                m_pLabelFields[i].Parent = this;
                m_pLabelFields[i].ForeColor = Color.White;
                m_pLabelFields[i].BackColor = Color.FromArgb(45, 45, 45);
                m_pLabelFields[i].AutoSize = false;
                m_pLabelFields[i].ReadOnly = true;
                m_pLabelFields[i].BorderStyle = BorderStyle.None;
                m_pLabelFields[i].Multiline = false;
                m_pLabelFields[i].TextAlign = HorizontalAlignment.Left;
                m_pLabelFields[i].Font = new Font("Arial", 10, FontStyle.Regular);
            }

            Size strSize = TextRenderer.MeasureText("Select Image:", m_pLabelFields[0].Font);
            m_pLabelFields[0].Bounds = new Rectangle(2 + strSize.Width, Height - 230, 300 - strSize.Width + 2, strSize.Height);
            strSize = TextRenderer.MeasureText("Select File Dir:", m_pLabelFields[1].Font);
            m_pLabelFields[1].Bounds = new Rectangle(2 + strSize.Width, Height - 210, 300 - strSize.Width + 2, strSize.Height);

            m_pCheckBox = new CheckBoxItem[5];

            for (int i = 0; i < 5; i++)
            {
                m_pCheckBox[i] = new CheckBoxItem(utils.GetAvailableTags[i]);
                m_pCheckBox[i].Parent = this;
                m_pCheckBox[i].Click += new EventHandler(OnTagClicked);
            }

            m_pCheckBox[0].Bounds = new Rectangle(6, (Height - 160) + 0, 150, 20);
            m_pCheckBox[1].Bounds = new Rectangle(6, (Height - 160) + 20, 150, 20);
            m_pCheckBox[2].Bounds = new Rectangle(6, (Height - 160) + 40, 150, 20);
            m_pCheckBox[3].Bounds = new Rectangle(6, (Height - 160) + 60, 150, 20);
            m_pCheckBox[4].Bounds = new Rectangle(6, (Height - 160) + 80, 150, 20);

            // Lets hide em
            m_pCheckBox[0].Hide();
            m_pCheckBox[1].Hide();
            m_pCheckBox[2].Hide();
            m_pCheckBox[3].Hide();
            m_pCheckBox[4].Hide();

            m_pContestTags = new ComboBox();
            m_pContestTags.Parent = this;
            m_pContestTags.Bounds = new Rectangle(310, Height - 30, 160, 15);
            m_pContestTags.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pContestTags.Cursor = Cursors.Hand;
            m_pContestTags.Items.Insert(0, "None");
            utils.AddContestItems(m_pContestTags);
            m_pContestTags.SelectedIndex = 0;

            m_pTitle = new TextBox();
            m_pTitle.Parent = this;
            m_pTitle.Multiline = false;
            m_pTitle.Font = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            m_pTitle.Bounds = new Rectangle(310, (int)flStandardHeight + 20, Width - 320, 8);
            m_pTitle.ForeColor = Color.White;
            m_pTitle.BackColor = BackColor;
            m_pTitle.BorderStyle = BorderStyle.FixedSingle;

            m_pDescription = new RichTextBox();
            m_pDescription.Parent = this;
            m_pDescription.ForeColor = Color.White;
            m_pDescription.BackColor = BackColor;
            m_pDescription.BorderStyle = BorderStyle.FixedSingle;
            m_pDescription.Font = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            m_pDescription.Bounds = new Rectangle(310, (int)flStandardHeight + 65, Width - 320, 175);

            m_pPatchNotes = new RichTextBox();
            m_pPatchNotes.Parent = this;
            m_pPatchNotes.ForeColor = Color.White;
            m_pPatchNotes.BackColor = BackColor;
            m_pPatchNotes.BorderStyle = BorderStyle.FixedSingle;
            m_pPatchNotes.Font = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            m_pPatchNotes.Bounds = new Rectangle(380, (int)flStandardHeight + 275, Width - 390, 54);
            m_pPatchNotes.Visible = m_pPatchNotes.Enabled = false;

            ImageButton btnUpload = new ImageButton("upload", "upload_hover");
            btnUpload.Parent = this;
            btnUpload.Bounds = new Rectangle(Width - 94, Height - 34, 90, 30);
            btnUpload.Click += new EventHandler(OnUploadAddon);

            m_pVisibilityChoices = new RadioButton[3];
            for (int i = 0; i < 3; i++)
            {
                m_pVisibilityChoices[i] = new RadioButton();
                m_pVisibilityChoices[i].Parent = this;
                m_pVisibilityChoices[i].Font = new Font("Arial", 9, FontStyle.Regular);
                m_pVisibilityChoices[i].ForeColor = Color.White;
                m_pVisibilityChoices[i].BackColor = Color.Transparent;
                m_pVisibilityChoices[i].AutoSize = false;
                m_pVisibilityChoices[i].Bounds = new Rectangle(310, ((int)flStandardHeight + 270) + (i * 18), 100, 18);
                m_pVisibilityChoices[i].Cursor = Cursors.Hand;
            }

            m_pVisibilityChoices[0].Text = "Public";
            m_pVisibilityChoices[1].Text = "Private";
            m_pVisibilityChoices[2].Text = "Hidden";

            m_pVisibilityChoices[0].Select();

            UGCHandler.OnCreateWorkshopItem += new UGCHandler.ItemCreatedHandler(OnCreateItem);

            base.OnFormCreate(0.07F, 0.07F);
        }

        private void OnSelectType(object sender, EventArgs e)
        {
            string szItem = ((Label)sender).Text;

            SelectType.LabelTxt = szItem;
            SelectType_List.Visible = false;

            // Lets check what type it is
            if (szItem == utils.GetWorkshopType[0])
                SetTags(0, true);
            else if (szItem == utils.GetWorkshopType[1])
                SetTags(1, true);
            else if (szItem == utils.GetWorkshopType[2])
                SetTags(2, true);

            // Lets reset everything...
            for (int i = 0; i < 5; i++)
                m_pCheckBox[i].ActiviateItem(false);

            for (int i = 0; i < utils.GetAvailableTags[i].Count(); i++)
                RemoveTag(utils.GetAvailableTags[i]);
        }

        private void SetTags(int tagtype, bool listchange=false)
        {
            if (listchange)
            {
                // Deactivate them
                m_pCheckBox[0].ActiviateItem(false);
                m_pCheckBox[1].ActiviateItem(false);
                m_pCheckBox[2].ActiviateItem(false);
                m_pCheckBox[3].ActiviateItem(false);
                m_pCheckBox[4].ActiviateItem(false);
            }

            // Lets make sure they are shown
            m_pCheckBox[0].Show();
            m_pCheckBox[1].Show();
            m_pCheckBox[2].Show();
            m_pCheckBox[3].Show();

            /*
             0 "Story Mode",
             1 "Objective",
             2 "Elimination",
             3 "Arena",
             4 "Weapons",
             5 "NPCs",
             6 "Survivors",
             7 "Survivor Voicesets",
             8 "NPC Voicesets",
             9 "Weapons Sounds",
             10 "Misc Sounds",
             11 "Textures"
            */
            // Lets check what type it is
            if (tagtype == 0)
            {
                m_pCheckBox[0].SetText("Story Mode");
                m_pCheckBox[1].SetText("Objective");
                m_pCheckBox[2].SetText("Elimination");
                m_pCheckBox[3].SetText("Arena");
                m_pCheckBox[4].SetText("Textures");

                // Bounds
                m_pCheckBox[0].Bounds = new Rectangle(6, (Height - 160) + 0, 100, 20);
                m_pCheckBox[3].Bounds = new Rectangle(6, (Height - 160) + 60, 150, 20);
                m_pCheckBox[4].Bounds = new Rectangle(106, (Height - 160) + 0, 100, 20);

                m_pCheckBox[4].Show();
            }
            else if (tagtype == 1)
            {
                m_pCheckBox[0].SetText("Weapons");
                m_pCheckBox[1].SetText("NPCs");
                m_pCheckBox[2].SetText("Survivors");
                m_pCheckBox[3].SetText("Textures");

                // Bounds
                m_pCheckBox[0].Bounds = new Rectangle(6, (Height - 160) + 0, 100, 20);
                m_pCheckBox[3].Bounds = new Rectangle(106, (Height - 160) + 0, 100, 20);

                m_pCheckBox[4].Hide();
            }
            else if (tagtype == 2)
            {
                m_pCheckBox[0].SetText("Survivor Voicesets");
                m_pCheckBox[1].SetText("NPC Voicesets");
                m_pCheckBox[2].SetText("Weapons Sounds");
                m_pCheckBox[3].SetText("Misc Sounds");

                // Bounds
                m_pCheckBox[0].Bounds = new Rectangle(6, (Height - 160) + 0, 150, 20);
                m_pCheckBox[3].Bounds = new Rectangle(156, (Height - 160) + 0, 100, 20);

                m_pCheckBox[4].Hide();
            }
        }

        private void SubmitWorkshopItem(PublishedFileId_t fileID, string changelog)
        {
            int iVisibility = 0;
            if (m_pVisibilityChoices[1].Checked)
                iVisibility = 1;
            else if (m_pVisibilityChoices[2].Checked)
                iVisibility = 2;

            try
            {
                File.Copy(pszImagePath, string.Format("{0}\\workshopper\\addons\\{1}.jpg", globals.GetTexturePath(), fileID.m_PublishedFileId.ToString()), true);
            }
            catch (Exception ex)
            {
                utils.LogAction(ex.Message);
            }
            finally
            {
                UGCHandler.SubmitItem(fileID, m_pTitle.Text, m_pDescription.Text, iVisibility, pszContentPath, pszImagePath, pszTagList, changelog, m_bShouldUpdateItem);
                Close();
            }
        }

        private void OnCreateItem(object sender, UGCCreationEventArg e)
        {
            SubmitWorkshopItem(e.FileID, "Initial Release");
        }

        private void OnTagClicked(object sender, EventArgs e)
        {
            CheckBoxItem pItem = ((CheckBoxItem)sender);
            if (pItem != null)
            {
                if (pItem.IsItemChecked())
                    AddTag(pItem.GetText());
                else
                    RemoveTag(pItem.GetText());
            }
        }

        private bool AddTag(string tag)
        {
            bool bCanAdd = true;
            for (int i = 0; i < pszTagList.Count(); i++)
            {
                if (pszTagList[i] == tag)
                {
                    bCanAdd = false;
                    break;
                }
            }

            if (!bCanAdd)
                return false;

            pszTagList.Add(tag);
            return true;
        }

        private bool RemoveTag(string tag)
        {
            bool bRemoved = false;

            for (int i = (pszTagList.Count() - 1); i >= 0; i--)
            {
                if (pszTagList[i] == tag)
                {
                    bRemoved = true;
                    pszTagList.RemoveAt(i);
                    break;
                }
            }

            return bRemoved;
        }

        private void GetTagsFromString(string tags)
        {
            string tagsCopy = tags;
            while (tagsCopy.Contains(','))
            {
                int index = tagsCopy.IndexOf(',', 0);
                string tagStripped = tagsCopy.Substring(0, index);
                tagsCopy = tagsCopy.Replace(tagStripped + ",", "");
                pszTagList.Add(tagStripped);
            }
        }

        private bool IsStringValid(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr) || string.IsNullOrWhiteSpace(inputStr))
                return false;

            Regex r = new Regex(@"^[^~`^<>""'\\/]+$");
            return r.IsMatch(inputStr);
        }

        private void OnUploadAddon(object sender, EventArgs e)
        {
            string imagePreview = (m_bHasChangedImagePath ? pszImagePath : null);

            if (string.IsNullOrEmpty(m_pTitle.Text) || string.IsNullOrEmpty(m_pDescription.Text))
            {
                utils.ShowWarningDialog("Please fill out the fields first!", null, true);
                return;
            }

            if (pszTagList.Count() <= 0)
            {
                utils.ShowWarningDialog("You have to select at least one tag!", null, true);
                return;
            }

            if (string.IsNullOrEmpty(imagePreview) && !m_bShouldUpdateItem)
            {
                utils.ShowWarningDialog("You have to select an image!", null, true);
                return;
            }

            if (string.IsNullOrEmpty(pszContentPath) && !m_bShouldUpdateItem)
            {
                utils.ShowWarningDialog("You have to select the content directory!", null, true);
                return;
            }

            if (!IsStringValid(m_pTitle.Text)/* || !IsStringValid(m_pDescription.Text)*/ ||
                (m_bShouldUpdateItem && !IsStringValid(m_pPatchNotes.Text) && !string.IsNullOrEmpty(m_pPatchNotes.Text)))
            {
                utils.ShowWarningDialog("Invalid characters detected!", null, true);
                return;
            }

            if (m_pDescription.Text.Length > 1000)
            {
                utils.ShowWarningDialog("The description is more than 1000 characters long!", null, true);
                return;
            }

            if (!string.IsNullOrEmpty(pszContentPath))
            {
                ulong fileSize = utils.GetSizeOfContent(pszContentPath);
                if (fileSize <= 0)
                {
                    utils.ShowWarningDialog("No files selected!", null, true);
                    return;
                }

                if (fileSize > 104857600)
                {
                    utils.ShowWarningDialog("Content size is too big, 100MB is max!", null, true);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(imagePreview))
            {
                ulong fileSize = utils.GetSizeOfFile(pszImagePath);
                if (fileSize <= 0)
                {
                    utils.ShowWarningDialog("No image selected!", null, true);
                    return;
                }

                if (fileSize > 1048576)
                {
                    utils.ShowWarningDialog("Image size is too big, 1MB is max!", null, true);
                    return;
                }
            }

            if (m_bShouldUpdateItem)
                SubmitWorkshopItem(itemUniqueID, m_pPatchNotes.Text);
            else
                UGCHandler.CreateItem();
        }

        private void OnOpenFolderDialog(object sender, EventArgs e)
        {
            DialogResult result = _folderDialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
                pszContentPath = _folderDialog.SelectedPath;
        }

        private void OnOpenImageSelection(object sender, EventArgs e)
        {
            DialogResult result = _fileDialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
                m_bHasChangedImagePath = true;
        }

        private void OnSelectImage(object sender, CancelEventArgs e)
        {
            pszImagePath = _fileDialog.FileName;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Image render = Properties.Resources.unknown;
            if (pszImagePath != null && File.Exists(pszImagePath))
                render = Image.FromFile(pszImagePath);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12, 12, 10)), new Rectangle(0, 0, Width, (int)flStandardHeight));
            e.Graphics.DrawImage(render, new Rectangle(4, (int)flStandardHeight + 4, 300, 150));
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(4, (int)flStandardHeight + 4, 300, 150));

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Near;

        //    e.Graphics.DrawString("Game Modes", new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, new Rectangle(2, Height - 160, 100, 20), format);
        //    e.Graphics.DrawString("Custom Models", new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, new Rectangle(100, Height - 170, 120, 30), format);
        //    e.Graphics.DrawString("Custom Sounds", new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, new Rectangle(200, Height - 170, 120, 30), format);
        //    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(2, Height - 128, 304, 60));

            e.Graphics.DrawString("Contest:", Font, Brushes.White, new Rectangle(310, Height - 50, 160, 15));
            e.Graphics.DrawString(Text, Font, Brushes.White, new Rectangle(0, 2, Width, (int)flStandardHeight));
            e.Graphics.DrawString("Title:", Font, Brushes.White, new Rectangle(310, (int)flStandardHeight + 4, Width - 320, 14));
            e.Graphics.DrawString("Description:", Font, Brushes.White, new Rectangle(310, (int)flStandardHeight + 45, Width - 320, 14));

            if (m_bShouldUpdateItem)
                e.Graphics.DrawString("Changelog:", Font, Brushes.White, new Rectangle(380, (int)flStandardHeight + 255, Width - 390, 16));

            string imagePreview = (m_bHasChangedImagePath ? pszImagePath : null);
            m_pLabelFields[0].Text = imagePreview;
            m_pLabelFields[1].Text = pszContentPath;
        }
    }
}
