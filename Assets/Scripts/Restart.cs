using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] float _delay;

    public void RestartLevel()
    {
        StartCoroutine(RestartWithDelay(_delay));
    }

    private IEnumerator RestartWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1f;
    }
}
