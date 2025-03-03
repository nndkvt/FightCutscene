public class CloseCombatUnitGameObject : UnitGameObject
{
    public override void TrySpawnCharacter(HexCell cell)
    {
        base.TrySpawnCharacter(cell);

        AttackDistance = 1;
    }
}
