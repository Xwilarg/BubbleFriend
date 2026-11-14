using UnityEngine;

namespace BubbleFriend.Minigame
{
    public enum Minigame
    {
        None,
        Cleaning
    }

    public class MinigameManager : MonoBehaviour
    {
        public static MinigameManager Instance { private set; get; }

        public Minigame Minigame { set; get; }

        private void Awake()
        {
            Instance = this;
        }

        public void StartCleaningMinigame()
        {
            Minigame = Minigame.Cleaning;
        }
    }
}
