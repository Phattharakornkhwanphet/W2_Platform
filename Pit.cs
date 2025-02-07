using UnityEngine;
using Cinemachine; // For Cinemachine control

public class DeadPit : MonoBehaviour
{
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInImage fade = collision.GetComponent<FadeInImage>();
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fell into DeadPit!");

            // Disable CinemachineBrain on the Main Camera
            CinemachineBrain camBrain = Camera.main.GetComponent<CinemachineBrain>();
            if (camBrain != null)
            {
                camBrain.enabled = false;
                Debug.Log("CinemachineBrain Disabled!");
            }

            // Set Player HP to 0
            Player playerHealth = collision.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.SetHP(0); // Call a method to trigger death
                fade.fadeInTriggered = true;
                Debug.Log("Player HP set to 0!");
            }
        }
    }
}
