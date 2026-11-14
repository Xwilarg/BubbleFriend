using UnityEngine;

namespace BubbleFriend.Minigame
{
    public class MinigameManager : MonoBehaviour
    {
        public static MinigameManager Instance { private set; get; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
