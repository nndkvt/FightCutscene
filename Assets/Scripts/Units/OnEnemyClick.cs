using UnityEngine;
using UnityEngine.EventSystems;

public class OnEnemyClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Canvas _onEnemyClickedCanvas;
    // [SerializeField] private BattleSystem _battleSystem;

    public void OnPointerClick(PointerEventData eventData)
    {
        /*
        if (_battleSystem.IsPlayerInput())
        {
            _onEnemyClickedCanvas.gameObject.SetActive(true);
        }
        */

        _onEnemyClickedCanvas.gameObject.SetActive(true);
    }
}
