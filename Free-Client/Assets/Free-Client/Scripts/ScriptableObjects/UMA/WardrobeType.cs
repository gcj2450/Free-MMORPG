using UnityEngine;

namespace Assambra.FreeClient.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WardrobeType", menuName = "Assambra/UMA/WardrobeType", order = 1)]
    public class WardrobeType : ScriptableObject
    {
        public string Type;
        public bool HasNoneOption;
    }
}
