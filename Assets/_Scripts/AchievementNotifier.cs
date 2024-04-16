using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementNotifier : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
