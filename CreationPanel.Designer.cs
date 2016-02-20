namespace workshopper
{
    partial class CreationPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectType = new workshopper.controls.ListButton();
            this.SelectType_List = new workshopper.controls.ItemList();
            this.SuspendLayout();
            // 
            // SelectType
            // 
            this.SelectType.BackColor = System.Drawing.Color.Transparent;
            this.SelectType.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SelectType.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectType.ForeColor = System.Drawing.Color.Transparent;
            this.SelectType.LabelTxt = null;
            this.SelectType.Location = new System.Drawing.Point(5, 235);
            this.SelectType.Name = "SelectType";
            this.SelectType.Size = new System.Drawing.Size(155, 20);
            this.SelectType.TabIndex = 23;
            this.SelectType.Click += new System.EventHandler(this.SelectType_Click);
            // 
            // SelectType_List
            // 
            this.SelectType_List.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.SelectType_List.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SelectType_List.ForeColor = System.Drawing.Color.Transparent;
            this.SelectType_List.Location = new System.Drawing.Point(5, 255);
            this.SelectType_List.Name = "SelectType_List";
            this.SelectType_List.Size = new System.Drawing.Size(155, 35);
            this.SelectType_List.TabIndex = 25;
            this.SelectType_List.Visible = false;
            // 
            // CreationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 420);
            this.Controls.Add(this.SelectType);
            this.Controls.Add(this.SelectType_List);
            this.Name = "CreationPanel";
            this.Opacity = 1D;
            this.Text = "Create Addon";
            this.ResumeLayout(false);

        }

        #endregion

        private controls.ListButton SelectType;
        private controls.ItemList SelectType_List;

    }
}