#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PlayerPrefsCleaner
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs 데이터 삭제 완료");
        }
    }
}
#endif