using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "new UserData", menuName = "UserData")]
    public class SoUserData : ScriptableObject
    {
        public UserData Data { get; set; }
    }
}