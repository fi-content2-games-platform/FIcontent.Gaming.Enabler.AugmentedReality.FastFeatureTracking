using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour
{
    public float inSpeed = .7f;          // Speed that the screen fades to and from black.
    public float outSpeed = .7f;
    public float delay = 0f;

    private float startTime;
    private bool faded;


    void Awake()
    {
        // Set the texture so that it is the the size of the screen and covers it.
        guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        faded = false;
        Destroy(gameObject, 40f);
        startTime = Time.time;
    }

    void Update()
    {
        if (!faded)
            FadeToBlack();
        else
        {
            if (Time.time < startTime + delay)
                return;

            FadeToClear();
        }
    }


    void FadeToClear()
    {
        // Lerp the colour of the texture between itself and transparent.
        guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, inSpeed * Time.deltaTime);
    }


    void FadeToBlack()
    {
        // Lerp the colour of the texture between itself and black.
        guiTexture.color = Color.Lerp(guiTexture.color, Color.black, outSpeed * Time.deltaTime);
        if (guiTexture.color.a >= 0.95f)
            faded = true;
    }



}