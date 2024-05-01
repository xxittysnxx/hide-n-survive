using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : MonoBehaviour
{
    public float speedBoost = 10f;
    public bool respawn;
    public float respawnTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Collision detected");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision detected");
        // For testing, compare Capsule tag. For game, compare Clone tag
        // if (other.gameObject.tag == "Player")
        if (other.transform.CompareTag("Capsule") || 
            other.transform.CompareTag("Clone") || 
            other.transform.CompareTag("Character") ||
            other.transform.CompareTag("Model") ||
            other.gameObject.tag == "Capsule" || 
            other.gameObject.tag == "Clone" ||
            other.gameObject.tag == "Character" ||
            other.gameObject.tag == "Model" ||
            other.transform.CompareTag("Player") ||
            other.gameObject.tag == "Player"
            )
        {
            // For test, change color to red
            gameObject.GetComponent<Renderer>().material.color = Color.red;

            // Disable mesh
            // gameObject.GetComponent<MeshRenderer>().enabled = false;

            // Disable collider
            // gameObject.GetComponent<Collider>().enabled = false;

            // Change speed on CharacterMovement.cs of Character object (Parent objects of Capsule and Clone)
            GameObject.Find("Character").GetComponent<CharacterMovement>().speed += speedBoost;
            // other.gameObject.GetComponent<CharacterMovement>().speed += speedBoost;

            if (respawn)
            {
                Invoke("Respawn", respawnTime);
            }
        }
    }

    void Respawn()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }
}