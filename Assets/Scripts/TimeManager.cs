using System;
using TMPro;
using UnityEngine;


namespace BubbleFriend.Bubble
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { private set; get; }

        [SerializeField]
        private bool _isUsingSystemTime;

        [SerializeField]
        private float _debugTime;

        [SerializeField]
        private Gradient _dayColor;

        [SerializeField]
        private int _dayStart, _dayEnd;

        [SerializeField]
        private TMP_Text _timeDisplay;

        private float _midday, _durationMorning, _durationAfternoon;

        private Camera _cam;

        public float CurrentTime
        {
            get
            {
#if UNITY_EDITOR
                if (_isUsingSystemTime)
#endif
                {
                    var now = DateTime.Now;
                    return now.Hour + (now.Minute / 60f);
                }
                return _debugTime;
            }
        }

        private void Awake()
        {
            Instance = this;
            _cam = Camera.main;
            _midday = (_dayEnd - _dayStart) / 2f + _dayStart;
            _durationMorning = _midday - _dayStart;
            _durationAfternoon = _dayEnd - _midday;
        }

        private void Update()
        {
            var nowNumeric = CurrentTime;
            if (nowNumeric < _dayStart || nowNumeric >= _dayEnd) _cam.backgroundColor = _dayColor.Evaluate(1f);
            else if (nowNumeric < _midday) _cam.backgroundColor = _dayColor.Evaluate(1f - (nowNumeric - _dayStart) / _durationMorning);
            else _cam.backgroundColor = _dayColor.Evaluate(1f - (_dayEnd - nowNumeric) / _durationAfternoon);
        }
    }
}
