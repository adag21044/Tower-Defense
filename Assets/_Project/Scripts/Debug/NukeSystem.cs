using UnityEngine;

public class NukeSystem : MonoBehaviour
{
    [SerializeField] private WaveSpawner waveSpawner;

    private void Update()
    {
        // Nuke key
        if (Input.GetKeyDown(KeyCode.N))
        {
            NukeEnemies();
        }
    }

    private void NukeEnemies()
    {
        Debug.Log("Nuke activated! All enemies destroyed.");
        ToastManager.Instance.ShowToast("Nuke activated! All enemies destroyed.");
        
        foreach (var enemy in waveSpawner.enemiesInScene)
        {
            Destroy(enemy);
        }      
    }
}