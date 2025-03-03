using System;
using System.Collections;
using UnityEngine;

public abstract class UnitAnimation : MonoBehaviour
{
    [SerializeField] protected GameObject _attackParticle;

    public HexCell CurrentHexCell { get ; set ; }

    public Action<HexCell> OnUnitFinishedMoving;

    protected readonly Vector3 spawnOffset = Vector3.up * 2f;
    protected float _moveSpeed = 40.0f;
    protected float _rotateSpeed = 150.0f;

    public IEnumerator SmoothRotateAndAttack(Transform target)
    {
        //StopAllCoroutines();

        Vector3 targetDirection = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation , Time.deltaTime * _rotateSpeed);
            yield return null;
        }

        AttackAnimation(target);
    }

    public IEnumerator SmoothRotateAndMoveToCell(HexCell moveCell)
    {
        StopAllCoroutines();

        Vector3 targetDirection = (moveCell.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation , Time.deltaTime * _rotateSpeed);
            yield return null;
        }

        StartCoroutine(MoveCharacterSmooth(moveCell));
    }

    public IEnumerator MoveCharacterSmooth(HexCell cell)
    {
        CurrentHexCell?.DetachUnit();

        while (Mathf.Abs((cell.transform.position + spawnOffset - transform.position).magnitude) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                                                     cell.transform.position + spawnOffset, 
                                                     Time.deltaTime * _moveSpeed);

            yield return null;
        }

        CurrentHexCell = cell;
        OnUnitFinishedMoving?.Invoke(cell);

        Debug.Log("Character stop");
    }

    public void MoveCharacter(HexCell cell)
    {
        CurrentHexCell?.DetachUnit();

        transform.position = cell.transform.position + spawnOffset;

        CurrentHexCell = cell;
        
        OnUnitFinishedMoving?.Invoke(cell);
    }

    protected abstract void AttackAnimation(Transform target);
}
