using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenConsole : MonoBehaviour
{
    public static ScreenConsole instance { get; private set; }

    private TextMeshProUGUI textField;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { DestroyImmediate(this); }
    }

    private void Start()
    {
        textField = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        Reset();
    }

    public void Display(string message)
    {
        Display(message, Mathf.PI/2);
    }

    public void Display(string message, float duration)
    {
        Reset();
        Debug.Log($"ScreenConsole will now display: {message}");
        StartCoroutine(DisplayFor(message, duration));
    }

    private void Reset()
    {
        StopAllCoroutines();
        textField.text = "";
        canvasGroup.alpha = 1;
    }

    private IEnumerator DisplayFor(string message, float duration)
    {
        textField.text = message;

        float timePassed = 0;
        while (timePassed < duration)
        {
            canvasGroup.alpha = (Mathf.Sin(timePassed * Mathf.PI) + 1) * 0.5f;
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Reset();
    }
}
