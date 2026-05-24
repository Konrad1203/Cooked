using Unity.Cinemachine;
using System.Collections;
using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
    public float initialWaitTime = 0.5f;
    public float menuFadeDuration = 0.5f;
    public CinemachineCamera closeCamera;
    public CinemachineBrain brain;
    public CanvasGroup menuCanvasGroup;
    public GameObject firstSelectedButton;

    public void Start() {
        closeCamera.gameObject.SetActive(false);
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence() {
        yield return new WaitForSeconds(initialWaitTime);
        closeCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(brain.DefaultBlend.Time);
        foreach (Transform child in menuCanvasGroup.transform) child.gameObject.SetActive(true);
        StartCoroutine(FadeInMenu());
    }

    IEnumerator FadeInMenu() {
        float counter = 0f;
        while (counter < menuFadeDuration) {
            counter += Time.deltaTime;
            menuCanvasGroup.alpha = Mathf.Lerp(0, 1, counter / menuFadeDuration);
            yield return null;
        }
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
