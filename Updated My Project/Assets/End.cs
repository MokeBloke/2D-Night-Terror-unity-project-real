using UnityEngine;

public class End : MonoBehaviour
{
    ////Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // go to end screen
            SceneController.instance.EndScreen();
        }
    }
}
