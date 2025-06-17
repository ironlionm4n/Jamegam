using Parts;
using UnityEngine;

public class PartCurrency : MonoBehaviour
{
    [SerializeField] private float flySpeed = 2f;
    [SerializeField] float currencyValue = 1f;
    [SerializeField] float waveFrequency = 5f;
    [SerializeField] float waveAmplitude = 0.5f;
    private GameObject playerObject;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards player in a wavy motion in 2D space left and right
        if (playerObject != null)
        {
            Vector3 direction = (playerObject.transform.position - transform.position).normalized;

            // Calculate the wave offset
            float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
            Vector3 offset = new Vector3(waveOffset, 0, 0);

            // Move towards the player with a wavy motion
            transform.position += (direction + offset) * (Time.deltaTime * flySpeed);
            flySpeed += Time.deltaTime * 0.1f;
            
            // Rotate the visual child to add a spinning effect
            if (transform.childCount > 0)
            {
                var leftOrRight = direction.x > 0 ? 1 : -1; // Determine the direction to spin
                var visualChild = transform.GetChild(0);
                var angle = leftOrRight * 90f; // Set the angle based on direction
                visualChild.Rotate(Vector3.forward, angle * Time.deltaTime); // Rotate around the Z-axis
            }
            
            if(Vector2.Distance(gameObject.transform.position, playerObject.transform.position) < 0.33f)
            {
                playerObject.GetComponent<Economy>().CollectPartCurrency(currencyValue);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Player not found. Ensure the player has the 'Player' tag.");
        }
    }
}