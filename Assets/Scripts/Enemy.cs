using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    public int hp = 50;

    [Header("Detection Settings")]
    public float detectionRange = 10f;
    public float beamThickness = 0.3f;
    public Vector3 eyeOffset = new Vector3(0, 1.2f, 0);

    [Header("Layer Setup")]
    public LayerMask detectionLayers;

    void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Vector3 origin = transform.position + eyeOffset;

        Vector3 direction = transform.right;

        if (Physics.Raycast(origin, direction, out RaycastHit visualHit, detectionRange, detectionLayers))
        {
            Debug.DrawRay(origin, direction * visualHit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(origin, direction * detectionRange, Color.blue);
        }

        if (Physics.SphereCast(origin, beamThickness, direction, out RaycastHit hit, detectionRange, detectionLayers))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Game Over");
                Time.timeScale = 0f;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " Enemy is dead");
        Destroy(gameObject);
    }
}