namespace FactorioCalculator.Forms
{
    partial class TreeViewer
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
            this.ItemsSelector = new System.Windows.Forms.ComboBox();
            this.RecipeTreeView = new System.Windows.Forms.TreeView();
            this.TotalsDisplay = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ItemsSelector
            // 
            this.ItemsSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsSelector.FormattingEnabled = true;
            this.ItemsSelector.Location = new System.Drawing.Point(12, 12);
            this.ItemsSelector.Name = "ItemsSelector";
            this.ItemsSelector.Size = new System.Drawing.Size(404, 21);
            this.ItemsSelector.TabIndex = 0;
            this.ItemsSelector.SelectedIndexChanged += new System.EventHandler(this.ItemsSelector_SelectedItemChanged);
            // 
            // RecipeTreeView
            // 
            this.RecipeTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecipeTreeView.Location = new System.Drawing.Point(12, 39);
            this.RecipeTreeView.Name = "RecipeTreeView";
            this.RecipeTreeView.Size = new System.Drawing.Size(404, 311);
            this.RecipeTreeView.TabIndex = 1;
            // 
            // TotalsDisplay
            // 
            this.TotalsDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TotalsDisplay.FormattingEnabled = true;
            this.TotalsDisplay.Location = new System.Drawing.Point(12, 356);
            this.TotalsDisplay.Name = "TotalsDisplay";
            this.TotalsDisplay.ScrollAlwaysVisible = true;
            this.TotalsDisplay.Size = new System.Drawing.Size(404, 69);
            this.TotalsDisplay.TabIndex = 2;
            // 
            // TreeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 440);
            this.Controls.Add(this.TotalsDisplay);
            this.Controls.Add(this.RecipeTreeView);
            this.Controls.Add(this.ItemsSelector);
            this.Name = "TreeViewer";
            this.Text = "TreeViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TreeViewer_FormClosing);
            this.Load += new System.EventHandler(this.TreeViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ItemsSelector;
        private System.Windows.Forms.TreeView RecipeTreeView;
        private System.Windows.Forms.ListBox TotalsDisplay;
    }
}