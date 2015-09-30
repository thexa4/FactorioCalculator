namespace FactorioCalculator.Forms
{
    partial class RecipeBuilder
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ingredientsView = new System.Windows.Forms.DataGridView();
            this.IngredientNames = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.outputView = new System.Windows.Forms.DataGridView();
            this.OutputNames = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.OutputAmounts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultView = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bestCostLabel = new System.Windows.Forms.Label();
            this.solveButton = new System.Windows.Forms.Button();
            this.expandCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.previewImage = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.solutionImage = new System.Windows.Forms.PictureBox();
            this.itemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ingredientsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewImage)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.solutionImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ingredientsView
            // 
            this.ingredientsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ingredientsView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ingredientsView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.ingredientsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ingredientsView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IngredientNames,
            this.Cost});
            this.ingredientsView.Location = new System.Drawing.Point(3, 3);
            this.ingredientsView.Name = "ingredientsView";
            this.ingredientsView.RowTemplate.Height = 24;
            this.ingredientsView.Size = new System.Drawing.Size(237, 187);
            this.ingredientsView.TabIndex = 0;
            this.ingredientsView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ValidateDouble);
            this.ingredientsView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDataUpdate);
            // 
            // IngredientNames
            // 
            this.IngredientNames.HeaderText = "Ingredient";
            this.IngredientNames.Name = "IngredientNames";
            // 
            // Cost
            // 
            dataGridViewCellStyle1.NullValue = "1";
            this.Cost.DefaultCellStyle = dataGridViewCellStyle1;
            this.Cost.HeaderText = "Usage Cost";
            this.Cost.Name = "Cost";
            // 
            // outputView
            // 
            this.outputView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.outputView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.outputView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outputView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OutputNames,
            this.OutputAmounts});
            this.outputView.Location = new System.Drawing.Point(246, 3);
            this.outputView.Name = "outputView";
            this.outputView.RowTemplate.Height = 24;
            this.outputView.Size = new System.Drawing.Size(237, 187);
            this.outputView.TabIndex = 1;
            this.outputView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ValidateDouble);
            this.outputView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDataUpdate);
            // 
            // OutputNames
            // 
            this.OutputNames.HeaderText = "Output";
            this.OutputNames.Name = "OutputNames";
            // 
            // OutputAmounts
            // 
            dataGridViewCellStyle2.NullValue = "1";
            this.OutputAmounts.DefaultCellStyle = dataGridViewCellStyle2;
            this.OutputAmounts.HeaderText = "Production / second";
            this.OutputAmounts.MaxInputLength = 16;
            this.OutputAmounts.Name = "OutputAmounts";
            // 
            // resultView
            // 
            this.resultView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultView.Location = new System.Drawing.Point(3, 196);
            this.resultView.Multiline = true;
            this.resultView.Name = "resultView";
            this.resultView.Size = new System.Drawing.Size(237, 188);
            this.resultView.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.ingredientsView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.outputView, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.resultView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(730, 387);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bestCostLabel);
            this.groupBox1.Controls.Add(this.solveButton);
            this.groupBox1.Controls.Add(this.expandCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(489, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 187);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // bestCostLabel
            // 
            this.bestCostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bestCostLabel.AutoSize = true;
            this.bestCostLabel.Location = new System.Drawing.Point(7, 121);
            this.bestCostLabel.Name = "bestCostLabel";
            this.bestCostLabel.Size = new System.Drawing.Size(85, 17);
            this.bestCostLabel.TabIndex = 2;
            this.bestCostLabel.Text = "Cost: Infinite";
            // 
            // solveButton
            // 
            this.solveButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.solveButton.Enabled = false;
            this.solveButton.Location = new System.Drawing.Point(7, 144);
            this.solveButton.Name = "solveButton";
            this.solveButton.Size = new System.Drawing.Size(225, 37);
            this.solveButton.TabIndex = 1;
            this.solveButton.Text = "Solve!";
            this.solveButton.UseVisualStyleBackColor = true;
            this.solveButton.Click += new System.EventHandler(this.solveButton_Click);
            // 
            // expandCheckbox
            // 
            this.expandCheckbox.AutoSize = true;
            this.expandCheckbox.Checked = true;
            this.expandCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.expandCheckbox.Location = new System.Drawing.Point(7, 22);
            this.expandCheckbox.Name = "expandCheckbox";
            this.expandCheckbox.Size = new System.Drawing.Size(124, 21);
            this.expandCheckbox.TabIndex = 0;
            this.expandCheckbox.Text = "Expand factory";
            this.expandCheckbox.UseVisualStyleBackColor = true;
            this.expandCheckbox.CheckedChanged += new System.EventHandler(this.CheckboxChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.previewImage);
            this.groupBox2.Location = new System.Drawing.Point(246, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(237, 188);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // previewImage
            // 
            this.previewImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewImage.Location = new System.Drawing.Point(6, 21);
            this.previewImage.Name = "previewImage";
            this.previewImage.Size = new System.Drawing.Size(225, 161);
            this.previewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.previewImage.TabIndex = 6;
            this.previewImage.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.solutionImage);
            this.groupBox3.Location = new System.Drawing.Point(489, 196);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(238, 188);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Best Solution";
            // 
            // solutionImage
            // 
            this.solutionImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.solutionImage.Location = new System.Drawing.Point(10, 21);
            this.solutionImage.Name = "solutionImage";
            this.solutionImage.Size = new System.Drawing.Size(222, 161);
            this.solutionImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.solutionImage.TabIndex = 7;
            this.solutionImage.TabStop = false;
            // 
            // itemBindingSource
            // 
            this.itemBindingSource.DataSource = typeof(FactorioCalculator.Models.Item);
            // 
            // RecipeBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 411);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RecipeBuilder";
            this.Text = "RecipeBuilder";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RecipeBuilder_FormClosed);
            this.Load += new System.EventHandler(this.RecipeBuilder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ingredientsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewImage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.solutionImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource itemBindingSource;
        private System.Windows.Forms.DataGridView ingredientsView;
        private System.Windows.Forms.DataGridView outputView;
        private System.Windows.Forms.DataGridViewComboBoxColumn OutputNames;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutputAmounts;
        private System.Windows.Forms.DataGridViewComboBoxColumn IngredientNames;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cost;
        private System.Windows.Forms.TextBox resultView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox expandCheckbox;
        private System.Windows.Forms.Button solveButton;
        private System.Windows.Forms.Label bestCostLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox previewImage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PictureBox solutionImage;

    }
}