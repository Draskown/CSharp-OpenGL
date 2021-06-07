using System.Collections.Generic;
using SharpGL.SceneGraph.Assets;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using SharpGL;
using System;
using GlmNet;

namespace EP
{
    public partial class EP_Form : Form
    {
        #region Initialization

        private readonly OpenGL gl;

        private readonly Texture[] mainSBTextures;
        private readonly Texture[] otherSBTextures;

        private readonly Bitmap coloredImage, depthMap;

        private readonly List<float[]> points;

        private PointF delta, angle, initPoint;

        private Color color;

        private readonly float angleDelta, scale,
                               skyBoxSize, cubeSize,
                               portalSize;
        private float distance, portalDistance,
                      cubesMoveDelta, cubesRotationDelta,
                      objCentreX, objCentreY, objCentreZ,
                      maxZ;

        private readonly int imageIndex, pointSize,
                             cubesAmount;
        private int threshMin, threshMax, depth;

        private bool currentSB;



        private vec3 cameraPos = new vec3(0.0f, 0.0f, 6.0f);
        private vec3 cameraFront = new vec3(0.0f, 0.0f, -1.0f);
        private vec3 cameraUp = new vec3(0.0f, 1.0f, 0.0f);

        private float cameraSpeed = 0.5f, yaw = 0.0f, pitch = 0.0f,
                    xOffset = 0, yOffset = 0;

        private int lastX = 160,
                    lastY = 120;

        private bool firstMouse = true;

        public EP_Form()
        {
            InitializeComponent();

            gl = this.frameGL.OpenGL;

            mainSBTextures = new Texture[8];
            for (int i = 0; i < mainSBTextures.Length; i++)
                mainSBTextures[i] = new Texture();

            otherSBTextures = new Texture[8];
            for (int i = 0; i < otherSBTextures.Length; i++)
                otherSBTextures[i] = new Texture();

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
            portalSize = 1.0f;
            portalDistance = 0.0f;
            cubesMoveDelta = cubesRotationDelta = 0.0f;
            maxZ = 0.0f;

            currentSB = true;

            InitTextures();
        }


        private void EP_Form_Load(object sender, EventArgs e)
        {
            this.frameGL.FrameRate = 60;
            this.frameGL.MouseDown += (ss, ee) => initPoint = ee.Location;
            this.frameGL.MouseWheel += ChangeDistance;
            this.frameGL.MouseMove += ChangePosition;
            this.frameGL.OpenGLDraw += Draw;
            this.frameGL.KeyDown += HandleMoving;
        }


        private void InitTextures()
        {
            mainSBTextures[0].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/negx.jpg"), new Size(512, 512)));
            mainSBTextures[1].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/posx.jpg"), new Size(512, 512)));
            mainSBTextures[2].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/negy.jpg"), new Size(512, 512)));
            mainSBTextures[3].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/negz.jpg"), new Size(512, 512)));
            mainSBTextures[4].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/negx.jpg"), new Size(512, 512)));
            mainSBTextures[5].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/posy.jpg"), new Size(512, 512)));
            mainSBTextures[6].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/posz.jpg"), new Size(512, 512)));
            mainSBTextures[7].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/MainSkybox/portal.jpg"), new Size(512, 512)));

            otherSBTextures[0].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/negx.jpg"), new Size(512, 512)));
            otherSBTextures[1].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/posx.jpg"), new Size(512, 512)));
            otherSBTextures[2].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/negy.jpg"), new Size(512, 512)));
            otherSBTextures[3].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/negz.jpg"), new Size(512, 512)));
            otherSBTextures[4].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/negx.jpg"), new Size(512, 512)));
            otherSBTextures[5].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/posy.jpg"), new Size(512, 512)));
            otherSBTextures[6].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/posz.jpg"), new Size(512, 512)));
            otherSBTextures[7].Create(gl, new Bitmap(Image.FromFile($"Images/Textures/OtherSkybox/portal.jpg"), new Size(512, 512)));
        }

        #endregion



        #region Drawing

        private void Draw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            threshMin = (int)numMin.Value;
            threshMax = (int)numMax.Value;

            //gl.Translate(delta.X, delta.Y, -distance);
            //gl.Rotate(angle.Y, angle.X, 0.0f);

            gl.LookAt(cameraPos.x, cameraPos.y, cameraPos.z,
                      (cameraPos + cameraFront).x, (cameraPos + cameraFront).y, (cameraPos + cameraFront).z,
                      cameraUp.x, cameraUp.y, cameraUp.z);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_ALPHA_TEST);

            gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            CheckSB();

            if (currentSB)
                BindTextures(mainSBTextures);
            else
                BindTextures(otherSBTextures);

            gl.Disable(OpenGL.GL_TEXTURE_2D);

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

                        if (currentSB && depth <= threshMin || depth >= threshMax)
                            continue;

                        if (!currentSB && depth >= threshMin || depth >= threshMax)
                            continue;

                        var p = UVD2XYZ(x, y, depth);

                        p[0] /= scale;
                        p[1] /= scale;
                        p[2] = (scale - p[2]) / scale;

                        gl.Color(color.R, color.G, color.B);
                        gl.Vertex(p);
                        points.Add(p);
                    }
            }
            gl.End();

            objCentreX = points.Average(p => p[0]);
            objCentreY = points.Average(p => p[1]);
            objCentreZ = points.Average(p => p[2]);
            maxZ = points.Max(p => p[2]);

            if (rbCubes.Checked)
            {
                gl.Color(1.0f, 0.0f, 1.0f, 1.0f);
                gl.Translate(objCentreX, objCentreY, objCentreZ + 0.5f);
                gl.Rotate(0.0f, 0.0f, cubesRotationDelta);
                gl.Begin(OpenGL.GL_QUADS);
                {
                    for (int k = 0; k < cubesAmount; k++)
                    {
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
            }

            gl.Flush();

            cubesMoveDelta += 0.0125f;
            cubesRotationDelta += 2.0f;
        }

        #endregion



        #region Values conversion

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

        #endregion


        #region Checking in which dimension the camera currently is

        private void CheckSB()
        {
            if (rbPortal.Checked &&
                currentSB &&
                Math.Abs(delta.X - objCentreX) < portalSize * 2 &&
                Math.Abs(delta.Y - objCentreY) < portalSize * 2 &&
                distance < portalDistance &&
                (angle.X % 360.0f >= 0.0f &&
                angle.X % 360.0f < 90.0f || angle.X % 360.0f > 270.0f ||
                angle.X % 360.0f < 0.0f &&
                angle.X % 360.0f > -90.0f || angle.X % 360.0f < -270.0f) &&
                (angle.Y % 360.0f >= 0.0f &&
                angle.Y % 360.0f < 90.0f || angle.Y % 360.0f > 270.0f ||
                angle.Y % 360.0f < 0.0f &&
                angle.Y % 360.0f > -90.0f || angle.Y % 360.0f < -270.0f))
                currentSB = false;

            if (rbPortal.Checked &&
                !currentSB &&
                Math.Abs(delta.X - objCentreX) < portalSize * 2 &&
                Math.Abs(delta.Y - objCentreY) < portalSize * 2 &&
                distance < portalDistance &&
                (angle.X % 360.0f >= 180.0f &&
                angle.X % 360.0f <= 270.0f || angle.X % 360.0f > 90.0f ||
                angle.X % 360.0f < 180.0f &&
                angle.X % 360.0f >= 90.0f || angle.X % 360.0f < -90.0f) &&
                (angle.Y % 360.0f >= 0.0f &&
                angle.Y % 360.0f < 90.0f || angle.Y % 360.0f > 270.0f ||
                angle.Y % 360.0f < 0.0f &&
                angle.Y % 360.0f > -90.0f || angle.Y % 360.0f < -270.0f))
                currentSB = true;
        }

        #endregion



        #region Texture binding

        private void BindTextures(Texture[] texarr)
        {
            if (points.Count == 0)
                return;

            texarr[0].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
            }
            gl.End();

            texarr[1].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
            }
            gl.End();

            texarr[2].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, -skyBoxSize);
            }
            gl.End();

            texarr[3].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, -skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, -skyBoxSize);
            }
            gl.End();

            texarr[4].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, -skyBoxSize);
            }
            gl.End();

            texarr[5].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, -skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, skyBoxSize, skyBoxSize);
            }
            gl.End();

            texarr[6].Bind(gl);
            gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(-skyBoxSize, skyBoxSize, skyBoxSize);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(-skyBoxSize, -skyBoxSize, skyBoxSize);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(skyBoxSize, -skyBoxSize, skyBoxSize);
            }
            gl.End();

            if (rbPortal.Checked)
            {
                texarr[7].Bind(gl);
                gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    portalDistance = Map(numMin.Value, numMin.Minimum, numMin.Maximum, 0.0f, maxZ);
                    gl.TexCoord(0.0f, 0.0f); gl.Vertex(objCentreX - portalSize, objCentreY + portalSize, portalDistance);
                    gl.TexCoord(1.0f, 0.0f); gl.Vertex(objCentreX + portalSize, objCentreY + portalSize, portalDistance);
                    gl.TexCoord(1.0f, 1.0f); gl.Vertex(objCentreX + portalSize, objCentreY - portalSize, portalDistance);
                    gl.TexCoord(0.0f, 1.0f); gl.Vertex(objCentreX - portalSize, objCentreY - portalSize, portalDistance);
                }
                gl.End();
            }
        }

        #endregion



        #region Mouse moving

        private void ChangePosition(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    PointF finalPoint = e.Location;

            //    if (ModifierKeys == Keys.Shift)
            //    {
            //        angle.X += (finalPoint.X - initPoint.X) * angleDelta;
            //        angle.Y += (finalPoint.Y - initPoint.Y) * angleDelta;
            //    }
            //    else
            //    {
            //        delta.X += (finalPoint.X - initPoint.X) / 100;
            //        delta.Y -= (finalPoint.Y - initPoint.Y) / 100;
            //    }

            //    initPoint = finalPoint;

            //    //Console.WriteLine(delta + distance.ToString() + "\n\r");
            //    //Console.WriteLine($"{{ {angle.X % 360}, {angle.Y % 360} }} \n\r");
            //}

            if (firstMouse)
            {
                lastX = e.Location.X;
                lastY = e.Location.Y;
                firstMouse = false;
            }

            xOffset += e.Location.X - lastX;
            yOffset += e.Location.Y - lastY;
            lastX = e.Location.X;
            lastY = e.Location.Y;

            float sensitivity = 0.2f;
            xOffset *= sensitivity;
            yOffset *= sensitivity;

            yaw += xOffset;
            pitch -= yOffset;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            vec3 direction = new vec3();
            direction.x = glm.cos(glm.radians(yaw)) * glm.cos(glm.radians(pitch));
            direction.y = glm.sin(glm.radians(pitch));
            direction.z = glm.sin(glm.radians(yaw)) * glm.cos(glm.radians(pitch));
            cameraFront = glm.normalize(direction);
        }


        private void ChangeDistance(object sender, MouseEventArgs e)
        {
            distance -= e.Delta / 120;
        }



        private void HandleMoving(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                cameraPos += cameraSpeed * cameraFront;
            if (e.KeyCode == Keys.S)
                cameraPos -= cameraSpeed * cameraFront;
            if (e.KeyCode == Keys.A)
                cameraPos -= glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
            if (e.KeyCode == Keys.D)
                cameraPos += glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
        }


        #endregion



        #region Mapping

        private float Map(decimal value, decimal inMin, decimal inMax, float outMin, float outMax)
        {
            return ((float)(value - inMin) * (outMax - outMin) / (float)(inMax - inMin)) + outMin;
        }

        #endregion
    }
}
