using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    [SerializeField] private float lifeSpan = 2.0f;
    private void OnEnable()
    {
        StartCoroutine(this.DisableDelay(this.lifeSpan));
    }

    private IEnumerator DisableDelay(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}