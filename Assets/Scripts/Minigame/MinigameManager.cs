using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend.Minigame
{
    public enum Minigame
    {
        None,
        Cleaning
    }

    [AutoStaticsCleanup]
    public partial class MinigameManager : MonoBehaviour
    {
        public static MinigameManager Instance { private set; get; }

        public Minigame Minigame { set; get; }

        private Camera _cam;
        private float _timer = 1f;

        [SerializeField]
        private Transform _bubble;

        [SerializeField]
        private GameObject _dirtPrefab;

        [SerializeField]
        private Texture2D _cleaningCursor;

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
            Cursor.SetCursor(_cleaningCursor, Vector2.zero, CursorMode.Auto);

            var max = Mathf.Clamp(1, 10, BubbleManager.Instance.CleaningNeed * 10f);
            for (int i = 0; i < max; i++)
            {
                var dirt = Instantiate(_dirtPrefab, _bubble);
                dirt.transform.SetLocalPositionAndRotation(Random.insideUnitCircle * Random.Range(0f, 3f), Quaternion.Euler(0f, 0f, Random.value * Mathf.PI * 2f));
                dirt.transform.localScale = Vector3.one * Random.Range(.3f, .4f);
            }
        }
    }
}
