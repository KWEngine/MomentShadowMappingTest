using System;
using System.Collections.Generic;
using System.Text;
using MomentShadowMappingTest.Textures;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MomentShadowMappingTest.GameCore
{
    public class GameObject
    {
        public Vector3 Position = new Vector3(0, 0, 0);
        private Quaternion Orientation = new Quaternion(0, 0, 0, 1);
        private Vector3 Scale = new Vector3(1, 1, 1);
        private int _textureId = -1;

        public int GetTextureId()
        {
            return _textureId;
        }

        public void SetTexture(string filename)
        {
            if(TextureLoader.IsTextureAlreadyDefined(filename))
            {
                _textureId = TextureLoader.GetTexture(filename);
            }
            else
            {
                _textureId = TextureLoader.LoadTexture(filename);
            }
        }

        public void SetScale(float x, float y, float z)
        {
            if(x > 0 && y > 0 && z > 0)
            {
                Scale = new Vector3(x, y, z);
            }
            else
            {
                // Fehlerfall: entweder Exception oder Fehlermeldung ausgeben!
                Scale = new Vector3(1, 1, 1);
            }
        }

        public Vector3 GetScale()
        {
            return Scale;
        }

        public void AddRotation(Axis a, float degrees)
        {
            Quaternion newrotation;
            if (a == Axis.X)
            {
                newrotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(degrees));
            }
            else if(a == Axis.Y)
            {
                newrotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(degrees));
            }
            else
            {
                newrotation = Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(degrees));
            }

            Orientation = Orientation * newrotation;
        }

        public Quaternion GetRotation()
        {
            return Orientation;
        }

        public void Update(KeyboardState k, MouseState s)
        {
            if(k[Keys.W])
            {
                Position = new Vector3(Position.X, Position.Y + 1, Position.Z);
            }
            if (k[Keys.S])
            {
                Position = new Vector3(Position.X, Position.Y - 1, Position.Z);
            }
            if (k[Keys.A])
            {
                Position = new Vector3(Position.X - 1, Position.Y, Position.Z);
            }
            if (k[Keys.D])
            {
                Position = new Vector3(Position.X + 1, Position.Y, Position.Z);
            }
        }

    }
}
