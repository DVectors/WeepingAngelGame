using UnityEngine;

namespace Utilities
{
    public static class GameObjectHelper
    {
        public static GameObject FindChildGameObjectByTag(this GameObject parentGameObject, string tag)
        {
            Transform parentObjectTransform = parentGameObject.transform;

            for (int i = 0; i < parentObjectTransform.childCount; i++)
            {
                if (parentObjectTransform.GetChild(i).gameObject.CompareTag(tag))
                {
                    return parentObjectTransform.GetChild(i).gameObject;
                }
            }

            return null;
        }
    }
}
