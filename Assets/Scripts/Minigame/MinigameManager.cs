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

        private Camera _cam;
        private float _timer = 1f;

        private void Awake()
        {
            Instance = this;
            _cam = Camera.main;
        }

        private void Update()
        {
            if (_timer < 1f)
            {
                _timer += Time.deltaTime;
                if (_timer > 1f) _timer = 1f;
            }
            _cam.orthographicSize = Mathf.Lerp(Minigame == Minigame.None ? 3f : 5f, Minigame == Minigame.None ? 5f : 3f, _timer);
        }

        public void StartCleaningMinigame()
        {
            Minigame = Minigame.Cleaning;
            _timer = 0f;
        }
    }
}
