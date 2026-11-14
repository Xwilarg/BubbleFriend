using Sketch.Translation;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend
{
    [AutoStaticsCleanup]
    public partial class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Animator _collapseAnim, _settingsAnim;

        [SerializeField]
        private TMP_Dropdown _languageDropdown;

        private bool _isCollapsed = false;
        private bool _isSettings = false;

        private void Awake()
        {
            _settingsAnim.SetBool("IsCollapsed", false);
        }

        public void ToggleCollapse()
        {
            _isCollapsed = !_isCollapsed;
            _collapseAnim.SetBool("IsCollapsed", _isCollapsed);
        }

        public void ToggleSettings()
        {
            if (!_isCollapsed) // Close care menu when opening settings
            {
                _isCollapsed = true;
                _collapseAnim.SetBool("IsCollapsed", true);
            }
            _isSettings = !_isSettings;
            _settingsAnim.SetBool("IsCollapsed", _isSettings);
        }

        public void UpdateLanguage(int value)
        {
            Translate.Instance.CurrentLanguage = _languageDropdown.options[value].text;
        }
    }
}
