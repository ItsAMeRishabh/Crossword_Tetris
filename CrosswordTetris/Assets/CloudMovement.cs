using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 spawnPoint;
    [SerializeField]
    private float cloudMoveSpeed;
    [SerializeField]
    private int offsetRange;

    private void FixedUpdate()
    {
        ObjectPoolerScript.objpoolerinstance.SpawnFromPool("Clouds", spawnPoint, Quaternion.identity);
        gameObject.transform.position += new Vector3(cloudMoveSpeed*Time.fixedDeltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "DeleteClouds")
        {
            //new Y offset
            int offset = Random.Range(-offsetRange, offsetRange);
            Vector3 newTransform = new Vector3(spawnPoint.x, offset, 0);

            gameObject.transform.position = new Vector3(spawnPoint.x, spawnPoint.y + offset);

            ObjectPoolerScript.objpoolerinstance.SendToPool(gameObject);
        }
    }
}
