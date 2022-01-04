using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using OpenTK.Mathematics;

namespace MomentShadowMappingTest.ShaderProgramm
{
    public static class ShaderBlur
    {
        private static int _shaderId = -1;

        private static int _vertexShaderId = -1;
        private static int _fragmentShaderId = -1;

        private static int _uniformTextureInput = -1;
        private static int _uniformAxis = -1;

        public static void Init()
        {
            _shaderId = GL.CreateProgram();

            Assembly a = Assembly.GetExecutingAssembly();

            // Vertex Shader auslesen:
            Stream sVertex = a.GetManifestResourceStream("MomentShadowMappingTest.ShaderProgramm.shaderBlur_vertex.glsl");
            StreamReader sReaderVertex = new StreamReader(sVertex);
            string sVertexCode = sReaderVertex.ReadToEnd();
            sReaderVertex.Dispose();
            sVertex.Close();

            // Fragment Shader auslesen:
            Stream sFragment = a.GetManifestResourceStream("MomentShadowMappingTest.ShaderProgramm.shaderBlur_fragment.glsl");
            StreamReader sReaderFragment = new StreamReader(sFragment);
            string sFragmentCode = sReaderFragment.ReadToEnd();
            sReaderFragment.Dispose();
            sFragment.Close();

            _vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_vertexShaderId, sVertexCode);

            _fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fragmentShaderId, sFragmentCode);

            GL.CompileShader(_vertexShaderId);
            GL.AttachShader(_shaderId, _vertexShaderId);

            GL.CompileShader(_fragmentShaderId);
            GL.AttachShader(_shaderId, _fragmentShaderId);

            GL.LinkProgram(_shaderId);

            _uniformTextureInput = GL.GetUniformLocation(_shaderId, "uTextureInput");
            _uniformAxis = GL.GetUniformLocation(_shaderId, "uAxis");
        }

        public static int GetProgramId()
        {
            return _shaderId;
        }

        public static int GetTextureInputId()
        {
            return _uniformTextureInput;
        }

        public static int GetAxisId()
        {
            return _uniformAxis;
        }
    }
}
