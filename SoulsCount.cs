using UnityEngine;
using TMPro;  // Add this namespace for TextMeshPro

public class SoulCounter : MonoBehaviour
{
    public TextMeshProUGUI soulCountText;  // Reference to UI TextMeshProUGUI to display the soul count
    public GameObject gate;               // Reference to the gate GameObject to enable

    private int soulCount = 0;

    void Start()
    {
        // Initialize the count and update the UI
        CountSouls();
        UpdateUI();
    }

    void Update()
    {
        // Continuously check if the count needs updating
        CountSouls();
        UpdateUI();
    }

    void CountSouls()
    {
        // Find all game objects with the "Soul" tag
        GameObject[] souls = GameObject.FindGameObjectsWithTag("Souls");

        // Update the soul count
        int newSoulCount = souls.Length;

        // If the count changes, update the soul count
        if (newSoulCount != soulCount)
        {
            soulCount = newSoulCount;
        }
    }

    void UpdateUI()
    {
        // Update the UI text with the soul count or "Rescue" if souls are 0
        if (soulCount == 0)
        {
            soulCountText.text = "Rescued";  // Change text to "Rescue"
            gate.SetActive(true);           // Enable the gate GameObject
        }
        else
        {
            soulCountText.text = soulCount + " Left";  // Display the number of souls
            gate.SetActive(false);          // Disable the gate GameObject
        }
    }
}
