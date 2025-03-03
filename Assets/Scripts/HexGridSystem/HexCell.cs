using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    private UnitGameObject AttachedUnit {get; set;}

    public bool IsUnitAttached()
    {
        return AttachedUnit != null;
    }

    public void AttachUnit(UnitGameObject unitToAttach)
    {
        AttachedUnit = unitToAttach;
    }

    public void DetachUnit()
    {
        AttachedUnit = null;
    }
}
