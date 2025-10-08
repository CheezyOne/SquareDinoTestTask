using UnityEngine;
using Mirror;

public class CubeSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _cubeToSpawn;
    [SerializeField] private float _spawnDistance;

    private void Update()
    {
        if (!isOwned)
            return;

        if(Input.GetKeyDown(KeyCode.F))
            SpawnCube();
    }

    [Command]
    private void SpawnCube()
    {
        GameObject newCube = Instantiate(_cubeToSpawn, transform.position + transform.forward * _spawnDistance, Quaternion.identity);
        NetworkServer.Spawn(newCube);
    }
}