using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace MomentShadowMappingTest.GameCore
{
    public class GameWorld
    {
        private List<GameObject> _gameObjects = new List<GameObject>();
        private LightObject _sun = new LightObject(25, 25, 25);

        public void AddGameObject(GameObject g)
        {
            _gameObjects.Add(g);
        }

        public bool RemoveGameObject(GameObject g)
        {
            return _gameObjects.Remove(g);
        }

        public List<GameObject> GetGameObjects()
        {
            return _gameObjects;
        }

        public void SetSunPosition(float x, float y, float z)
        {
            _sun.Position = new Vector3(x, y, z);
        }

        public Vector3 GetSunPosition()
        {
            return _sun.Position;
        }
    }
}
