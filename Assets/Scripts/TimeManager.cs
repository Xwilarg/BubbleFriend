using System;
using UnityEngine;


namespace BubbleFriend
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { private set; get; }

        [SerializeField]
        private Gradient _dayColor;

        [SerializeField]
        private int _dayStart, _dayEnd;

        private float _midday, _durationMorning, _durationAfternoon;

        private Camera _cam;

        private void Awake()
        {
            Instance = this;
            _cam = Camera.main;
            _midday = (_dayEnd - _dayStart) / 2f;
            _durationMorning = _midday - _dayStart;
            _durationAfternoon = _dayEnd - _midday;
        }

        private void Update()
        {
            var now = DateTime.Now;
            var nowNumeric = now.Hour + (now.Minute / 60f);
            if (nowNumeric < _dayStart || nowNumeric >= _dayEnd) _cam.backgroundColor = _dayColor.Evaluate(1f);
            else if (nowNumeric < _midday) _cam.backgroundColor = _dayColor.Evaluate(nowNumeric - _dayStart / _durationMorning);
            else _cam.backgroundColor = _dayColor.Evaluate(_dayEnd - nowNumeric / _durationAfternoon);
        }
    }
}
