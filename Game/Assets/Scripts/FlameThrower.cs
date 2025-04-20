using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    public LayerMask enemyLayer;

    private int damage;
    private float interval;
    private Coroutine damageCoroutine;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public void StartDamaging(int dmg, float inter)
    {
        damage = dmg;
        interval = inter;
        damageCoroutine = StartCoroutine(DamageOverTime());
    }

    public void StopDamaging()
    {
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        enemiesInRange.Clear();
    }

    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy != null)
                {
                    enemy.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0 && !enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
