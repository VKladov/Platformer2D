using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PostProcessVolume))]
public class CameraDeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private float _zoomDuration;
    [SerializeField] private float _colorEffectDuration;

    private PostProcessVolume _postProcess;

    private void Awake()
    {
        _postProcess = GetComponent<PostProcessVolume>();
    }

    public void PlayDeathEffect()
    {
        Time.timeScale = 0.3f;
        StartCoroutine(Zoom());
        StartCoroutine(ApplyColorEffect());
    }

    private IEnumerator Zoom()
    {
        float time = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = _target.transform.position + new Vector3(0, 0, -5);
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = new Quaternion(0, 0, 0.1f, 1);

        while (time < _zoomDuration)
        {
            time += Time.unscaledDeltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, time / _zoomDuration);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time / _zoomDuration);

            yield return null;
        }


        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1f;
    }

    private IEnumerator ApplyColorEffect()
    {
        float time = 0f;
        while (time < _zoomDuration)
        {
            time += Time.unscaledDeltaTime;
            _postProcess.weight = time / _zoomDuration;
            yield return null;
        }
    }
}
