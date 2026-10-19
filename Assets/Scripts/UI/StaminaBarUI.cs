using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private Image staminaBarImage;
    [SerializeField] protected CanvasGroup staminaBarCanvas;

    private float maxStamina = 100f;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private float waitBeforeFadeOut = 3f;

    private Coroutine fadeOutTimerCoroutine;

    private void OnEnable()
    {
        PlayerController.OnStaminaChange += StaminaBarChange;
    }

    private void OnDisable()
    {
        PlayerController.OnStaminaChange -= StaminaBarChange;
    }

    private void Start()
    {
        staminaBarCanvas.alpha = 0f;
    }

    private void Update()
    {
        FadeIn();
        FadeOut();
    }

    private void StaminaBarChange(float currentStamina)
    {
        staminaBarImage.fillAmount = currentStamina / maxStamina;
        if (staminaBarImage.fillAmount == 1f)
        {
            if(fadeOutTimerCoroutine == null)
            {
               fadeOutTimerCoroutine =  StartCoroutine(FadeOutTimer());
            }

            fadeIn = false;
        }
        else
        {
            if(fadeOutTimerCoroutine != null)
            {
                StopCoroutine(fadeOutTimerCoroutine);
                fadeOutTimerCoroutine = null;
            }

            fadeOut = false;
            fadeIn = true;
        }
    }

    private void FadeIn()
    {
        if (fadeIn)
        {
            if (staminaBarCanvas.alpha < 1f)
            {
                staminaBarCanvas.alpha += Time.deltaTime;
            }

            else
            {
                staminaBarCanvas.alpha = 1f;
                fadeIn = false;
            }
        }
    }

    private void FadeOut()
    {
        if (fadeOut)
        {
            if (staminaBarCanvas.alpha > 0f)
            {
                staminaBarCanvas.alpha -= Time.deltaTime;
            }

            else
            {
                staminaBarCanvas.alpha = 0f;
                fadeOut = false;
            }
        }
    }

    private IEnumerator FadeOutTimer()
    {
        yield return new WaitForSeconds(waitBeforeFadeOut);
        fadeOut = true;
        fadeOutTimerCoroutine = null;
    }
}
