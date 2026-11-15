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

        [SerializeField]
        private GameObject _speech;

        [SerializeField]
        private SpriteRenderer _speechSR;

        [SerializeField]
        private Sprite _sleepIcon;

        private Vector2 _startingPos;
        private Vector2 _dir;
        private float _timer;

        private float _throughtTimer = 0f;

        public void ShowThought(Sprite sprite)
        {
            _throughtTimer = 3f;
            _speechSR.sprite = sprite;
        }

        private void Awake()
        {
            _startingPos = transform.position;
            _speech.SetActive(false);
        }

        private void Update()
        {
            if (_throughtTimer > 0f)
            {
                _throughtTimer -= Time.deltaTime;
                if (_throughtTimer <= 0f)
                {
                    _speech.SetActive(false);
                }
            }

            if (MinigameManager.Instance.Minigame != Minigame.Minigame.None)
            {
                if (_throughtTimer <= 0f) _speech.SetActive(false);
                transform.position = Vector2.Lerp(transform.position, _startingPos, Time.deltaTime);
            }
            else if (TimeManager.Instance.IsNight)
            {
                if (_throughtTimer <= 0f)
                {
                    _speech.SetActive(true);
                    _speechSR.sprite = _sleepIcon;
                }

                return;
            }
            else
            {
                transform.Translate(_dir * Time.deltaTime * _speed);
            }

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
        }
    }
}
