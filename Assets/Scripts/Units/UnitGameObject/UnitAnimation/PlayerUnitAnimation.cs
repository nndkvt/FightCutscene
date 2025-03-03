using Unity.VisualScripting;
using UnityEngine;

public class PlayerUnitAnimation : UnitAnimation
{
    [SerializeField] private Transform _bombSpawnPoint;

    protected override void AttackAnimation(Transform target)
    {
        /*
        _attackParticle.SetActive(true);
        _attackParticle.GetComponent<ParticleSystem>().Play();
        */

        Instantiate(_attackParticle, _bombSpawnPoint.position, _bombSpawnPoint.rotation);
    }

    /*
    public void AttackAnimation(Transform enemyLocation)
    {
        StartCoroutine(SmoothRotateAndAttack(enemyLocation));
    }
    */
}
