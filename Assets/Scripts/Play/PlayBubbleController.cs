using UnityEngine;

namespace BubbleFriend.Play
{
    public class PlayBubbleController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Vector2 _dir;

        private int _id;

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
            _rb.linearVelocity = _dir;
        }
    }
}
