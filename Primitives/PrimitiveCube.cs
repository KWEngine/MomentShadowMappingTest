using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;

namespace MomentShadowMappingTest.Primitives
{
    public static class PrimitiveCube
    {
        private static int _vao;

        private static readonly float[] VERTICES = { 
			// front face
	        -0.5f, 0.5f, 0.5f, // 1st triangle
	        -0.5f, -0.5f, 0.5f, // 1st triangle
	        0.5f, 0.5f, 0.5f, // 1st triangle
	        -0.5f, -0.5f, 0.5f, // 2nd triangle
	        0.5f, -0.5f, 0.5f, // 2nd triangle
	        0.5f, 0.5f, 0.5f, // 2nd triangle

	        // right face
	        0.5f, 0.5f, 0.5f,
            0.5f, -0.5f, 0.5f,
            0.5f, 0.5f, -0.5f,
            0.5f, -0.5f, 0.5f,
            0.5f, -0.5f, -0.5f,
            0.5f, 0.5f, -0.5f,
	        
	        // back face
	        0.5f, 0.5f, -0.5f,
            0.5f, -0.5f, -0.5f,
            -0.5f, 0.5f, -0.5f,
            0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, 0.5f, -0.5f,
	        
	        // left face
	        -0.5f, 0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, 0.5f, 0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, 0.5f,
            -0.5f, 0.5f, 0.5f,
	        
	        // top face
	        -0.5f, 0.5f, -0.5f,
            -0.5f, 0.5f, 0.5f,
            0.5f, 0.5f, -0.5f,
            -0.5f, 0.5f, 0.5f,
            0.5f, 0.5f, 0.5f,
            0.5f, 0.5f, -0.5f,
	        
	        // bottom face
	        0.5f, -0.5f, -0.5f,
            0.5f, -0.5f, 0.5f,
            -0.5f, -0.5f, -0.5f,
            0.5f, -0.5f, 0.5f,
            -0.5f, -0.5f, 0.5f,
            -0.5f, -0.5f, -0.5f
        };

        private static readonly float[] NORMALS = {
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,

            1, 0, 0,
            1, 0, 0,
            1, 0, 0,
            1, 0, 0,
            1, 0, 0,
            1, 0, 0,

            0, 0, -1,
            0, 0, -1,
            0, 0, -1,
            0, 0, -1,
            0, 0, -1,
            0, 0, -1,

            -1, 0, 0,
            -1, 0, 0,
            -1, 0, 0,
            -1, 0, 0,
            -1, 0, 0,
            -1, 0, 0,

            0, 1, 0,
            0, 1, 0,
            0, 1, 0,
            0, 1, 0,
            0, 1, 0,
            0, 1, 0,

            0, -1, 0,
            0, -1, 0,
            0, -1, 0,
            0, -1, 0,
            0, -1, 0,
            0, -1, 0

        };

        private static readonly float[] UVS = {
			// front
			0, 0,
            0, 1,
            1, 0,
            0, 1,
            1, 1,
            1, 0,
	        
	        // right
	        0, 0,
            0, 1,
            1, 0,
            0, 1,
            1, 1,
            1, 0,
	        
	        // back
	        0, 1,
            0, 0,
            1, 1,
            0, 0,
            1, 0,
            1, 1,
	        
	        // left
	        0, 0,
            0, 1,
            1,0,
            0,1,
            1,1,
            1,0,
	        
	        //top
	        0, 0,
            0, 1,
            1, 0,
            0, 1,
            1, 1,
            1, 0,

            1, 1,
            1, 0,
            0, 1,
            1, 0,
            0, 0,
            0, 1
        };

        public static void Init()
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            int vboVertices = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, VERTICES.Length * 4, VERTICES, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            int vboUVs = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUVs);
            GL.BufferData(BufferTarget.ArrayBuffer, UVS.Length * 4, UVS, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            int vboNormals = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
            GL.BufferData(BufferTarget.ArrayBuffer, NORMALS.Length * 4, NORMALS, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);
        }

        public static int GetVAOId()
        {
            return _vao;
        }

        public static int GetPointCount()
        {
            return 36;
        }
    }
}
