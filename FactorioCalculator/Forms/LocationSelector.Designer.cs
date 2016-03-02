namespace FactorioCalculator.Forms
{
    partial class LocationSelector
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
            this.label1 = new System.Windows.Forms.Label();
            this.locationInput = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.electricityCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(501, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "We need to load the Factorio files from disk, please select your install location" +
    ":";
            // 
            // locationInput
            // 
            this.locationInput.Location = new System.Drawing.Point(15, 48);
            this.locationInput.Name = "locationInput";
            this.locationInput.Size = new System.Drawing.Size(474, 22);
            this.locationInput.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(495, 48);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(88, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // continueButton
            // 
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.continueButton.Location = new System.Drawing.Point(495, 98);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(88, 23);
            this.continueButton.TabIndex = 3;
            this.continueButton.Text = "OK";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.statusLabel.Location = new System.Drawing.Point(12, 101);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(48, 17);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Status";
            // 
            // electricityCheckbox
            // 
            this.electricityCheckbox.AutoSize = true;
            this.electricityCheckbox.Location = new System.Drawing.Point(15, 76);
            this.electricityCheckbox.Name = "electricityCheckbox";
            this.electricityCheckbox.Size = new System.Drawing.Size(230, 21);
            this.electricityCheckbox.TabIndex = 5;
            this.electricityCheckbox.Text = "Enable Electricity [experimental]";
            this.electricityCheckbox.UseVisualStyleBackColor = true;
            // 
            // LocationSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 133);
            this.Controls.Add(this.electricityCheckbox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.locationInput);
            this.Controls.Add(this.label1);
            this.Name = "LocationSelector";
            this.Text = "Factorio Location";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LocationSelector_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox locationInput;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.CheckBox electricityCheckbox;
    }
}