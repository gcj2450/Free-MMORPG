using UnityEngine;

namespace Assambra.FreeClient.Helper
{
    public static class RandomString
    {
        public static string GetNumericString(int n)
        {
            return Random.Range(0, n).ToString();
        }
    }
}
