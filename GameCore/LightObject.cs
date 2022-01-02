using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MomentShadowMappingTest.GameCore
{
    public class LightObject
    {
        public Vector3 Position = new Vector3(0, 0, 0);

        public LightObject(float x, float y, float z)
        {
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
        }
    }
}
