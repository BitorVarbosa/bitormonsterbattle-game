using UnityEngine;

namespace BitorMonsterBattle.UI
{
    public abstract class BattlePanelUI : MonoBehaviour
    {
        private static BattlePanelUI _currentBattlePanel;
        protected abstract void OnPanelOpen();
        protected abstract void OnPanelHide();

        public void OpenPanel()
        {
            if (_currentBattlePanel != null)
                _currentBattlePanel.OnPanelHide();

            _currentBattlePanel = this;
            OnPanelOpen();
        }
    }
}
