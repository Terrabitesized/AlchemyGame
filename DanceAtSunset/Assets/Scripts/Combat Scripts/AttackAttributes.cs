using System.Collections;
using UnityEngine;

public class AttackAttributes : MonoBehaviour
{
    private IDamagable player;
    public int damage;
    public float damageCooldown;

    private bool isDamaging;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isDamaging)
        {
            player = other.GetComponent<IDamagable>();
            if (player != null)
            {
                StartCoroutine(AtkPlayer());
            }
        }
    }

    private IEnumerator AtkPlayer()
    {
        isDamaging = true;
        player.takeDamage(damage);
        yield return new WaitForSeconds(damageCooldown);
        isDamaging = false;
    }
}