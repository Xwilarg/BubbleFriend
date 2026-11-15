using BubbleFriend.Bubble;
using BubbleFriend.Play;
using Sketch.Common;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BubbleFriend.Minigame
{
    public enum Minigame
    {
        None,
        Cleaning,
        Play
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

        [SerializeField]
        private PlayerInput _pInput;

        private Vector2 _lastPos;

        private int _dirtLayerMask;
        private int _dirtCount = 0;

        private void Awake()
        {
            Instance = this;
            _cam = Camera.main;
            _dirtLayerMask = LayerMask.GetMask("Dirt");
        }

        private void Update()
        {
            if (_timer < 1f)
            {
                _timer += Time.deltaTime;
                if (_timer > 1f) _timer = 1f;
            }
            _cam.orthographicSize = Mathf.Lerp(Minigame != Minigame.Cleaning ? 3f : 5f, Minigame != Minigame.Cleaning ? 5f : 3f, _timer);

            if (Minigame == Minigame.Cleaning)
            {
                var newPos = GetWorldMousePos(_cam);
                if (newPos != null && newPos != _lastPos)
                {
                    var eff = Vector2.Distance(newPos.Value, _lastPos);
                    var dirts = Physics2D.OverlapCircleAll(newPos.Value, .5f, _dirtLayerMask);
                    foreach (var dirt in dirts)
                    {
                        var newScale = dirt.transform.localScale.x - (eff * .01f);
                        if (newScale < .1f)
                        {
                            _dirtCount--;
                            Destroy(dirt.gameObject);
                            if (_dirtCount == 0)
                            {
                                _timer = 0f;
                                Minigame = Minigame.None;
                                BubbleManager.Instance.ClearNeed(Need.Clean, 1f);
                                BubbleManager.Instance.UpdateHappiness(1);
                                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                            }
                        }
                        else dirt.transform.localScale = Vector3.one * newScale;
                    }
                    _lastPos = newPos.Value;
                }
            }
        }

        public void StartCleaningMinigame()
        {
            if (Minigame != Minigame.None) return;
            _dirtCount = BubbleManager.Instance.NeedOnTen(Need.Clean);

            if (_dirtCount == 0)
            {
                WarningManager.Instance.ShowWarning("no_wash");
                return;
            }

            Minigame = Minigame.Cleaning;
            _timer = 0f;
            Cursor.SetCursor(_cleaningCursor, Vector2.zero, CursorMode.Auto);

            for (int i = 0; i < _dirtCount; i++)
            {
                var dirt = Instantiate(_dirtPrefab, _bubble);
                dirt.transform.SetLocalPositionAndRotation(Random.insideUnitCircle * Random.Range(0f, 3f), Quaternion.Euler(0f, 0f, Random.value * Mathf.PI * 2f));
                dirt.transform.localScale = Vector3.one * Random.Range(.3f, .4f);
            }

            _lastPos = GetWorldMousePos(_cam) ?? Vector2.zero;
        }

        public void TryOpenFoodMenu()
        {
            bool allowed = BubbleManager.Instance.NeedOnTen(Need.Feed) > 0;
            if (!allowed)
            {
                WarningManager.Instance.ShowWarning("no_eat");
                return;
            }
            MenuManager.Instance.ToggleFlavorMenu(true);
        }

        public void GiveFood(int flavor)
        {
            var isGoodFood = BubbleManager.Instance.CurrentTaste == (Flavor)flavor;
            MenuManager.Instance.ToggleFlavorMenu(false);
            BubbleManager.Instance.ClearNeed(Need.Feed, 1f);
            BubbleManager.Instance.UpdateHappiness(isGoodFood ? 1 : -1);
        }

        public void StartPlay()
        {
            if (Minigame != Minigame.None) return;

            int count = Mathf.CeilToInt(BubbleManager.Instance.NeedOnTen(Need.Play) / 2f);
            if (count == 0)
            {
                WarningManager.Instance.ShowWarning("no_play");
                return;
            }

            Minigame = Minigame.Play;
            PlayManager.Instance.StartGame(count + 1);
        }

        public Vector2? GetWorldMousePos(Camera cam)
        {
            var mousePos = CursorUtils.GetPosition(_pInput);
            if (mousePos != null)
            {
                Gizmos.color = Color.white;
                var worldPos = Camera.main.ScreenToWorldPoint(mousePos.Value);
                worldPos.z = 0f;
                return worldPos;
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            var pos = GetWorldMousePos(Camera.main);
            if (pos != null) Gizmos.DrawWireSphere(pos.Value, .25f);
        }
    }
}
