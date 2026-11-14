using Sketch.Translation;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend
{
    [AutoStaticsCleanup]
    public partial class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { private set; get; }

        [SerializeField]
        private Animator _collapseAnim, _settingsAnim;

        [SerializeField]
        private TMP_Dropdown _languageDropdown;

        [SerializeField]
        private GameObject _soapFlavorsMenu;

        private bool _isCollapsed = false;
        private bool _isSettings = false;

        private void Awake()
        {
            Instance = this;

            _settingsAnim.SetBool("IsCollapsed", false);
            ToggleFlavorMenu(false);
        }

        public void ToggleCollapse()
        {
            _isCollapsed = !_isCollapsed;
            _collapseAnim.SetBool("IsCollapsed", _isCollapsed);

            if (_isCollapsed)
            {
                ToggleFlavorMenu(false);
            }
        }

        public void ToggleSettings()
        {
            if (!_isCollapsed) // Close care menu when opening settings
            {
                _isCollapsed = true;
                _collapseAnim.SetBool("IsCollapsed", true);
                ToggleFlavorMenu(false);
            }
            _isSettings = !_isSettings;
            _settingsAnim.SetBool("IsCollapsed", _isSettings);
        }

        public void UpdateLanguage(int value)
        {
            Translate.Instance.CurrentLanguage = _languageDropdown.options[value].text;
        }

        public void ToggleFlavorMenu(bool value)
        {
            _soapFlavorsMenu.SetActive(value);
        }
    }
}
