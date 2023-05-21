using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ButtonHider : MonoBehaviour
{

    public Image buttonImage;
    public TextMeshProUGUI buttonText;

    public void Hide(float fadeDuration){
        buttonImage.CrossFadeAlpha(0f, fadeDuration, true);
        buttonText.CrossFadeAlpha(0f, fadeDuration, true);
    }

    public void Show(float fadeDuration){
        buttonImage.CrossFadeAlpha(1f, fadeDuration, true);
        buttonText.CrossFadeAlpha(1f, fadeDuration, true);
    }
}
