using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
   public static RespawnController instance;
   public Transform respawnPoint;
    private void Awake()
    {
        instance = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = respawnPoint.position;   
        }
    }
    public void Reborn()
    {

        if (respawnPoint != null)
        {
            Debug.Log("Respawning player...");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = respawnPoint.position;
            }
            else
            {
                Debug.LogWarning("Player not found for respawn!");
            }
        }
        else
        {
            Debug.LogWarning("Respawn point not set!");
        }

    }
}
