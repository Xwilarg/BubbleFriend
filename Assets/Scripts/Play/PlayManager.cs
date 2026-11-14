using BubbleFriend.Minigame;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BubbleFriend.Play
{
    [AutoStaticsCleanup]
    public partial class PlayManager : MonoBehaviour
    {
        public static PlayManager Instance { private set; get; }

        [SerializeField]
        private Transform _launchPoint;

        [SerializeField]
        private GameObject _playBubblePrefab;
        private GameObject _playBubbleInstance;

        private Camera _cam;

        private bool _canShoot = false;

        private int _startY = 5;
        private int _startX = 3;

        private void Awake()
        {
            Instance = this;
            _cam = Camera.main;
        }

        private void SpawnBubble()
        {
            if (!_canShoot) return;
            _playBubbleInstance = Instantiate(_playBubblePrefab, _launchPoint.transform.position, Quaternion.identity);
        }

        public void Shoot(Vector2 mousePos)
        {
            _canShoot = false;

            var pbc = _playBubbleInstance.GetComponent<PlayBubbleController>();
            pbc.Setup(
                Random.Range(0, 3),
                ((Vector2)transform.position - mousePos).normalized
            );
        }

        public void StartGame()
        {
            SpawnBubble();
            _canShoot = true;
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

        public void OnShoot(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && MinigameManager.Instance.Minigame == Minigame.Minigame.Play && _canShoot)
            {
                var mousePos = MinigameManager.Instance.GetWorldMousePos(_cam);
                if (mousePos != null) Shoot(mousePos.Value);
            }
        }
    }
}
