using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EP
{
    public partial class EP_Form : Form
    {
        #region Initialization

        private readonly OpenGL gl;

        private readonly Texture[] textures;

        private readonly Bitmap coloredImage, depthMap;

        private readonly List<float[]> points;

        private PointF delta, angle, initPoint;

        private Color color;

        private readonly float angleDelta, scale,
                               skyBoxSize, cubeSize;
        private float distance, cubesMoveDelta, 
                      cubesRotationDelta;

        private readonly int imageIndex, pointSize,
                             cubesAmount;
        private int threshMin, threshMax, depth;

        public EP_Form()
        {
            InitializeComponent();

            gl = this.frameGL.OpenGL;

            textures = new Texture[7];
            for (int i = 0; i < textures.Length; i++)
                textures[i] = new Texture();

            imageIndex = 7;
            pointSize = 2;
            cubesAmount = 6;

            depthMap = new Bitmap(Image.FromFile($"Images/bmps/{imageIndex}_Map.bmp"));
            coloredImage = new Bitmap(
                Image.FromFile($"Images/bmps/{imageIndex}_Color.bmp"),
                new Size(depthMap.Width, depthMap.Height));

            points = new List<float[]>();

            delta = new PointF(0.0f, 0.0f);
            angle = new PointF(0.0f, 0.0f);

            angleDelta = 0.02f;
            distance = 6.0f;
            scale = 100.0f;
            skyBoxSize = 50.0f;
            cubeSize = 0.1f;
            cubesMoveDelta = cubesRotationDelta = 0.0f;

            InitTextures();
        }


        private void EP_Form_Load(object sender, EventArgs e)
        {
            this.frameGL.FrameRate = 60;
            this.frameGL.MouseDown += (ss, ee) => initPoint = ee.Location;
            this.frameGL.MouseWheel += ChangeDistance;
            this.frameGL.MouseMove += ChangePosition;
            this.frameGL.OpenGLDraw += Draw;
        }

        #endregion


        private void InitTextures()
        {
            textures[0].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/negx.jpg"), new Size(512, 512)));
            textures[1].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/posx.jpg"), new Size(512, 512)));
            textures[2].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/negy.jpg"), new Size(512, 512)));
            textures[3].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/negz.jpg"), new Size(512, 512)));
            textures[4].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/negx.jpg"), new Size(512, 512)));
            textures[5].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/posy.jpg"), new Size(512, 512)));
            textures[6].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/posz.jpg"), new Size(512, 512)));
        }


        #region Drawing

        private void Draw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            threshMin = (int)numMin.Value;
            threshMax = (int)numMax.Value;

            gl.Translate(delta.X, delta.Y, -distance);
            gl.Rotate(angle.Y, angle.X, 0.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_ALPHA_TEST);

            gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            BindTextures();
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Translate(0.0f, 0.0f, -100.0f);
            gl.PointSize(pointSize);
            gl.Begin(OpenGL.GL_POINTS);
            {
                gl.Color(1.0f, 1.0f, 1.0f);
                gl.Vertex(0.0f, 0.0f, 0.0f);

                for (int x = 0; x < coloredImage.Width; x += pointSize)
                    for (int y = 0; y < coloredImage.Height; y += pointSize)
                    {
                        color = coloredImage.GetPixel(x, y);
                        depth = (int)depthMap.GetPixel(x, y).R;

                        if (depth <= threshMin || depth >= threshMax)
                            continue;

                        var p = UVD2XYZ(x, y, depth);

                        gl.Color(color.R, color.G, color.B);
                        gl.Vertex(p[0] / scale, p[1] / scale, scale - p[2] / scale);
                        points.Add(p);
                    }
            }
            gl.End();

            var centreX = points.Average(p => p[0]) / scale;
            var centreY = points.Average(p => p[1]) / scale;
            var centreZ = scale - points.Average(p => p[2]) / scale;

            gl.Color(1.0f, 0.0f, 1.0f, 1.0f);
            gl.Translate(centreX, centreY, centreZ + 0.5f);
            gl.Rotate(0.0f, 0.0f, cubesRotationDelta);
            gl.Begin(OpenGL.GL_QUADS);
            {
                for (int k = 0; k < cubesAmount; k++)
                {
                    gl.Rotate(1.0f, 1.0f, 20.0f);
                    var xOffset = 0.7f * Math.Sin(360 / cubesAmount * k * Math.PI / 180 + cubesMoveDelta);
                    var yOffset = 0.7f * Math.Cos(360 / cubesAmount * k * Math.PI / 180 + cubesMoveDelta);

                    gl.Vertex(xOffset - cubeSize, yOffset - cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset - cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset - cubeSize, cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset - cubeSize, cubeSize);

                    gl.Vertex(xOffset - cubeSize, yOffset + cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset + cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset + cubeSize, cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset + cubeSize, cubeSize);

                    gl.Vertex(xOffset - cubeSize, yOffset - cubeSize, -cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset - cubeSize, cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset + cubeSize, cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset + cubeSize, -cubeSize);

                    gl.Vertex(xOffset + cubeSize, yOffset - cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset - cubeSize, cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset + cubeSize, cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset + cubeSize, -cubeSize);

                    gl.Vertex(xOffset - cubeSize, yOffset - cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset - cubeSize, -cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset + cubeSize, -cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset + cubeSize, -cubeSize);

                    gl.Vertex(xOffset - cubeSize, yOffset - cubeSize, cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset - cubeSize, cubeSize);
                    gl.Vertex(xOffset + cubeSize, yOffset + cubeSize, cubeSize);
                    gl.Vertex(xOffset - cubeSize, yOffset + cubeSize, cubeSize);
                }
            }
            gl.End();

            gl.Flush();

            cubesMoveDelta += 0.0125f;
            cubesRotationDelta += 2.0f;
        }

        #endregion



        private float[] UVD2XYZ(float u, float v, float d)
        {
            var xyz = new float[3];

            var cx = 1.5f;
            var cy = 2.4273913761751615e+02f;
            var fx = 1.0f / 5.9421434211923247e+02f;
            var fy = 1.0f / 5.9104053696870778e+02f;

            xyz[0] = (u - cx) * d * fx;
            xyz[1] = (v - cy) * d * fy;
            xyz[2] = d;

            xyz[1] *= (float)Math.Cos(Math.PI);
            xyz[2] *= (float)Math.Cos(Math.PI);

            return xyz;
        }


        private void BindTextures()
        {
            textures[0].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
            }
            gl.End();

            textures[1].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
            }
            gl.End();

            textures[2].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, -skyBoxSize);
            }
            gl.End();

            textures[3].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
            }
            gl.End();

            textures[4].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, -skyBoxSize);
            }
            gl.End();

            textures[5].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, skyBoxSize, skyBoxSize);
            }
            gl.End();

            textures[6].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, skyBoxSize);
            }
            gl.End();
        }



        #region Mouse moving

        private void ChangePosition(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF finalPoint = e.Location;

                if (ModifierKeys == Keys.Shift)
                {
                    angle.X += (finalPoint.X - initPoint.X) * angleDelta;
                    angle.Y += (finalPoint.Y - initPoint.Y) * angleDelta;
                }
                else
                {
                    delta.X += (finalPoint.X - initPoint.X) / 100;
                    delta.Y -= (finalPoint.Y - initPoint.Y) / 100;
                }

                initPoint = finalPoint;
            }
        }


        private void ChangeDistance(object sender, MouseEventArgs e)
        {
            distance -= e.Delta / 120;
        }

        #endregion
    }
}
