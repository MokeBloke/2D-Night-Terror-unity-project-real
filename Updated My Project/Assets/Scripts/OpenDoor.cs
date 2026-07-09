using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public bool isHidden = true;

    [Tooltip("If set, the door watches this specific GameObject (usually an enemy). If null, a GameObject with Enemy tag will be auto-found at Start.")]
    public GameObject enemyToWatch;

    [Tooltip("Tag used to find an enemy when 'enemyToWatch' is not assigned.")]
    public string enemyTag = "Enemy";

    [Tooltip("If true the door will be destroyed when ALL enemies with the given tag are gone. If false it will be destroyed when the single watched enemy is gone.")]
    public bool destroyWhenAllEnemiesDead = false;

    void Start()
    {
        if (enemyToWatch == null && !destroyWhenAllEnemiesDead)
        {
            var found = GameObject.FindWithTag(enemyTag);
            if (found != null)
                enemyToWatch = found;
        }
    }

    void Update()
    {
        if (destroyWhenAllEnemiesDead)
        {
            // If no GameObjects with the enemy tag remain, destroy this door.
            var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            if (enemies == null || enemies.Length == 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // If the specific watched enemy has been destroyed, destroy this door.
            // Note: Unity returns 'true' for (enemyToWatch == null) when the object has been destroyed.
            if (enemyToWatch != null)
                return;

            // enemyToWatch is null (either not found or destroyed) -> destroy door
            Destroy(gameObject);
        }
    }
}
