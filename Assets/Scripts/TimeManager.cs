using System;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;


namespace BubbleFriend.Bubble
{
    [AutoStaticsCleanup]
    public partial class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { private set; get; }

        [SerializeField]
        private bool _isUsingSystemTime;

        [SerializeField, Range(0f, 1440f)]
        private float _debugTime;

        [SerializeField]
        private Gradient _dayColor;

        [SerializeField]
        private int _dayStart, _dayEnd;

        [SerializeField]
        private TMP_Text _timeDisplay;

        private float _midday, _durationMorning, _durationAfternoon;

        private Camera _cam;

        public DateTime CurrentTime
        {
            get
            {
#if UNITY_EDITOR
                if (_isUsingSystemTime)
#endif
                {
                    return DateTime.Now;
                }
                return new DateTime(2026, 1, 1, Mathf.FloorToInt(_debugTime / 60), Mathf.FloorToInt(_debugTime % 60), 0);
            }
        }

        public bool IsNight
        {
            get
            {
                var curr = CurrentTime;
                return curr.Hour < _dayStart || curr.Hour >= _dayEnd;
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
            var now = CurrentTime;
            var nowNumeric = now.Hour + (now.Minute / 60f);
            if (IsNight) _cam.backgroundColor = _dayColor.Evaluate(1f);
            else if (nowNumeric < _midday) _cam.backgroundColor = _dayColor.Evaluate(1f - (nowNumeric - _dayStart) / _durationMorning);
            else _cam.backgroundColor = _dayColor.Evaluate(1f - (_dayEnd - nowNumeric) / _durationAfternoon);

            _timeDisplay.text = $"{now.Hour:00}:{now.Minute:00}";
        }
    }
}
