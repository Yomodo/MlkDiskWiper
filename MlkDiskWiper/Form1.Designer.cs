namespace MlkDiskWiper
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.diskPicker = new System.Windows.Forms.ComboBox();
            this.diskPickerLabel = new System.Windows.Forms.Label();
            this.wipe = new System.Windows.Forms.Button();
            this.dataWipeCheck = new System.Windows.Forms.CheckBox();
            this.randomWipeProgress = new System.Windows.Forms.ProgressBar();
            this.cancelWipe = new System.Windows.Forms.Button();
            this.randomWipeType = new System.Windows.Forms.ComboBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // diskPicker
            // 
            this.diskPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.diskPicker.DropDownWidth = 600;
            this.diskPicker.FormattingEnabled = true;
            this.diskPicker.Location = new System.Drawing.Point(115, 36);
            this.diskPicker.Name = "diskPicker";
            this.diskPicker.Size = new System.Drawing.Size(561, 21);
            this.diskPicker.TabIndex = 0;
            // 
            // diskPickerLabel
            // 
            this.diskPickerLabel.AutoSize = true;
            this.diskPickerLabel.Location = new System.Drawing.Point(13, 39);
            this.diskPickerLabel.Name = "diskPickerLabel";
            this.diskPickerLabel.Size = new System.Drawing.Size(70, 13);
            this.diskPickerLabel.TabIndex = 1;
            this.diskPickerLabel.Text = "Choose Disk:";
            // 
            // wipe
            // 
            this.wipe.Location = new System.Drawing.Point(115, 159);
            this.wipe.Name = "wipe";
            this.wipe.Size = new System.Drawing.Size(75, 23);
            this.wipe.TabIndex = 2;
            this.wipe.Text = "Wipe Disk";
            this.wipe.UseVisualStyleBackColor = true;
            this.wipe.Click += new System.EventHandler(this.wipe_Click);
            // 
            // dataWipeCheck
            // 
            this.dataWipeCheck.AutoSize = true;
            this.dataWipeCheck.Checked = true;
            this.dataWipeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dataWipeCheck.Location = new System.Drawing.Point(12, 92);
            this.dataWipeCheck.Name = "dataWipeCheck";
            this.dataWipeCheck.Size = new System.Drawing.Size(77, 17);
            this.dataWipeCheck.TabIndex = 4;
            this.dataWipeCheck.Text = "Data Wipe";
            this.tooltip.SetToolTip(this.dataWipeCheck, "Wipe the data with stuff");
            this.dataWipeCheck.UseVisualStyleBackColor = true;
            // 
            // randomWipeProgress
            // 
            this.randomWipeProgress.Location = new System.Drawing.Point(265, 86);
            this.randomWipeProgress.Name = "randomWipeProgress";
            this.randomWipeProgress.Size = new System.Drawing.Size(411, 23);
            this.randomWipeProgress.TabIndex = 5;
            // 
            // cancelWipe
            // 
            this.cancelWipe.Enabled = false;
            this.cancelWipe.Location = new System.Drawing.Point(197, 159);
            this.cancelWipe.Name = "cancelWipe";
            this.cancelWipe.Size = new System.Drawing.Size(75, 23);
            this.cancelWipe.TabIndex = 6;
            this.cancelWipe.Text = "Cancel";
            this.cancelWipe.UseVisualStyleBackColor = true;
            this.cancelWipe.Click += new System.EventHandler(this.cancelWipe_Click);
            // 
            // randomWipeType
            // 
            this.randomWipeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.randomWipeType.FormattingEnabled = true;
            this.randomWipeType.Location = new System.Drawing.Point(115, 88);
            this.randomWipeType.Name = "randomWipeType";
            this.randomWipeType.Size = new System.Drawing.Size(121, 21);
            this.randomWipeType.TabIndex = 7;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(27, 130);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 203);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.randomWipeType);
            this.Controls.Add(this.cancelWipe);
            this.Controls.Add(this.randomWipeProgress);
            this.Controls.Add(this.dataWipeCheck);
            this.Controls.Add(this.wipe);
            this.Controls.Add(this.diskPickerLabel);
            this.Controls.Add(this.diskPicker);
            this.Name = "Form1";
            this.Text = "MlkDiskWiper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox diskPicker;
        private System.Windows.Forms.Label diskPickerLabel;
        private System.Windows.Forms.Button wipe;
        private System.Windows.Forms.CheckBox dataWipeCheck;
        private System.Windows.Forms.ProgressBar randomWipeProgress;
        private System.Windows.Forms.Button cancelWipe;
        private System.Windows.Forms.ComboBox randomWipeType;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

