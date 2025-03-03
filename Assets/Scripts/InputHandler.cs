using UnityEngine;

public class InputHandler : MonoBehaviour
{
	[SerializeField] private UnitGameObject player;

	// [SerializeField] private BattleSystem battleSystem;

    private void Update() 
    {
		if (Input.GetMouseButtonDown(1)) 
        {
			HandleInput();
		}
	}

	private void HandleInput() 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit)) 
        {
			HexCell moveCell = HexGrid.Instance.SelectCell(hit.point);

			if (!moveCell.IsUnitAttached() /*&& battleSystem.IsPlayerInput()*/)
			{
				StartCoroutine(player.unitAnimation.SmoothRotateAndMoveToCell(moveCell));
			}
		}
	}

	public void PlayerAttack(Transform enemy)
	{
		StartCoroutine(player.unitAnimation.SmoothRotateAndAttack(enemy));
	}
}
