using UnityEngine;
using System.Collections;

public class TweenMessage : MonoBehaviour
{
    public const int sALARM = 1;
    public const int sVICTORY = 2;

    UIPanel panel;
    private float duration = 2f;
    private string text;

    // Use this for initialization
    void Start()
    {
        panel = GetComponent<UIPanel>();

        GetComponentInChildren<UILabel>().text = text;

        Open();

        StartCoroutine(Close());
    }

    public void PlaySound(int x)
    {
        if (x == sALARM)
            Camera.main.SendMessage("PlaySound", AudioControl.AlarmSound);
    }

    public void SetText(string t)
    { this.text = t; }

    public void SetDuration(float d)
    { this.duration = d; }

    IEnumerator Close()
    {

        yield return new WaitForSeconds(duration);

        TweenScale.Begin(this.gameObject, .15f, new Vector3(1, 0.01f, 1));

        yield return new WaitForSeconds(.15f);

        TweenScale.Begin(this.gameObject, .1f, new Vector3(.01f, .01f, .01f));

        yield return new WaitForSeconds(.1f);

        Destroy(gameObject);
    }

    void Open()
    {
        TweenScale.Begin(this.gameObject, .15f, new Vector3(1, 1, 1));
        //tween.Toggle();
        Camera.main.SendMessage("PlaySound", AudioControl.MessageSound);
    }
}
