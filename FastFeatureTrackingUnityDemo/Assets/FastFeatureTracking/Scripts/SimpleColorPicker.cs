using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class SimpleColorPicker : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
   [DllImport("__Internal")]
    private static extern void _peekEnableSamples( bool shouldCopy );
	
    [DllImport("__Internal")]
    private static extern float _peekAverageR();

	[DllImport("__Internal")]
    private static extern float _peekAverageG();
	
	[DllImport("__Internal")]
    private static extern float _peekAverageB();
#endif

	public int radius = 8;
	public SkyePeek skyePeek;
	public bool updateColor = true;
	private int fX, tX, fY, tY;
	private Texture2D texture;
	private Color color;
	private bool init = false;
	private Texture2D smallTex;
	private GUIStyle style;
	private readonly Rect centerRect = new Rect (Screen.width / 2 - 20, Screen.height / 2 - 20, 40, 40);
	
	void Awake ()
	{
		smallTex = new Texture2D (1, 1);
	}

	// Use this for initialization
	void Start ()
	{
		if (skyePeek == null)
			Debug.LogError ("please assign the object using the Skye Peek component in the inspector", this);

		Invoke ("Init", 2f);


	}

	void Init ()
	{
		texture = skyePeek.videoTexture;

		fX = texture.width / 2 - radius / 2;
		tX = texture.width / 2 + radius / 2;
		fY = texture.height / 2 - radius / 2;
		tY = texture.height / 2 + radius / 2;

		init = true;

		smallTex = new Texture2D (1, 1);        
		smallTex.wrapMode = TextureWrapMode.Repeat;
		smallTex.Apply ();

		style = new GUIStyle ();
		style.normal.background = smallTex;
		
#if UNITY_IOS && !UNITY_EDITOR
		_peekEnableSamples(true);
#endif
	}

	// Update is called once per frame
	void Update ()
	{
		if (!init || !updateColor)
			return;
		
		float r = 0, g = 0, b = 0;

#if UNITY_IOS && !UNITY_EDITOR
		r=_peekAverageR();
		g=_peekAverageG();
		b=_peekAverageB();
#else
		for (int y = fY; y < tY; y++)
			for (int x = fX; x < tX; x++) {
				r += texture.GetPixel (x, y).r;
				g += texture.GetPixel (x, y).g;
				b += texture.GetPixel (x, y).b;
			}

		int tot = (tY - fY) * (tX - fX);
		r /= tot;
		g /= tot;
		b /= tot;
#endif
		
		color = new Color (r, g, b, 1);

		skyePeek.SetBlimpColor (color);
	}

	void OnGUI ()
	{
		if (!init)
			return;
		
		if (color.grayscale > 0.0f) {
			smallTex.SetPixel (0, 0, color);
			smallTex.Apply ();
			GUI.DrawTexture (centerRect, smallTex, ScaleMode.StretchToFill);
		}

	}
	
	void OnDisable ()
	{
#if UNITY_IOS && !UNITY_EDITOR
		_peekEnableSamples(false);
#endif
	}
	
	void OnEnable ()
	{
#if UNITY_IOS && !UNITY_EDITOR
		_peekEnableSamples(true);
#endif
	}
}

