using System.Collections.Generic;
using SharpGL.SceneGraph.Assets;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using SharpGL;
using System;
using GlmNet;
using System.IO;

namespace EP
{
    public partial class EP_Form : Form
    {
        #region Initialization

        private readonly OpenGL gl;

        private readonly Texture[] mainSBTextures;
        private readonly Texture[] otherSBTextures;

        private Bitmap coloredImage, depthMap;

        private readonly List<float[]> points;
        private readonly List<float[]> canny;

        private vec3 cameraPos, cameraFront, cameraUp, direction;

        private Color color;

        private readonly float scale,
                               skyBoxSize, cubeSize,
                               portalSize, cameraSpeed, 
                               beamsSize, beamsHeight;
        private float portalDistance,
                      cubesMoveDelta, cubesRotationDelta,
                      objCentreX, objCentreY, objCentreZ,
                      xOffset, yOffset, yaw, pitch;

        private readonly int pointSize, cubesAmount;
        private int threshMin, threshMax, depth, lastX, lastY;

        private bool currentSB;

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

            pointSize = 2;
            cubesAmount = 6;

            points = new List<float[]>();
            canny = new List<float[]>();

            cameraPos = new vec3(0.0f, 0.0f, 6.0f);
            cameraFront = new vec3(0.0f, 0.0f, -1.0f);
            cameraUp = new vec3(0.0f, 1.0f, 0.0f);

            scale = 100.0f;
            skyBoxSize = 50.0f;
            cubeSize = 0.1f;
            portalSize = 1.0f;
            cameraSpeed = 0.5f;
            portalDistance = 0.0f;
            cubesMoveDelta = cubesRotationDelta = 0.0f;
            xOffset = yOffset = 0.0f; 
            yaw = pitch = 0.0f;
            beamsSize = 0.05f;
            beamsHeight = 0.5f;

            lastX = lastY = 0;

            currentSB = true;

            ReadFiles();

            InitTextures();
        }


        private void EP_Form_Load(object sender, EventArgs e)
        {
            this.frameGL.FrameRate = 60;
            this.frameGL.MouseDown += (ss, ee) => { 
                lastX = ee.Location.X;
                lastY = ee.Location.Y;
            };
            this.frameGL.MouseMove += ChangePosition;
            this.frameGL.OpenGLDraw += Draw;
            this.frameGL.KeyDown += HandleMoving;

            this.cbObjects.SelectedIndexChanged += ChangeObject;
        }


        private void ReadFiles()
        {
            var path = new DirectoryInfo(@"Images\bmps");

            foreach (var file in path.GetFiles("*_Color.bmp"))
                this.cbObjects.Items.Add(file.Name.Split('_')[0]);

            this.cbObjects.SelectedIndex = 0;
            ChangeObject(null, null);
        }


        private void ChangeObject(object sender, EventArgs e)
        {
            var fileName = this.cbObjects.SelectedItem.ToString();
            depthMap = new Bitmap(Image.FromFile($"Images/bmps/{fileName}_Map.bmp"));
            coloredImage = new Bitmap(
                Image.FromFile($"Images/bmps/{fileName}_Color.bmp"),
                new Size(depthMap.Width, depthMap.Height));
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

            points.Clear();
            canny.Clear();

            gl.PointSize(pointSize * 2);
            gl.Begin(OpenGL.GL_POINTS);
            {
                for (int x = 1; x < coloredImage.Width; x += pointSize)
                    for (int y = 1; y < coloredImage.Height; y += pointSize)
                    {
                        var contour = false;
                        color = coloredImage.GetPixel(x, y);
                        depth = (int)depthMap.GetPixel(x, y).R;

                        if (!rbPortal.Checked && (depth >= threshMax || depth <= threshMin))
                            continue;

                        if (!rbPortal.Checked && (depth >= threshMax || depth <= threshMin))
                            continue;

                        if ((Math.Abs(depth - depthMap.GetPixel(x - 1, y).R) > 10 ||
                            Math.Abs(depth - depthMap.GetPixel(x, y - 1).R) > 10) &&
                            depth > 200)
                            contour = true;

                        var p = UVD2XYZ(x, y, depth);

                        p[0] /= scale;
                        p[1] /= scale;
                        p[2] = (scale - p[2]) / scale;

                        if (currentSB && Math.Abs(p[0]) <= portalSize && Math.Abs(p[1]) <= portalSize && p[2] <= portalDistance || 
                            !currentSB && Math.Abs(p[0]) <= portalSize && Math.Abs(p[1]) <= portalSize && p[2] >= portalDistance)
                            continue;

                        if (!currentSB && (Math.Abs(p[0]) >= portalSize || Math.Abs(p[1]) >= portalSize))
                            continue;

                        gl.Color(color.R, color.G, color.B);
                        gl.Vertex(p);
                        points.Add(p);

                        if (contour)
                            canny.Add(p);
                    }
            }
            gl.End();

            if (canny.Count != 0)
            {
                objCentreX = canny.Average(p => p[0]);
                objCentreY = canny.Average(p => p[1]);
                objCentreZ = canny.Average(p => p[2]);
            }

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            if (rbCubes.Checked && currentSB)
            {
                gl.Color(0.7f, 0.0f, 0.7f, 0.7f);
                gl.Translate(objCentreX, objCentreY, objCentreZ);
                gl.Rotate(0.0f, 0.0f, cubesRotationDelta);
                gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
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

            if (rbBeams.Checked && currentSB)
            {
                gl.Color(0.0f, 0.7f, 0.0f, 0.5f);
                gl.Translate(objCentreX, objCentreY, objCentreZ);
                gl.Begin(SharpGL.Enumerations.BeginMode.Quads);
                {
                    gl.Vertex(- beamsSize, 0.0f, - beamsSize);
                    gl.Vertex(+ beamsSize, 0.0f, - beamsSize);
                    gl.Vertex(+ beamsSize, 0.0f, + beamsSize);
                    gl.Vertex(- beamsSize, 0.0f, + beamsSize);

                    gl.Vertex(- beamsSize, 0.0f, - beamsSize);
                    gl.Vertex(- beamsSize, 0.0f, + beamsSize);
                    gl.Vertex(- beamsSize, beamsHeight, + beamsSize);
                    gl.Vertex(- beamsSize, beamsHeight, - beamsSize);

                    gl.Vertex(- beamsSize, 0.0f, - beamsSize);
                    gl.Vertex(+ beamsSize, 0.0f, - beamsSize);
                    gl.Vertex(+ beamsSize, + beamsHeight, - beamsSize);
                    gl.Vertex(- beamsSize, + beamsHeight, - beamsSize);

                    gl.Vertex(+ beamsSize, 0.0f, - beamsSize);
                    gl.Vertex(+ beamsSize, 0.0f, + beamsSize);
                    gl.Vertex(+ beamsSize, + beamsHeight, + beamsSize);
                    gl.Vertex(+ beamsSize, + beamsHeight, - beamsSize);

                    gl.Vertex(- beamsSize, 0.0f, + beamsSize);
                    gl.Vertex(+ beamsSize, 0.0f, + beamsSize);
                    gl.Vertex(+ beamsSize, + beamsHeight, + beamsSize);
                    gl.Vertex(- beamsSize, + beamsHeight, + beamsSize);

                    gl.Vertex(- beamsSize, + beamsHeight, - beamsSize);
                    gl.Vertex(+ beamsSize, + beamsHeight, - beamsSize);
                    gl.Vertex(+ beamsSize, + beamsHeight, + beamsSize);
                    gl.Vertex(- beamsSize, + beamsHeight, + beamsSize);
                }
                gl.End();
            }

            gl.Flush();
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Disable(OpenGL.GL_ALPHA_TEST);

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
                Math.Abs(cameraPos.x) <= portalSize &&
                Math.Abs(cameraPos.y) <= portalSize &&
                cameraPos.z < portalDistance)
                currentSB = false;

            if (rbPortal.Checked &&
                !currentSB &&
                Math.Abs(cameraPos.x) <= portalSize &&
                Math.Abs(cameraPos.y) <= portalSize &&
                cameraPos.z > portalDistance)
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
                    portalDistance = Map(numMin.Value, numMin.Minimum, numMin.Maximum, -1.0f, 5.0f);
                    gl.TexCoord(0.0f, 0.0f); gl.Vertex(- portalSize, + portalSize, portalDistance);
                    gl.TexCoord(1.0f, 0.0f); gl.Vertex(+ portalSize, + portalSize, portalDistance);
                    gl.TexCoord(1.0f, 1.0f); gl.Vertex(+ portalSize, - portalSize, portalDistance);
                    gl.TexCoord(0.0f, 1.0f); gl.Vertex(- portalSize, - portalSize, portalDistance);
                }
                gl.End();
            }
        }

        #endregion



        #region Camera moving

        private void ChangePosition(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

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

            direction = new vec3
            {
                x = glm.cos(glm.radians(yaw)) * glm.cos(glm.radians(pitch)),
                y = glm.sin(glm.radians(pitch)),
                z = glm.sin(glm.radians(yaw)) * glm.cos(glm.radians(pitch))
            };

            cameraFront = glm.normalize(direction);
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
