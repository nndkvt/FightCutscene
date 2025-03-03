using UnityEngine;

public class Bomb : DestroyAfterTime
{
    public float attackValue = 10;

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.GetComponent<UnitGameObject>())
        {
            other.gameObject.GetComponent<UnitGameObject>().TakeDamage(attackValue);
        }
    }
}
