using BubbleFriend.Minigame;
using System.Collections.Generic;
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

        private int _objective = 0;

        private Dictionary<Vector2Int, PlayBubbleController> _grid = new();

        private void Awake()
        {
            Instance = this;
            _cam = Camera.main;
        }

        public void SpawnBubble()
        {
            _canShoot = true;
            _playBubbleInstance = Instantiate(_playBubblePrefab, _launchPoint.transform.position, Quaternion.identity);
            _playBubbleInstance.GetComponent<PlayBubbleController>().SetId(Random.Range(0, 3));
        }

        public void GetAdjacents(Vector2Int pos, int id, List<Vector2Int> elements)
        {
            if (elements.Contains(pos) || _grid[pos].Id != id) return;

            elements.Add(pos);
            if (_grid.ContainsKey(new Vector2Int(pos.x - 1, pos.y))) GetAdjacents(new Vector2Int(pos.x - 1, pos.y), id, elements);
            if (_grid.ContainsKey(new Vector2Int(pos.x + 1, pos.y))) GetAdjacents(new Vector2Int(pos.x + 1, pos.y), id, elements);

            if (_grid.ContainsKey(new Vector2Int(pos.x, pos.y + 1))) GetAdjacents(new Vector2Int(pos.x, pos.y + 1), id, elements);
            if (_grid.ContainsKey(new Vector2Int(pos.x, pos.y - 1))) GetAdjacents(new Vector2Int(pos.x, pos.y - 1), id, elements);
            if (pos.y % 2 == 0)
            {
                if (_grid.ContainsKey(new Vector2Int(pos.x + 1, pos.y + 1))) GetAdjacents(new Vector2Int(pos.x + 1, pos.y + 1), id, elements);
                if (_grid.ContainsKey(new Vector2Int(pos.x + 1, pos.y - 1))) GetAdjacents(new Vector2Int(pos.x + 1, pos.y - 1), id, elements);
            }
            else
            {
                if (_grid.ContainsKey(new Vector2Int(pos.x - 1, pos.y + 1))) GetAdjacents(new Vector2Int(pos.x - 1, pos.y + 1), id, elements);
                if (_grid.ContainsKey(new Vector2Int(pos.x - 1, pos.y - 1))) GetAdjacents(new Vector2Int(pos.x - 1, pos.y - 1), id, elements);
            }
        }

        public void DestroyFromGrid(Vector2Int p)
        {
            Destroy(_grid[p].gameObject);
            _grid.Remove(p);
        }

        public void Shoot(Vector2 mousePos)
        {
            _canShoot = false;

            var pbc = _playBubbleInstance.GetComponent<PlayBubbleController>();
            pbc.SetDir(
                (mousePos - (Vector2)_playBubbleInstance.transform.position).normalized
            );
        }

        public void StartGame(int count)
        {
            _objective = count;

            var xMax = (_startX * 2 + 1) * 2 + 1;
            for (int y = 0; y < 6; y++)
            {
                for (var x = 0; x < xMax; x++)
                {
                    if ((y == 0 && Random.Range(0, 100) < 90) || (y == 1 && Random.Range(0, 100) < 50))
                    {
                        if (y == 0 || (_grid.ContainsKey(new(x + 1, y)) || _grid.ContainsKey(new(x, y))))
                        {
                            var bubble = Instantiate(_playBubblePrefab, new(-_startX + x / 2f - .25f, _startY - y - .25f), Quaternion.identity);
                            var c = bubble.GetComponent<PlayBubbleController>();
                            c.SetId(Random.Range(0, 4));
                            c.LockOnGrid(false);
                        }
                    }
                    if ((y == 0 && Random.Range(0, 100) < 70) || (y == 1 && Random.Range(0, 100) < 25))
                    {
                        if (_grid.ContainsKey(new(x - 1, y)) || _grid.ContainsKey(new(x, y)))
                        {
                            var bubble = Instantiate(_playBubblePrefab, new(-_startX + x / 2f - .5f, _startY - y - .75f), Quaternion.identity);
                            var c = bubble.GetComponent<PlayBubbleController>();
                            c.SetId(Random.Range(0, 4));
                            c.LockOnGrid(false);
                        }
                    }
                }
            }

            SpawnBubble();
        }

        public void ValidateMatch()
        {
            _objective--;
            BubbleManager.Instance.ClearNeed(Need.Play, .1f);
            if (_objective == 0) EndGame(false);
        }

        public void EndGame(bool badEnding)
        {
            foreach (var obj in _grid)
            {
                Destroy(obj.Value.gameObject);
            }
            if (_playBubbleInstance != null)
            {
                Destroy(_playBubbleInstance);
            }
            _grid.Clear();

            MinigameManager.Instance.Minigame = Minigame.Minigame.None;
            _canShoot = true;

            BubbleManager.Instance.UpdateHappiness(badEnding ? -1 : 2);
        }

        private float SqrtDistance(Vector2 a, Vector2 b) // Distance without sqrt for optimization
        {
            return Mathf.Pow(a.x - b.x, 2f) + Mathf.Pow(a.y - b.y, 2f);
        }

        public bool RegisterToGrid(Vector2Int pos, PlayBubbleController obj)
        {
            if (_grid.ContainsKey(pos)) return false;
            _grid.Add(pos, obj);
            _playBubbleInstance = null;
            return true;
        }

        public (Vector2, Vector2Int) GetGridPos(Vector2 pos)
        {
            Vector2 closest = Vector2.zero;
            Vector2Int gridPos = Vector2Int.zero;
            float dist = float.MaxValue;

            // There is probably a wayyyyyyyyyyyyy better way to do this but I have no idea how to do the math
            var xMax = (_startX * 2 + 1) * 2 + 1;
            for (int y = 0; y < 6; y++)
            {
                for (var x = 0; x < xMax; x++)
                {
                    var p = new Vector2(-_startX + x / 2f - .5f, _startY - y - .75f);
                    var pDist = SqrtDistance(pos, p);
                    if (pDist < dist)
                    {
                        closest = p;
                        dist = pDist;
                        gridPos = new(x, y * 2 + 1);
                    }
                    if (x < xMax - 1)
                    {
                        var p2 = new Vector2(-_startX + x / 2f - .25f, _startY - y - .25f);
                        var p2Dist = SqrtDistance(pos, p2);
                        if (p2Dist < dist)
                        {
                            closest = p2;
                            dist = p2Dist;
                            gridPos = new(x, y * 2);
                        }
                    }
                }
            }

            return (closest, gridPos);
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
