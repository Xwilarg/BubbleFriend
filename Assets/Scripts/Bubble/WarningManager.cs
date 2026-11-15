using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend.Bubble
{
    [AutoStaticsCleanup]
    public partial class WarningManager : MonoBehaviour
    {
        public static WarningManager Instance { private set; get; }

        private void Awake()
        {
            Instance = this;
        }

        public void ShowWarning(string key)
        {

        }
    }
}
