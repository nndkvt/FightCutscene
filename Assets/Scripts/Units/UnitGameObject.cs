using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UnitGameObject : MonoBehaviour
{
    public Unit unitInfo;

    [SerializeField] protected Image hpBar;

    public Action<UnitGameObject> OnUnitDead;

    protected float _currentHP;

    public int AttackDistance {get; set;}

    [SerializeField] private GameObject _onDeathEffect;

    [NonSerialized] public UnitAnimation unitAnimation;

    public virtual void TrySpawnCharacter(HexCell cell)
    {
        if (cell.coordinates == unitInfo.spawnCoordinates)
        {
            unitAnimation = GetComponent<UnitAnimation>();

            _currentHP = unitInfo.maxHP;
            unitAnimation.OnUnitFinishedMoving += ReattachToCell;

            unitAnimation.MoveCharacter(cell);
        }
    }

    public bool TakeDamage(float damage)
    {
        _currentHP -= damage;

        hpBar.fillAmount = _currentHP / unitInfo.maxHP;

        if (_currentHP <= 0)
        {
            Debug.Log("I'm dead");
            Instantiate(_onDeathEffect, transform.position, _onDeathEffect.transform.rotation);

            OnUnitDead?.Invoke(this);

            _currentHP = unitInfo.maxHP;
            hpBar.fillAmount = _currentHP / unitInfo.maxHP;

            Destroy(gameObject);

            return true;
        }

        return false;
    }

    protected void OnParticleCollision(GameObject other) 
    {
        Debug.Log("Damage from collision");
        TakeDamage(other.GetComponent<Bomb>().attackValue);
    }

    public void OnRaycastHit()
    {
        Debug.Log("Damage from raycast");
    }

    public bool CanAttackUnit(UnitGameObject unitToAttack)
    {
        int distance = HexCoordinates.CalculateDistance(unitAnimation.CurrentHexCell.coordinates, 
                                                        unitToAttack.unitAnimation.CurrentHexCell.coordinates);

        return distance == AttackDistance;
    }

    private void ReattachToCell(HexCell cell)
    {
        cell.AttachUnit(this);
    }
}
