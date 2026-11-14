using BubbleFriend.Minigame;
using UnityEngine;

namespace BubbleFriend.Bubble
{
    public class BubbleController : MonoBehaviour
    {
        [SerializeField]
        private float _maxRange;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _durationMin, _durationMax;

        private Vector2 _startingPos;
        private Vector2 _dir;
        private float _timer;

        private void Awake()
        {
            _startingPos = transform.position;
        }

        private void Update()
        {
            if (MinigameManager.Instance.Minigame != Minigame.Minigame.None)
            {
                transform.position = Vector2.Lerp(transform.position, _startingPos, Time.deltaTime);
            }
            else
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    if (Vector2.Distance(transform.position, _startingPos) > _maxRange)
                    {
                        _dir = (_startingPos - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        _dir = Random.onUnitCircle;
                    }
                    _timer = Random.Range(_durationMin, _durationMax);
                }

                transform.Translate(_dir * Time.deltaTime * _speed);
            }
        }
    }
}
