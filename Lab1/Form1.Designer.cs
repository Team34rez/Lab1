namespace Lab1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label quantityLabel;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.Label itemsLabel;

        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox quantityTextBox;
        private System.Windows.Forms.TextBox priceTextBox;
        private System.Windows.Forms.TextBox categoryTextBox;

        private System.Windows.Forms.ListBox itemsListBox;
        private System.Windows.Forms.Button addItemButton;
        private System.Windows.Forms.Button updateItemButton;
        private System.Windows.Forms.Button removeItemButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            this.nameLabel = new System.Windows.Forms.Label();
            this.quantityLabel = new System.Windows.Forms.Label();
            this.priceLabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.itemsLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.quantityTextBox = new System.Windows.Forms.TextBox();
            this.priceTextBox = new System.Windows.Forms.TextBox();
            this.categoryTextBox = new System.Windows.Forms.TextBox();
            this.itemsListBox = new System.Windows.Forms.ListBox();
            this.addItemButton = new System.Windows.Forms.Button();
            this.updateItemButton = new System.Windows.Forms.Button();
            this.removeItemButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.stockThresholdInput = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.stockThresholdInput)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(20, 20);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(123, 16);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Название товара";
            // 
            // quantityLabel
            // 
            this.quantityLabel.AutoSize = true;
            this.quantityLabel.Location = new System.Drawing.Point(20, 84);
            this.quantityLabel.Name = "quantityLabel";
            this.quantityLabel.Size = new System.Drawing.Size(110, 16);
            this.quantityLabel.TabIndex = 1;
            this.quantityLabel.Text = "Количество, шт.";
            // 
            // priceLabel
            // 
            this.priceLabel.AutoSize = true;
            this.priceLabel.Location = new System.Drawing.Point(20, 148);
            this.priceLabel.Name = "priceLabel";
            this.priceLabel.Size = new System.Drawing.Size(151, 16);
            this.priceLabel.TabIndex = 2;
            this.priceLabel.Text = "Цена за единицу, руб.";
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Location = new System.Drawing.Point(20, 212);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(75, 16);
            this.categoryLabel.TabIndex = 3;
            this.categoryLabel.Text = "Категория";
            // 
            // itemsLabel
            // 
            this.itemsLabel.AutoSize = true;
            this.itemsLabel.Location = new System.Drawing.Point(265, 20);
            this.itemsLabel.Name = "itemsLabel";
            this.itemsLabel.Size = new System.Drawing.Size(112, 16);
            this.itemsLabel.TabIndex = 4;
            this.itemsLabel.Text = "Список товаров";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(20, 44);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(220, 22);
            this.nameTextBox.TabIndex = 0;
            // 
            // quantityTextBox
            // 
            this.quantityTextBox.Location = new System.Drawing.Point(20, 108);
            this.quantityTextBox.Name = "quantityTextBox";
            this.quantityTextBox.Size = new System.Drawing.Size(220, 22);
            this.quantityTextBox.TabIndex = 1;
            // 
            // priceTextBox
            // 
            this.priceTextBox.Location = new System.Drawing.Point(20, 172);
            this.priceTextBox.Name = "priceTextBox";
            this.priceTextBox.Size = new System.Drawing.Size(220, 22);
            this.priceTextBox.TabIndex = 2;
            // 
            // categoryTextBox
            // 
            this.categoryTextBox.Location = new System.Drawing.Point(20, 236);
            this.categoryTextBox.Name = "categoryTextBox";
            this.categoryTextBox.Size = new System.Drawing.Size(220, 22);
            this.categoryTextBox.TabIndex = 3;
            // 
            // itemsListBox
            // 
            this.itemsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsListBox.HorizontalScrollbar = true;
            this.itemsListBox.IntegralHeight = false;
            this.itemsListBox.ItemHeight = 16;
            this.itemsListBox.Location = new System.Drawing.Point(265, 44);
            this.itemsListBox.Name = "itemsListBox";
            this.itemsListBox.Size = new System.Drawing.Size(515, 376);
            this.itemsListBox.TabIndex = 5;
            this.itemsListBox.SelectedIndexChanged += new System.EventHandler(this.itemsListBox_SelectedIndexChanged);
            // 
            // addItemButton
            // 
            this.addItemButton.Location = new System.Drawing.Point(20, 284);
            this.addItemButton.Name = "addItemButton";
            this.addItemButton.Size = new System.Drawing.Size(220, 36);
            this.addItemButton.TabIndex = 4;
            this.addItemButton.Text = "Добавить";
            this.addItemButton.UseVisualStyleBackColor = true;
            this.addItemButton.Click += new System.EventHandler(this.addItemButton_Click);
            // 
            // updateItemButton
            // 
            this.updateItemButton.Location = new System.Drawing.Point(20, 334);
            this.updateItemButton.Name = "updateItemButton";
            this.updateItemButton.Size = new System.Drawing.Size(220, 36);
            this.updateItemButton.TabIndex = 6;
            this.updateItemButton.Text = "Сохранить изменения";
            this.updateItemButton.UseVisualStyleBackColor = true;
            this.updateItemButton.Click += new System.EventHandler(this.updateItemButton_Click);
            // 
            // removeItemButton
            // 
            this.removeItemButton.Location = new System.Drawing.Point(20, 384);
            this.removeItemButton.Name = "removeItemButton";
            this.removeItemButton.Size = new System.Drawing.Size(220, 36);
            this.removeItemButton.TabIndex = 7;
            this.removeItemButton.Text = "Удалить";
            this.removeItemButton.UseVisualStyleBackColor = true;
            this.removeItemButton.Click += new System.EventHandler(this.removeItemButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(265, 427);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Порог запаса (0 — выкл.)";
            // 
            // stockThresholdInput
            // 
            this.stockThresholdInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stockThresholdInput.Location = new System.Drawing.Point(520, 424);
            this.stockThresholdInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.stockThresholdInput.Name = "stockThresholdInput";
            this.stockThresholdInput.Size = new System.Drawing.Size(120, 22);
            this.stockThresholdInput.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 453);
            this.Controls.Add(this.stockThresholdInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.quantityLabel);
            this.Controls.Add(this.quantityTextBox);
            this.Controls.Add(this.priceLabel);
            this.Controls.Add(this.priceTextBox);
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.categoryTextBox);
            this.Controls.Add(this.itemsLabel);
            this.Controls.Add(this.itemsListBox);
            this.Controls.Add(this.addItemButton);
            this.Controls.Add(this.updateItemButton);
            this.Controls.Add(this.removeItemButton);
            this.MinimumSize = new System.Drawing.Size(680, 500);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Управление инвентарём";
            ((System.ComponentModel.ISupportInitialize)(this.stockThresholdInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown stockThresholdInput;
    }
}

