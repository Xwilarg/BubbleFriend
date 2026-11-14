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

        public void Setup(int id, Vector2 dir)
        {
            _dir = dir;
            _id = id;
        }

        private void Update()
        {
            _rb.linearVelocity = _isLocked ? Vector2.zero : _dir * 10f;
        }

        public void LockOnGrid()
        {
            _isLocked = true;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.position = PlayManager.Instance.GetGridPos(transform.position);
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
                LockOnGrid();
                PlayManager.Instance.SpawnBubble();
            }
            else
            {
                var xDir = collision.collider.transform.position.x < transform.position.x ? 1f : -1f;
                _dir = new(Mathf.Abs(_dir.x) * xDir, _dir.y);
            }
        }
    }
}
