
namespace EP
{
    partial class EP_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EP_Form));
            this.frameGL = new SharpGL.OpenGLControl();
            this.numMin = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numMax = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.frameGL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).BeginInit();
            this.SuspendLayout();
            // 
            // frameGL
            // 
            this.frameGL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frameGL.DrawFPS = false;
            this.frameGL.Location = new System.Drawing.Point(16, 16);
            this.frameGL.Name = "frameGL";
            this.frameGL.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.frameGL.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.frameGL.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.frameGL.Size = new System.Drawing.Size(680, 665);
            this.frameGL.TabIndex = 0;
            // 
            // numMin
            // 
            this.numMin.ForeColor = System.Drawing.Color.White;
            this.numMin.Location = new System.Drawing.Point(712, 105);
            this.numMin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numMin.Name = "numMin";
            this.numMin.Size = new System.Drawing.Size(140, 23);
            this.numMin.TabIndex = 1;
            this.numMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(736, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Min thresh";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(736, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max thresh";
            // 
            // numMax
            // 
            this.numMax.ForeColor = System.Drawing.Color.White;
            this.numMax.Location = new System.Drawing.Point(712, 41);
            this.numMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numMax.Name = "numMax";
            this.numMax.Size = new System.Drawing.Size(140, 23);
            this.numMax.TabIndex = 1;
            this.numMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numMax.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // EP_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 605);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numMax);
            this.Controls.Add(this.numMin);
            this.Controls.Add(this.frameGL);
            this.Font = new System.Drawing.Font("Gilroy", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EP_Form";
            this.Text = "Application";
            this.Load += new System.EventHandler(this.EP_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.frameGL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpGL.OpenGLControl frameGL;
        private System.Windows.Forms.NumericUpDown numMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMax;
    }
}

