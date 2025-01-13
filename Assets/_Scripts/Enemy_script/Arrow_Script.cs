using UnityEngine;

public class Arrow_Script : MonoBehaviour
{
    public float damage = 10f; // Sát thương gây ra

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với trụ
        if (collision.CompareTag("Turret"))
        {
            // Gây sát thương cho trụ
            _BaseTurret turret = collision.GetComponent<_BaseTurret>();
            if (turret != null)
            {
                turret.TakeDamage(damage);
                Destroy(gameObject);
            }


        }
    }
}
