using Sketch.Translation;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend.Bubble
{
    [AutoStaticsCleanup]
    public partial class WarningManager : MonoBehaviour
    {
        public static WarningManager Instance { private set; get; }

        [SerializeField]
        private TMP_Text _warningText;

        private float _timer;

        private void Awake()
        {
            Instance = this;

            _warningText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _warningText.gameObject.SetActive(false);
                }
            }
        }

        public void ShowWarning(string key)
        {
            _timer = 3f;
            _warningText.text = Translate.Instance.Tr(key);
            _warningText.gameObject.SetActive(true);
        }
    }
}
