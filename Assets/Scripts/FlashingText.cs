using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour
{
    private Text text;
    private Color originalColor;
    private Color transparentColor;
    public float fadeTime;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();

        originalColor = text.color;
        transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
        //fadeTime = 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        text.color = Color.Lerp(originalColor, transparentColor, Mathf.PingPong(Time.time * fadeTime, 1));
    }
}
