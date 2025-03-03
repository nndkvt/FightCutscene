using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UnitGameObject))]
public class EnemyIgnoreBattleSystem : MonoBehaviour
{
    [SerializeField] private UnitGameObject _player;

    private UnitGameObject _enemy;

    private void Awake() 
    {
        _enemy = GetComponent<UnitGameObject>();
        
        StartCoroutine(WaitAndAttack());
    }

    private IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(_enemy.unitAnimation.SmoothRotateAndAttack(_player.transform));
    }
}
