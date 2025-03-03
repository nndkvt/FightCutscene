using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
	public int height = 6;

	public static HexGrid Instance;

    public Text cellLabelPrefab;

	public HexCell cellPrefab;

    public UnitGameObject player;
	public UnitGameObject[] enemies;

    private HexCell[] cells;

    private Canvas gridCanvas;
    private HexMesh hexMesh;

	private Action<HexCell> TrySpawnUnit;

	[SerializeField] private bool _isDisplayCoordinates;

	private void Awake () 
    {
		Instance = this;

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

		TrySpawnUnit += player.TrySpawnCharacter;
		player.OnUnitDead += RespawnCharacter;

		foreach (UnitGameObject enemy in enemies)
		{
			TrySpawnUnit += enemy.TrySpawnCharacter;
			//enemy.OnUnitDead += RespawnCharacter;
		}

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) 
        {
			for (int x = 0; x < width; x++) 
            {
				CreateCell(x, z, i++);
			}
		}

		TrySpawnUnit -= player.TrySpawnCharacter;
		
		foreach (UnitGameObject enemy in enemies)
		{
			TrySpawnUnit -= enemy.TrySpawnCharacter;
		}
	}

    private void Start () 
    {
		hexMesh.Triangulate(cells);
	}
	
	private void CreateCell (int x, int z, int i) 
    {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		if (_isDisplayCoordinates)
		{
			Text label = Instantiate(cellLabelPrefab);
			label.rectTransform.SetParent(gridCanvas.transform, false);
			label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
			label.text = cell.coordinates.ToStringOnSeparateLines();
		}

		TrySpawnUnit?.Invoke(cell);
	}
	
	public HexCell SelectCell (Vector3 position) 
    {
		position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

		if (FindCell(coordinates, out HexCell cell))
		{
			return cell;
		}

		return null;
	}

	private bool FindCell(HexCoordinates coordinates, out HexCell outCell)
	{
		foreach (HexCell cell in cells)
		{
			if (cell.coordinates == coordinates)
			{
				outCell = cell;
				return true;
			}
		}

		outCell = null;
		return false;
	}

	public void RespawnCharacter(UnitGameObject character)
	{
		Random random = new Random();

		int spawnCellIndex = random.Next(cells.Length);

		if (!cells[spawnCellIndex].IsUnitAttached())
		{
			character.unitAnimation.MoveCharacter(cells[spawnCellIndex]);
			return;
		}
		else
		{
			RespawnCharacter(character);
		}
	}
}
