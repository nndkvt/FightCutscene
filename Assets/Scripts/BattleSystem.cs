using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState 
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose,
}

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;

    private BattleState battleState;

    [SerializeField] private UnitGameObject _player;
    [SerializeField] private UnitGameObject _enemy;
    [SerializeField] private Button _playerTurnButton;

    private void Awake() 
    {
        Instance = this;
        
        battleState = BattleState.Start;
        _playerTurnButton.interactable = false;

        _playerTurnButton.onClick.AddListener(EnemyTurn);

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        Debug.Log("Start battle");

        yield return new WaitForSeconds(1.5f);

        PlayerTurn();
    }

    private void PlayerAnimation()
    {
        if (_player.CanAttackUnit(_enemy))
        {
            _playerTurnButton.interactable = false;

            StartCoroutine(_player.unitAnimation.SmoothRotateAndAttack(_enemy.unitAnimation.CurrentHexCell.transform));
        }
    }

    private void EnemyAnimation()
    {
        if (_enemy.CanAttackUnit(_player))
        {
            StartCoroutine(_enemy.unitAnimation.SmoothRotateAndAttack(_player.unitAnimation.CurrentHexCell.transform));
        }
        else
        {
            PlayerTurn();
        }
    }

    public void PlayerAttackVoid()
    {
        StartCoroutine(PlayerAttack());
    }

    public void EnemyAttackVoid()
    {
        StartCoroutine(EnemyAttack());
    }

    private IEnumerator PlayerAttack()
    {
        if (_enemy.TakeDamage(_player.unitInfo.attackValue))
        {
            yield return new WaitForSeconds(1.5f);

            PlayerTurn();
        }
        else
        {
            _playerTurnButton.interactable = false;

            yield return new WaitForSeconds(1.5f);

            EnemyTurn();
        }
    }

    private IEnumerator EnemyAttack()
    {
        if (_enemy.CanAttackUnit(_player))
        {
            if (_player.TakeDamage(_enemy.unitInfo.attackValue))
            {
                yield return new WaitForSeconds(1.5f);

                EnemyTurn();
            }
            else
            {
                yield return new WaitForSeconds(1.5f);

                PlayerTurn();
            }
        }
        else
        {
            yield return new WaitForSeconds(1.5f);

            PlayerTurn();
        }
    }

    public void PlayerTurn()
    {
        Debug.Log("Player turn");

        battleState = BattleState.PlayerTurn;
        _playerTurnButton.interactable = true;
    }

    public void EnemyTurn()
    {
        Debug.Log("Enemy turn");

        battleState = BattleState.EnemyTurn;
        _playerTurnButton.interactable = false;

        EnemyAnimation();
    }

    public void OnAttackButton()
    {
        if (battleState != BattleState.PlayerTurn)
        {
            return;
        }

        PlayerAnimation();
    }

    /*
    /// Not used in the demo

    private void PlayerVictory()
    {
        battleState = BattleState.Win;
        Debug.Log("Player win");
        _playerTurnButton.interactable = false;
    }

    private void EnemyVictory()
    {
        battleState = BattleState.Lose;
        Debug.Log("Enemy win");
    }
    */

    public bool IsPlayerInput()
    {
        return _playerTurnButton.interactable;
    }
}
