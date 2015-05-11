using UnityEngine;
using System.Collections;

// relies on: http://forum.unity3d.com/threads/12031-create-random-colors?p=84625&viewfull=1#post84625

public class ColorPicker : MonoBehaviour
{

    public bool useDefinedPosition = false;
    public int positionLeft = 0;
    public int positionTop = 0;

    // the solid texture which everything is compared against
    public Texture2D colorPicker;

    // the picker being displayed
    private Texture2D displayPicker;

    // the color that has been chosen
    public Color setColor;
    private Color lastSetColor;

    public bool useDefinedSize = false;
    public int textureWidth = 360;
    public int textureHeight = 120;

    private float saturationSlider = 0.0F;
    private Texture2D saturationTexture;

    private Texture2D styleTexture;

    public bool showPicker = false;

    void Awake()
    {
        if (!useDefinedPosition)
        {
            positionLeft = (Screen.width / 2) - (textureWidth / 2);
            positionTop = (Screen.height / 2) - (textureHeight / 2);
        }

        // if a default color picker texture hasn't been assigned, make one dynamically
        if (!colorPicker)
        {
            colorPicker = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            ColorHSV hsvColor;
            for (int i = 0; i < textureWidth; i++)
            {
                for (int j = 0; j < textureHeight; j++)
                {
                    hsvColor = new ColorHSV((float)i, (1.0f / j) * textureHeight, 1.0f);
                    colorPicker.SetPixel(i, j, hsvColor.ToColor());
                }
            }
        }
        colorPicker.Apply();
        displayPicker = colorPicker;

        if (!useDefinedSize)
        {
            textureWidth = colorPicker.width;
            textureHeight = colorPicker.height;
        }

        float v = 0.0F;
        float diff = 1.0f / textureHeight;
        saturationTexture = new Texture2D(20, textureHeight);
        for (int i = 0; i < saturationTexture.width; i++)
        {
            for (int j = 0; j < saturationTexture.height; j++)
            {
                saturationTexture.SetPixel(i, j, new Color(v, v, v));
                v += diff;
            }
            v = 0.0F;
        }
        saturationTexture.Apply();

        // small color picker box texture
        styleTexture = new Texture2D(1, 1);
        styleTexture.SetPixel(0, 0, setColor);
    }

    void OnGUI()
    {
        if (!showPicker) return;

        GUI.Box(new Rect(positionLeft - 3, positionTop - 3, textureWidth + 60, textureHeight + 60), "");

        if (GUI.RepeatButton(new Rect(positionLeft, positionTop, textureWidth, textureHeight), displayPicker))
        {
            int a = (int)Input.mousePosition.x;
            int b = Screen.height - (int)Input.mousePosition.y;

            setColor = displayPicker.GetPixel(a - positionLeft, -(b - positionTop));
            lastSetColor = setColor;
        }

        saturationSlider = GUI.VerticalSlider(new Rect(positionLeft + textureWidth + 3, positionTop, 10, textureHeight), saturationSlider, 1, -1);
        setColor = lastSetColor + new Color(saturationSlider, saturationSlider, saturationSlider);
        Camera.main.gameObject.SendMessage("SetBlimpColor", setColor, SendMessageOptions.DontRequireReceiver);
        
        GUI.Box(new Rect(positionLeft + textureWidth + 20, positionTop, 20, textureHeight), saturationTexture);
        


        if (GUI.Button(new Rect(positionLeft + textureWidth - 60, positionTop + textureHeight + 10, 60, 25), "Apply"))
        {
            setColor = styleTexture.GetPixel(0, 0);
           

            // hide picker
            showPicker = false;
        }

        // color display
        GUIStyle style = new GUIStyle();
        styleTexture.SetPixel(0, 0, setColor);
        styleTexture.Apply();

        style.normal.background = styleTexture;
        GUI.Box(new Rect(positionLeft + textureWidth + 10, positionTop + textureHeight + 10, 30, 30), new GUIContent(""), style);
    }

}

class ColorHSV : System.Object

{

    float h= 0.0f;
    float s = 0.0f;
    float v= 0.0f;
    float a= 0.0f;    

    /**
    * Construct without alpha (which defaults to 1)
    */

    public ColorHSV(float h, float s, float v)
    {

        this.h = h;
        this.s = s;
        this.v = v;
        this.a = 1.0f;
    }
    

    /**
    * Construct with alpha
    */
    public ColorHSV(float h, float s, float v, float a)
    {
        this.h = h;
        this.s = s;
        this.v = v;
        this.a = a;
    }    

    /**
    * Create from an RGBA color object
    */
    public ColorHSV(Color color)
    {
        float min = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
        float max = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
        float delta = max - min; 

        // value is our max color
        this.v = max; 

        // saturation is percent of max
        if(!Mathf.Approximately(max, 0))
            this.s = delta / max;
        else
        {
            // all colors are zero, no saturation and hue is undefined
            this.s = 0;
            this.h = -1;
            return;
        } 

        // grayscale image if min and max are the same
        if(Mathf.Approximately(min, max))
        {
            this.v = max;
            this.s = 0;
            this.h = -1;
            return;
        }        

        // hue depends which color is max (this creates a rainbow effect)
        if( color.r == max )
            this.h = ( color.g - color.b ) / delta;         // between yellow & magenta
        else if( color.g == max )
            this.h = 2 + ( color.b - color.r ) / delta; // between cyan & yellow
        else
            this.h = 4 + ( color.r - color.g ) / delta; // between magenta & cyan 

        // turn hue into 0-360 degrees
        this.h *= 60;
        if(this.h < 0 )
            this.h += 360;
    }
    
    /**
    * Return an RGBA color object
    */
    public Color ToColor()
    {
        // no saturation, we can return the value across the board (grayscale)
        if(this.s == 0 )
            return new Color(this.v, this.v, this.v, this.a); 

        // which chunk of the rainbow are we in?
        float sector = this.h / 60;        

        // split across the decimal (ie 3.87 into 3 and 0.87)
        int i = (int)Mathf.Floor(sector);
        float f = sector - i;
               

        float v = this.v;
        float p = v * ( 1 - s );
        float q = v * ( 1 - s * f );
        float t = v * ( 1 - s * ( 1 - f ) );        

        // build our rgb color
        Color color = new Color(0, 0, 0, this.a);
        
        switch(i)
        {
            case 0:
                color.r = v;
                color.g = t;
                color.b = p;
                break;

            case 1:
                color.r = q;
                color.g = v;
                color.b = p;
                break;

            case 2:
                color.r  = p;
                color.g  = v;
                color.b  = t;
                break;

            case 3:
                color.r  = p;
                color.g  = q;
                color.b  = v;
                break;

            case 4:
                color.r  = t;
                color.g  = p;
                color.b  = v;
                break;

            default:
                color.r  = v;
                color.g  = p;
                color.b  = q;
                break;
        }
                
        return color;
    }

    
    /**
    * Format nicely
    */

    public override string ToString()
    {
        return string.Format("h: {0:0.00}, s: {1:0.00}, v: {2:0.00}, a: {3:0.00}", h, s, v, a);
    }

}