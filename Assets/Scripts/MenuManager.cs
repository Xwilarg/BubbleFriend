using UnityEngine;

namespace BubbleFriend
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Animator _collapseAnim, _settingsAnim;

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
    }
}
