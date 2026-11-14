using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend.Play
{
    [AutoStaticsCleanup]
    public partial class PlayManager : MonoBehaviour
    {
        public static PlayManager Instance { private set; get; }

        private int _startY = 5;
        private int _startX = 3;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, .5f);
            var xMax = (_startX * 2 + 1) * 2 + 1;
            for (int y = 0; y < 6; y ++)
            {
                for (var x = 0; x < xMax; x++)
                {
                    Gizmos.DrawWireCube(new(-_startX + x / 2f - .5f, _startY - y - .75f), Vector3.one * .5f);
                    if (x < xMax - 1) Gizmos.DrawWireCube(new(-_startX + x / 2f - .25f, _startY - y - .25f), Vector3.one * .5f);
                }
            }
        }
    }
}
