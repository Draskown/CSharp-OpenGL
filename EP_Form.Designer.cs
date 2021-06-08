
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
            this.rbCubes = new System.Windows.Forms.RadioButton();
            this.rbPortal = new System.Windows.Forms.RadioButton();
            this.rbBeams = new System.Windows.Forms.RadioButton();
            this.cbImages = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
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
            this.numMin.Location = new System.Drawing.Point(714, 172);
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
            this.label1.Location = new System.Drawing.Point(751, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Min thresh";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(748, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max thresh";
            // 
            // numMax
            // 
            this.numMax.ForeColor = System.Drawing.Color.White;
            this.numMax.Location = new System.Drawing.Point(714, 108);
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
            // rbCubes
            // 
            this.rbCubes.AutoSize = true;
            this.rbCubes.Checked = true;
            this.rbCubes.Location = new System.Drawing.Point(742, 264);
            this.rbCubes.Name = "rbCubes";
            this.rbCubes.Size = new System.Drawing.Size(64, 19);
            this.rbCubes.TabIndex = 3;
            this.rbCubes.TabStop = true;
            this.rbCubes.Text = "Cubes";
            this.rbCubes.UseVisualStyleBackColor = true;
            // 
            // rbPortal
            // 
            this.rbPortal.AutoSize = true;
            this.rbPortal.Location = new System.Drawing.Point(742, 311);
            this.rbPortal.Name = "rbPortal";
            this.rbPortal.Size = new System.Drawing.Size(60, 19);
            this.rbPortal.TabIndex = 3;
            this.rbPortal.Text = "Portal";
            this.rbPortal.UseVisualStyleBackColor = true;
            // 
            // rbBeams
            // 
            this.rbBeams.AutoSize = true;
            this.rbBeams.Location = new System.Drawing.Point(742, 358);
            this.rbBeams.Name = "rbBeams";
            this.rbBeams.Size = new System.Drawing.Size(66, 19);
            this.rbBeams.TabIndex = 3;
            this.rbBeams.Text = "Beams";
            this.rbBeams.UseVisualStyleBackColor = true;
            // 
            // cbImages
            // 
            this.cbImages.FormattingEnabled = true;
            this.cbImages.Location = new System.Drawing.Point(714, 46);
            this.cbImages.Name = "cbImages";
            this.cbImages.Size = new System.Drawing.Size(140, 23);
            this.cbImages.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(728, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Choose an image";
            // 
            // EP_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 605);
            this.Controls.Add(this.cbImages);
            this.Controls.Add(this.rbBeams);
            this.Controls.Add(this.rbPortal);
            this.Controls.Add(this.rbCubes);
            this.Controls.Add(this.label3);
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
        private System.Windows.Forms.RadioButton rbCubes;
        private System.Windows.Forms.RadioButton rbPortal;
        private System.Windows.Forms.RadioButton rbBeams;
        private System.Windows.Forms.ComboBox cbImages;
        private System.Windows.Forms.Label label3;
    }
}

