using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

namespace MomentShadowMappingTest.Textures
{
    public static class TextureLoader
    {
        private static Dictionary<string, int> _textureDictionary = new Dictionary<string, int>();

        public static void RegisterTexture(string filename, int openglid)
        {
            if(_textureDictionary.ContainsKey(filename))
            {
                // Fehlerfall:
                throw new Exception("Texture is already stored in dictionary.");
            }
            else
            {
                _textureDictionary.Add(filename, openglid);
            }
        }

        public static bool IsTextureAlreadyDefined(string filename)
        {
            return _textureDictionary.ContainsKey(filename);
        }

        public static int GetTexture(string filename)
        {
            _textureDictionary.TryGetValue(filename, out int id);
            return id;
        }


        public static int LoadTexture(string file)
        {
            if(IsTextureAlreadyDefined(file))
            {
                throw new Exception("Texture is already defined.");
            }

            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream(file);

            Bitmap b = new Bitmap(s);
            if(b == null)
            {
                return -1; // Fehlerfall
            }
            else
            {
                BitmapData bmd = b.LockBits(
                    new Rectangle(0, 0, b.Width, b.Height), 
                    ImageLockMode.ReadOnly, 
                    b.PixelFormat
                    );

                int textureId = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                    b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? PixelInternalFormat.Rgb8 : PixelInternalFormat.Rgba8, 
                    b.Width, b.Height, 0,
                    b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? OpenTK.Graphics.OpenGL4.PixelFormat.Bgr : OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, 
                    PixelType.UnsignedByte, bmd.Scan0);

                GL.TexParameter(
                    TextureTarget.Texture2D, 
                    (TextureParameterName)OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 
                    8
                    );
                GL.TexParameter(
                    TextureTarget.Texture2D, 
                    TextureParameterName.TextureMinFilter, 
                    (int)TextureMinFilter.LinearMipmapLinear
                    );
                GL.TexParameter(
                    TextureTarget.Texture2D, 
                    TextureParameterName.TextureMagFilter, 
                    (int)TextureMagFilter.Linear
                    );
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                b.Dispose();
                s.Close();

                RegisterTexture(file, textureId);

                return textureId;
            }
        }
    }
}
