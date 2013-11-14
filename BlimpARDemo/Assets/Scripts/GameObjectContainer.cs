using UnityEngine;
namespace DRZ
{

    /// <summary>
    /// Defines a GameObject Container to store clones
    /// </summary>
    /// <example>
    /// public static class Containers
    /// {
    ///     public static GameObjectContainer Lasers = new GameObjectContainer("Lasers");    
    /// }
    /// </example>
    public class GameObjectContainer
    {
        private Transform container;
        public Transform transform { get { return container; } }

        private const string baseContainerName = "Containers";
        private static Transform baseContainer;

        public static implicit operator Transform(GameObjectContainer c)
        {
            return c.container;
        }

        public GameObjectContainer(string objectName)
        {
            var tmpGO = GameObject.Find(baseContainerName);
            if (!tmpGO)
            {
                baseContainer = new GameObject(baseContainerName).transform;
                Debug.Log(string.Format("Base Gameobject container not found, created \"{0}\"!", baseContainerName));
            }

            tmpGO = GameObject.Find(objectName);
            if (!tmpGO)
            {
                tmpGO = new GameObject(objectName);
                tmpGO.transform.parent = baseContainer;
                Debug.Log(string.Format("Gameobject container not found, created \"{0}\"!", objectName));
            }

            container = tmpGO.transform;
        }
    }
}
