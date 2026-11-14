using UnityEngine;

namespace BubbleFriend.Play
{
    public class PlayBubbleController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Vector2 _dir;

        private int _id;

        private bool _isLocked;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetDir(Vector2 dir)
        {
            _dir = dir;
        }

        public void SetId(int id)
        {
            _id = id;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = _id switch
            {
                0 => Color.red,
                1 => Color.green,
                2 => Color.blue,
                _ => Color.white
            };
        }

        private void Update()
        {
            _rb.linearVelocity = _isLocked ? Vector2.zero : _dir * 10f;
        }

        public bool LockOnGrid()
        {
            _isLocked = true;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            var p = PlayManager.Instance.GetGridPos(transform.position);
            transform.position = p.Item1;
            if (!PlayManager.Instance.RegisterToGrid(p.Item2, gameObject))
            {
                Debug.LogWarning($"Failed to register bubble on grid at {p.Item1} => {p.Item2}");
                Destroy(gameObject);
                return false;
            }
            if (p.Item2.y >= 8)
            {
                PlayManager.Instance.EndGame();
                return false;
            }
            return true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isLocked) return;

            if (collision.collider.CompareTag("Delete"))
            {
                _dir = new(_dir.x, Mathf.Abs(_dir.y));
            }
            else if (collision.collider.CompareTag("Roof") || collision.collider.CompareTag("PlayBubble"))
            {
                if (LockOnGrid()) PlayManager.Instance.SpawnBubble();
            }
            else
            {
                var xDir = collision.collider.transform.position.x < transform.position.x ? 1f : -1f;
                _dir = new(Mathf.Abs(_dir.x) * xDir, _dir.y);
            }
        }
    }
}
