using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using MomentShadowMappingTest.Primitives;
using MomentShadowMappingTest.ShaderProgramm;
using MomentShadowMappingTest.Textures;
using MomentShadowMappingTest.GameCore;

namespace MomentShadowMappingTest
{
    class ApplicationWindow : GameWindow
    {
        private GameWorld _currentWorld = new GameWorld();

        private Matrix4 _projectionMatrix = Matrix4.Identity;   // Gleicht das Bildschirmverhältnis (z.B. 16:9) aus
        private Matrix4 _viewMatrix = Matrix4.Identity;         // Simuliert eine Kamera

        private Matrix4 _viewMatrixShadowMap = Matrix4.Identity;
        private Matrix4 _projectionMatrixShadowMap = Matrix4.Identity;
        private Matrix4 _viewProjectionMatrixShadowMap = Matrix4.Identity;

        private int _fbShadowMap = -1;
        private int _fbShadowMapTexture = -1;
        private int _fbShadowMapDownsampled = -1;
        private int _fbShadowMapTextureDownsampled = -1;
        private int _fbShadowMapColor = -1; 
        private int _fbShadowMapColorTexture = -1;

        public ApplicationWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) 
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Basis-OpenGL-Aktionen (wie z.B. grundlegende Einstellungen) ausführen!
            GL.ClearColor(0, 0, 0, 1); // Farbe des gelöschten Bildschirms wählen

            GL.Enable(EnableCap.DepthTest); // Tiefenpuffer aktivieren

            GL.Enable(EnableCap.CullFace); // Zeichnen von verdeckten Teilen eines Objekts verhindern
            GL.CullFace(CullFaceMode.Back); // Der Kamera abgewandte Flächen werden ignoriert

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //PrimitiveTriangle.Init();
            PrimitiveQuad.Init();
            PrimitiveCube.Init();
            ShaderStandard.Init();
            ShaderShadow.Init();
            ShaderMoments.Init();
            InitializeShadowMap();

            _viewMatrix = Matrix4.LookAt(0, 25, 25, 0, 0, 0, 0, 1, 0);

            GameObject g1 = new GameObject();
            g1.Position = new Vector3(0, 2.5f, 0);
            g1.SetScale(5, 5, 5);
            g1.SetTexture("MomentShadowMappingTest.Textures.crate.jpg");
            _currentWorld.AddGameObject(g1);

            
            GameObject g2 = new GameObject();
            g2.Position = new Vector3(0, -0.5f, 0);
            g2.SetScale(50, 1, 50);
            g2.SetTexture("MomentShadowMappingTest.Textures.metalgrid1_diffuse.png");
            _currentWorld.AddGameObject(g2);
            

            _currentWorld.SetSunPosition(25, 25, 25);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            // Anpassung der 3D-Instanzen an die neue Fenstergröße

             GL.Viewport(0, 0, Size.X, Size.Y);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, (float)Size.X / Size.Y, 0.1f, 1000f);
        }


        private void InitializeShadowMap()
        {
            uint filterNearest = (uint)TextureMagFilter.Nearest;
            uint filterLinear = (uint)TextureMagFilter.Linear;
            uint clampToEdge = (uint)TextureWrapMode.ClampToEdge;

            GL.CreateFramebuffers(1, out _fbShadowMap);
            GL.CreateTextures(TextureTarget.Texture2DMultisample, 1, out _fbShadowMapTexture);
            GL.CreateRenderbuffers(1, out int rbShadowMap);

            GL.BindTexture(TextureTarget.Texture2DMultisample, _fbShadowMapTexture);
            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 4, PixelInternalFormat.R32f, 1024, 1024, true);
            GL.TexParameterI(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, ref filterNearest);
            GL.TexParameterI(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, ref filterNearest);
            GL.TexParameterI(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapS, ref clampToEdge);
            GL.TexParameterI(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapT, ref clampToEdge);
            GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureBorderColor, 1f);
            GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbShadowMap);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _fbShadowMapTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbShadowMap);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 4, RenderbufferStorage.DepthComponent32f, 1024, 1024);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbShadowMap);
            
            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if(status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer invalid.");
            }

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // ===================================================================

            GL.CreateFramebuffers(1, out _fbShadowMapColor);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _fbShadowMapColorTexture);

            GL.BindTexture(TextureTarget.Texture2D, _fbShadowMapColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1024, 1024, 0, PixelFormat.Rgba, PixelType.UnsignedShort, IntPtr.Zero);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref filterNearest);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref filterNearest);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clampToEdge);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbShadowMapColor);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _fbShadowMapColorTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer invalid.");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // ===============================================
            
            GL.CreateFramebuffers(1, out _fbShadowMapDownsampled);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _fbShadowMapTextureDownsampled);

            GL.BindTexture(TextureTarget.Texture2D, _fbShadowMapTextureDownsampled);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, 1024, 1024, 0, PixelFormat.Red, PixelType.Float, IntPtr.Zero);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref filterLinear);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref filterLinear);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clampToEdge);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, 1f);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbShadowMapDownsampled);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _fbShadowMapTextureDownsampled, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.ReadBuffer(ReadBufferMode.None);

            status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer invalid.");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            

            // ===============================================

            _projectionMatrixShadowMap = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1f, 1f, 100f);
        }

        private void RenderShadowMap()
        {
            GL.Viewport(0, 0, 1024, 1024);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbShadowMap);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _viewMatrixShadowMap = Matrix4.LookAt(_currentWorld.GetSunPosition(), new Vector3(0, 0, 0), Vector3.UnitY);
            _viewProjectionMatrixShadowMap = _viewMatrixShadowMap * _projectionMatrixShadowMap;

            GL.UseProgram(ShaderShadow.GetProgramId());

            foreach (GameObject g in _currentWorld.GetGameObjects())
            {
                Matrix4 modelMatrix =
                    Matrix4.CreateScale(g.GetScale())
                    * Matrix4.CreateFromQuaternion(g.GetRotation())
                    * Matrix4.CreateTranslation(g.Position);

                Matrix4 mvpShadowMap = modelMatrix * _viewProjectionMatrixShadowMap;
                GL.UniformMatrix4(ShaderShadow.GetMatrixId(), false, ref mvpShadowMap);

                GL.BindVertexArray(PrimitiveCube.GetVAOId());
                GL.DrawArrays(PrimitiveType.Triangles, 0, PrimitiveCube.GetPointCount());
                GL.BindVertexArray(0);

               
            }
            GL.UseProgram(0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _fbShadowMap);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbShadowMapDownsampled);
            GL.BlitFramebuffer(0, 0, 1024, 1024, 0, 0, 1024, 1024, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
        }

        private void RenderMomentsMap()
        {
            GL.Viewport(0, 0, 1024, 1024);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbShadowMapColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(ShaderMoments.GetProgramId());

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _fbShadowMapTextureDownsampled);
            GL.Uniform1(ShaderMoments.GetTextureDepthMS(), 0);

            GL.BindVertexArray(PrimitiveQuad.GetVAOId());
            GL.DrawArrays(PrimitiveType.Triangles, 0, PrimitiveQuad.GetPointCount());
            GL.BindVertexArray(0);

            GL.UseProgram(0);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            // Hier passiert das tatsächliche Zeichnen von Formen (z.B. Dreiecke)

            RenderShadowMap();
            RenderMomentsMap();

            // back to normal fb:
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // view-projection matrix:
            Matrix4 viewProjection = _viewMatrix * _projectionMatrix;

            // Shader-Programm wählen:
            GL.UseProgram(ShaderStandard.GetProgramId());

            GL.Uniform3(ShaderStandard.GetLightPositionId(), _currentWorld.GetSunPosition());
            GL.Uniform3(ShaderStandard.GetAmbientLightId(), 0.5f, 0.5f, 0.5f);

            foreach (GameObject g in _currentWorld.GetGameObjects())
            {
                Matrix4 modelMatrix = 
                    Matrix4.CreateScale(g.GetScale()) 
                    * Matrix4.CreateFromQuaternion(g.GetRotation()) 
                    * Matrix4.CreateTranslation(g.Position);

                // model-view-projection matrix erstellen:
                Matrix4 mvp = modelMatrix * viewProjection;

                GL.UniformMatrix4(ShaderStandard.GetMatrixId(), false, ref mvp);
                GL.UniformMatrix4(ShaderStandard.GetModelMatrixId(), false, ref modelMatrix);
                GL.UniformMatrix4(ShaderStandard.GetMatrixShadowId(), false, ref _viewProjectionMatrixShadowMap);

                // Texture an den Shader übertragen:
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, g.GetTextureId());
                GL.Uniform1(ShaderStandard.GetTextureId(), 0);

                //GL.ActiveTexture(TextureUnit.Texture1);
                //GL.BindTexture(TextureTarget.Texture2D, _fbShadowMapTexture);
                //GL.Uniform1(ShaderStandard.GetTextureShadowId(), 1);

                GL.BindVertexArray(PrimitiveCube.GetVAOId());
                GL.DrawArrays(PrimitiveType.Triangles, 0, PrimitiveCube.GetPointCount());
                GL.BindVertexArray(0);

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            GL.UseProgram(0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            // Objekte richten sich je nach Benutzereingaben neu in der Welt aus

            foreach (GameObject g in _currentWorld.GetGameObjects())
            {
                g.Update(KeyboardState, MouseState);
            }
        }
    }
}
