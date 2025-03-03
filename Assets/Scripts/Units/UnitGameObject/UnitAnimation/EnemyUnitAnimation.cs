using System.Collections;
using UnityEngine;

public class EnemyUnitAnimation : UnitAnimation
{
    [SerializeField] private float _attackTime;
    [SerializeField] private float _cooldownTime;

    protected override void AttackAnimation(Transform target)
    {
        _attackParticle.SetActive(true);

        /// <summary>
        /// CountAttackTime - turn-based combat
        /// NonstopAttack - demo
        /// </summary>

        //StartCoroutine(CountAttackTime());
        StartCoroutine(NonstopAttack(target));
    }

    private IEnumerator NonstopAttack(Transform target)
    {
        yield return new WaitForSeconds(_attackTime);

        _attackParticle.SetActive(false);

        yield return new WaitForSeconds(_cooldownTime);

        StartCoroutine(SmoothRotateAndAttack(target));
    }

    private IEnumerator CountAttackTime()
    {
        yield return new WaitForSeconds(_attackTime);

        _attackParticle.SetActive(false);

        BattleSystem.Instance.PlayerTurn();
    }
}
