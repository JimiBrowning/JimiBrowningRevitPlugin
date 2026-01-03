namespace JimiBrowningRevitPlugin
{
    partial class RenumberFabDuctForm
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
            this.prefixLabel = new System.Windows.Forms.Label();
            this.countLabel = new System.Windows.Forms.Label();
            this.prefixTextBox = new System.Windows.Forms.TextBox();
            this.finishRenumberingLabel = new System.Windows.Forms.Label();
            this.countNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.setToNextHighestButton = new System.Windows.Forms.Button();
            this.preventDuplicateCheckBox = new System.Windows.Forms.CheckBox();
            this.startRenumberingButton = new System.Windows.Forms.Button();
            this.lockAllCheckBox = new System.Windows.Forms.CheckBox();
            this.lockCurrentCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.countNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // prefixLabel
            // 
            this.prefixLabel.AutoSize = true;
            this.prefixLabel.Location = new System.Drawing.Point(12, 15);
            this.prefixLabel.Name = "prefixLabel";
            this.prefixLabel.Size = new System.Drawing.Size(33, 13);
            this.prefixLabel.TabIndex = 2;
            this.prefixLabel.Text = "Prefix";
            // 
            // countLabel
            // 
            this.countLabel.AutoSize = true;
            this.countLabel.Location = new System.Drawing.Point(12, 42);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(44, 13);
            this.countLabel.TabIndex = 3;
            this.countLabel.Text = "Number";
            // 
            // prefixTextBox
            // 
            this.prefixTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prefixTextBox.Location = new System.Drawing.Point(62, 12);
            this.prefixTextBox.Name = "prefixTextBox";
            this.prefixTextBox.Size = new System.Drawing.Size(269, 20);
            this.prefixTextBox.TabIndex = 4;
            this.prefixTextBox.TextChanged += new System.EventHandler(this.PrefixTextBoxOnTextChanged);
            // 
            // finishRenumberingLabel
            // 
            this.finishRenumberingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.finishRenumberingLabel.AutoSize = true;
            this.finishRenumberingLabel.Location = new System.Drawing.Point(12, 148);
            this.finishRenumberingLabel.Name = "finishRenumberingLabel";
            this.finishRenumberingLabel.Size = new System.Drawing.Size(284, 13);
            this.finishRenumberingLabel.TabIndex = 6;
            this.finishRenumberingLabel.Text = "Press ESC key to finish renumbering and to enable settings";
            this.finishRenumberingLabel.Visible = false;
            // 
            // countNumericUpDown
            // 
            this.countNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.countNumericUpDown.Location = new System.Drawing.Point(62, 40);
            this.countNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.countNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countNumericUpDown.Name = "countNumericUpDown";
            this.countNumericUpDown.Size = new System.Drawing.Size(137, 20);
            this.countNumericUpDown.TabIndex = 7;
            this.countNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.countNumericUpDown.ValueChanged += new System.EventHandler(this.CountNumericUpDownOnValueChanged);
            // 
            // setToNextHighestButton
            // 
            this.setToNextHighestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.setToNextHighestButton.Location = new System.Drawing.Point(212, 38);
            this.setToNextHighestButton.Name = "setToNextHighestButton";
            this.setToNextHighestButton.Size = new System.Drawing.Size(120, 24);
            this.setToNextHighestButton.TabIndex = 8;
            this.setToNextHighestButton.Text = "Set to next highest";
            this.setToNextHighestButton.UseVisualStyleBackColor = true;
            this.setToNextHighestButton.Click += new System.EventHandler(this.SetToNextHighestButtonOnClick);
            // 
            // preventDuplicateCheckBox
            // 
            this.preventDuplicateCheckBox.AutoSize = true;
            this.preventDuplicateCheckBox.Checked = true;
            this.preventDuplicateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.preventDuplicateCheckBox.Location = new System.Drawing.Point(15, 66);
            this.preventDuplicateCheckBox.Name = "preventDuplicateCheckBox";
            this.preventDuplicateCheckBox.Size = new System.Drawing.Size(177, 17);
            this.preventDuplicateCheckBox.TabIndex = 9;
            this.preventDuplicateCheckBox.Text = "Prevent duplicate Item Numbers";
            this.preventDuplicateCheckBox.UseVisualStyleBackColor = true;
            this.preventDuplicateCheckBox.CheckedChanged += new System.EventHandler(this.PreventDuplicateCheckBoxOnCheckedChanged);
            // 
            // startRenumberingButton
            // 
            this.startRenumberingButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startRenumberingButton.Location = new System.Drawing.Point(12, 164);
            this.startRenumberingButton.Name = "startRenumberingButton";
            this.startRenumberingButton.Size = new System.Drawing.Size(320, 25);
            this.startRenumberingButton.TabIndex = 16;
            this.startRenumberingButton.Text = "Start Reumbering";
            this.startRenumberingButton.UseVisualStyleBackColor = true;
            this.startRenumberingButton.Click += new System.EventHandler(this.StartRenumberingButtonOnClick);
            // 
            // lockAllCheckBox
            // 
            this.lockAllCheckBox.AutoSize = true;
            this.lockAllCheckBox.Checked = true;
            this.lockAllCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lockAllCheckBox.Location = new System.Drawing.Point(15, 112);
            this.lockAllCheckBox.Name = "lockAllCheckBox";
            this.lockAllCheckBox.Size = new System.Drawing.Size(177, 17);
            this.lockAllCheckBox.TabIndex = 15;
            this.lockAllCheckBox.Text = "Lock all duct with Item Numbers";
            this.lockAllCheckBox.UseVisualStyleBackColor = true;
            this.lockAllCheckBox.CheckedChanged += new System.EventHandler(this.LockAllCheckBoxOnCheckedChanged);
            // 
            // lockCurrentCheckBox
            // 
            this.lockCurrentCheckBox.AutoSize = true;
            this.lockCurrentCheckBox.Checked = true;
            this.lockCurrentCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lockCurrentCheckBox.Location = new System.Drawing.Point(15, 89);
            this.lockCurrentCheckBox.Name = "lockCurrentCheckBox";
            this.lockCurrentCheckBox.Size = new System.Drawing.Size(206, 17);
            this.lockCurrentCheckBox.TabIndex = 14;
            this.lockCurrentCheckBox.Text = "Lock duct as Item Number is assigned";
            this.lockCurrentCheckBox.UseVisualStyleBackColor = true;
            this.lockCurrentCheckBox.CheckedChanged += new System.EventHandler(this.LockCurrentCheckBoxOnCheckedChanged);
            // 
            // RenumberFabDuctForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 201);
            this.Controls.Add(this.startRenumberingButton);
            this.Controls.Add(this.lockAllCheckBox);
            this.Controls.Add(this.lockCurrentCheckBox);
            this.Controls.Add(this.preventDuplicateCheckBox);
            this.Controls.Add(this.setToNextHighestButton);
            this.Controls.Add(this.countNumericUpDown);
            this.Controls.Add(this.finishRenumberingLabel);
            this.Controls.Add(this.prefixTextBox);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.prefixLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenumberFabDuctForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Renumber Fab Duct";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RenumberFabDuctFormOnClosed);
            this.Load += new System.EventHandler(this.RenumberFabDuctFormOnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.countNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label prefixLabel;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.TextBox prefixTextBox;
        private System.Windows.Forms.Label finishRenumberingLabel;
        private System.Windows.Forms.NumericUpDown countNumericUpDown;
        private System.Windows.Forms.Button setToNextHighestButton;
        private System.Windows.Forms.CheckBox preventDuplicateCheckBox;
        private System.Windows.Forms.Button startRenumberingButton;
        private System.Windows.Forms.CheckBox lockAllCheckBox;
        private System.Windows.Forms.CheckBox lockCurrentCheckBox;
    }
}