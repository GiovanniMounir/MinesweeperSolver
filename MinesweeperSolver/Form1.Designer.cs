namespace MinesweeperSolver
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.autoMouse = new System.Windows.Forms.CheckBox();
            this.refreshFrequency = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.autoRestart = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.refreshFrequency)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(457, 329);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Capture";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            this.button1.MouseLeave += new System.EventHandler(this.RestoreLabel2);
            this.button1.MouseHover += new System.EventHandler(this.Button1_MouseHover);
            // 
            // autoMouse
            // 
            this.autoMouse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.autoMouse.BackColor = System.Drawing.Color.Black;
            this.autoMouse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autoMouse.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.autoMouse.Location = new System.Drawing.Point(438, 306);
            this.autoMouse.Name = "autoMouse";
            this.autoMouse.Size = new System.Drawing.Size(90, 17);
            this.autoMouse.TabIndex = 5;
            this.autoMouse.Text = "Auto-mouse";
            this.autoMouse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoMouse.ThreeState = true;
            this.autoMouse.UseVisualStyleBackColor = false;
            this.autoMouse.MouseLeave += new System.EventHandler(this.RestoreLabel2);
            this.autoMouse.MouseHover += new System.EventHandler(this.AutoMouse_MouseHover);
            // 
            // refreshFrequency
            // 
            this.refreshFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshFrequency.BackColor = System.Drawing.Color.Black;
            this.refreshFrequency.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.refreshFrequency.Location = new System.Drawing.Point(312, 303);
            this.refreshFrequency.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.refreshFrequency.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.refreshFrequency.Name = "refreshFrequency";
            this.refreshFrequency.Size = new System.Drawing.Size(120, 20);
            this.refreshFrequency.TabIndex = 6;
            this.refreshFrequency.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.refreshFrequency.Enter += new System.EventHandler(this.RefreshFrequency_Enter);
            this.refreshFrequency.Leave += new System.EventHandler(this.RestoreLabel2);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(187, 305);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Refresh Frequency (ms)";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Red;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button4.Location = new System.Drawing.Point(11, 30);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(25, 22);
            this.button4.TabIndex = 8;
            this.button4.Text = "X";
            this.button4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.Button4_Click);
            this.button4.MouseLeave += new System.EventHandler(this.RestoreLabel2);
            this.button4.MouseHover += new System.EventHandler(this.Button4_MouseHover);
            // 
            // autoRestart
            // 
            this.autoRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.autoRestart.BackColor = System.Drawing.Color.Black;
            this.autoRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.autoRestart.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.autoRestart.Location = new System.Drawing.Point(438, 283);
            this.autoRestart.Name = "autoRestart";
            this.autoRestart.Size = new System.Drawing.Size(90, 17);
            this.autoRestart.TabIndex = 9;
            this.autoRestart.Text = "Auto-restart";
            this.autoRestart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoRestart.ThreeState = true;
            this.autoRestart.UseVisualStyleBackColor = false;
            this.autoRestart.MouseLeave += new System.EventHandler(this.RestoreLabel2);
            this.autoRestart.MouseHover += new System.EventHandler(this.AutoRestart_MouseHover);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label2.Cursor = System.Windows.Forms.Cursors.No;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(40, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(233, 22);
            this.label2.TabIndex = 11;
            this.label2.Text = "Press F1 to exit auto mouse";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(540, 364);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.autoRestart);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.refreshFrequency);
            this.Controls.Add(this.autoMouse);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Minesweeper Solver";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.White;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.refreshFrequency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox autoMouse;
        private System.Windows.Forms.NumericUpDown refreshFrequency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox autoRestart;
        private System.Windows.Forms.Label label2;
    }
}

