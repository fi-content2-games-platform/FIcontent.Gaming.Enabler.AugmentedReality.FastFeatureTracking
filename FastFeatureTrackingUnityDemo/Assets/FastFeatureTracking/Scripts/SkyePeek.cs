/*==============================================================================
Copyright (c) 2013 Disney Research
All Rights Reserved.
==============================================================================*/

// debug display 
#define SHOW_DEBUG 

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


/// <summary>
/// Defines the AR exposure modes
/// </summary>
public enum ARExposureMode
{
	Locked = 0,
	AutoExpose = 1,
	ContinuousAutoExposure = 2,
}

/// <summary>
/// Defines the AR focus modes
/// </summary>
public enum ARFocusMode
{
	Locked = 0,
	AutoFocus = 1,
	ContinuousAutoFocus = 2,
}

/// <summary>
/// Defines the AR debug modes
/// </summary>
public enum ARDebugMode
{
	Off = 0,
	Gray = 1,
	Distance = 2,
	CentroidX = 3,
	CentroidY = 4,
	Centroid0X = 5,
	Centroid0Y = 6,
	Centroid1X = 7,
	Centroid1Y = 8,
	KMeans = 9
}

// Defines the AR feature tracking modes
public enum ARFeatureTrackingMode
{
	kMeansCPU = 0,
	MomentGPU = 1
}

/// <summary>
/// SkyePeek Plugin Interface
/// </summary>
public class SkyePeek : MonoBehaviour
{
	#region PEEK_IMPORTS	
	
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
	private static extern void _peekStart(bool useFront, int texId);
	
    [DllImport ("__Internal")]
	private static extern void _peekReadPixels(int dstTexId, int srcTexId,  int width, int height, IntPtr data);

    [DllImport ("__Internal")]
	private static extern void _peekEnd();
	
    [DllImport("__Internal")]
    private static extern void _peekSetFocusMode( int focusMode );
	
    [DllImport("__Internal")]
    private static extern void _peekSetExposureMode( int exposureMode );
	
    [DllImport("__Internal")]
    private static extern float _peekHalf2Float( short sum );
	
#else
    /// <summary>
    /// Starts the camera capture
    /// </summary>
    private  void _peekStart(bool useFront, int texId) {}

    /// <summary>
    /// Stops the camera capture
    /// </summary>
	private  void _peekEnd() {}

    /// <summary>
    /// Sets the camera focus mode
    /// </summary>
	private  void _peekSetFocusMode(int focusMode ) {}

    /// <summary>
    /// Sets the camera exposure mode
    /// </summary>
	private  void _peekSetExposureMode(int exposureMode ) {}
#endif

    /// <summary>
    ///  Writes a RenderTexture into a Texture2D
    /// </summary>
	private void TextureFromRT(Texture2D dstTex, RenderTexture srcTex)
	{
		int width=dstTex.width;
		int height=dstTex.height;
		
#if UNITY_IOS && !UNITY_EDITOR
		if (handlePx.IsAllocated) 
			handlePx.Free();
		
		handlePx = GCHandle.Alloc (curTexData, GCHandleType.Pinned);
		_peekReadPixels(dstTex.GetNativeTextureID(), srcTex.GetNativeTextureID(), width, height, handlePx.AddrOfPinnedObject());
#else
		RenderTexture.active = srcTex;
		dstTex.ReadPixels(new Rect(0,0, width, height), 0,0, false);        
		//dstTex.Apply(); only need if rendering with this tex
#endif
		curTex=dstTex;
	}

    /// <summary>
    ///  Accumulates sum and vol from texture
    /// </summary>
	private void AccumulateFromTex(ref float sum, ref float vol, int x, int y)
	{
#if UNITY_IOS && !UNITY_EDITOR
		int idx=(y*curTex.width + x)*2;
		sum += _peekHalf2Float(curTexData[idx++]);
		vol += _peekHalf2Float(curTexData[idx]);
#else
		Color rg=curTex.GetPixel(x,y);
		sum += rg.r;
		vol += rg.g;
#endif	
	}

    /// <summary>
    ///  Frees handles in iOS
    /// </summary>
	private void DoneAccumulation()
	{
#if UNITY_IOS && !UNITY_EDITOR
		if (handlePx.IsAllocated) 
			handlePx.Free();
#endif
	}
	
	#endregion // PEEK_IMPORTS

    #region PUBLIC_METHODS

    /// <summary>
    ///  Disables AR debugging
    /// </summary>
    public void DisableDebug(bool disable)
    {
        debugMode = ARDebugMode.Off;
    }

    /// <summary>
    ///  Sets the blimp color
    /// </summary>
    public void SetBlimpColor(Color color)
    {
        this.refColor = color;
    }

    /// <summary>
    ///  Starts the camera capture and returns a Texture2D that will have the camera output as it's content
    /// </summary> 
    public void startCameraCapture( bool useFrontCameraIfAvailable )
    {
        // Create texture that will be updated in the plugin code/webcam
		MakeVideoTexture();
		
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_peekStart( useFrontCameraIfAvailable, videoTexture.GetNativeTextureID() );
	}
	
    /// <summary>
    ///  Stops the camera capture
    /// </summary> 
    public  void stopCameraCapture()
    {
		_peekEnd();
    }
	 
    /// <summary>
    /// Sets the exposure mode.  Capture must be started for this to work!
    /// </summary> 
    public  void setExposureMode( ARExposureMode mode )
    {
		_peekSetExposureMode( (int)mode );
    }
	
    /// <summary>
    /// Sets the focus mode.  Capture must be started for this to work!
    /// </summary> 
    public  void setFocusMode( ARFocusMode mode )
    {
		_peekSetFocusMode( (int)mode );
    }
	
    /// <summary>
    ///  Updates the materials UV offset to accommodate the texture being placed into the next biggest power of 2 container
    /// </summary> 
	public  void updateMaterialUVScaleForTexture( Material material, Texture2D texture )
	{
		Vector2 textureOffset = new Vector2( (float)texture.width / (float)nearestPowerOfTwo( texture.width ), (float)texture.height / (float)nearestPowerOfTwo( texture.height ) );
		material.mainTextureScale = textureOffset;
	}

    #endregion // PUBLIC_METHODS
	
	#region PUBLIC_MEMBERS
	
	public bool useFrontCamera = false;
	public ARFocusMode focusMode = ARFocusMode.ContinuousAutoFocus;
	public ARExposureMode exposureMode = ARExposureMode.ContinuousAutoExposure;
	
	public int videoWidth = 640;
	public int videoHeight = 480;
	public int videoFPS = 60;
	public int numPassSamples = 8;
	public float offsetScale = 1.0f;
	public Color refColor;

	public GameObject backdropObj;
	
    public Transform blimp;

	public float baseDistance = 40;
    public float initialBlimpDistance;
	
#if SHOW_DEBUG
	public ARDebugMode debugMode = ARDebugMode.Gray;
#endif
	
	public ARFeatureTrackingMode trackingMode = ARFeatureTrackingMode.MomentGPU;
	
	public Shader centroid0XShader;
	public Shader centroid1XShader;
	public Shader centroid0YShader;
	public Shader centroid1YShader;
	
#if SHOW_DEBUG
	public Shader grayShader;
	public Shader distShader;
#endif

	#endregion
	
    #region PRIVATE_MEMBERS
	[HideInInspector] // Hides var below
	public Texture2D videoTexture;
	
	private RenderTexture centTex0X=null;
	private RenderTexture centTex1X=null;
	private RenderTexture centTex0Y=null;
	private RenderTexture centTex1Y=null;
	
	
#if SHOW_DEBUG
	private RenderTexture debugRTex=null;
	private Material grayMaterial=null;
	private Material distMaterial=null;
	private Texture2D debugTex=null;
	private GameObject debugObj;
#endif
	
	private Texture2D centTexX=null;
	private Texture2D centTexY=null;
	
	private Texture2D curTex; // used to abstract texture data access
	private short[] curTexData= new short[256*256*2];
	GCHandle handlePx;
	
	private Material cent0XMaterial=null;
	private Material cent1XMaterial=null;
	private Material cent0YMaterial=null;
	private Material cent1YMaterial=null;
	
	private Vector4 offsets0;
	private Vector4 offsets1;

	private string deviceName;
	private WebCamTexture wct;
	
    private Pixel oldA, oldB, oldC;
    private Vector3 initialScreenPos;
	
	private Vector2 trackingCenter;
	private float trackingScale;
	private bool trackingFound=false;
	private Vector3 lastCenter= new Vector3(0,0,0);

#if UNITY_IOS && !UNITY_EDITOR
	private static iPhoneGeneration[] unsupportedDevices = 
	{
        iPhoneGeneration.iPad1Gen,
        iPhoneGeneration.iPhone,
        iPhoneGeneration.iPhone3G,
        iPhoneGeneration.iPhone3GS,
        iPhoneGeneration.iPodTouch1Gen,
        iPhoneGeneration.iPodTouch2Gen,
        iPhoneGeneration.iPodTouch3Gen,
		iPhoneGeneration.iPhone4,
        iPhoneGeneration.iPodTouch4Gen
    };
#endif
	/// <summary>
	/// Defines the Pixel struct
	/// </summary>
    struct Pixel
    {
        public int x, y;

        public float DistSquare(Pixel other)
        {
            return (x - other.x) * (x - other.x) + (y - other.y) * (y - other.y);
        }

        public Vector2 toVector2()
        {
            return new Vector2(x, y);
        }
    };

    /// <summary>
    /// Calculates the nearest power of two for a number
    /// </summary>    
	private  int nearestPowerOfTwo( float number )
	{
		int n = 1;
		
		while( n < number )
			n <<= 1;

		return n;
	}

    /// <summary>
    /// Sets up debugging objects
    /// </summary>    
	private void DebugSetup()
	{
#if SHOW_DEBUG

#if UNITY_IOS && !UNITY_EDITOR
		//debug.transform.Rotate(270,0,0);
		//debug.transform.localScale=new Vector3(-1,1,1);
#else
		//debugObj.transform.Rotate(90,180,0); 
#endif

		debugTex = new Texture2D(videoWidth, videoHeight, TextureFormat.RGBA32, false);
		debugTex.filterMode = FilterMode.Point;
		
		debugObj = new GameObject("Debug");        
        debugObj.AddComponent("GUITexture");
		debugObj.guiTexture.texture = debugTex;
        debugObj.transform.localScale = new Vector3(.06f, .3f, 0f); 
        debugObj.transform.position = new Vector3(.06f, .2f, 0f); 
		debugObj.guiTexture.pixelInset = new Rect(8.0f,8.0f,videoWidth/2,videoHeight/2);
		
		//grayShader = Shader.Find("Custom/BGRA2Gray");
		grayMaterial = new Material(grayShader);
		
		//distShader = Shader.Find ("Custom/ColorDistance");
		distMaterial = new Material(distShader);

		debugRTex = new RenderTexture(videoWidth,videoHeight,-1,RenderTextureFormat.ARGB32);
		debugRTex.filterMode = FilterMode.Point;

		Debug.Log(initialScreenPos);
        Debug.Log(initialBlimpDistance);
		Debug.Log ("Screen:" + Screen.width + "," + Screen.height + " Video:" + videoWidth + "," + videoHeight + " " + videoFPS );
#endif
	}
	
	private void DebugUpdate()
	{
		if (debugMode==ARDebugMode.Off)				debugObj.SetActive(false);
		else
		{
			if (!debugObj.activeSelf)					debugObj.SetActive(true);
				
			switch (debugMode)
			{
			case ARDebugMode.KMeans					:	debugObj.guiTexture.texture = debugTex;			break;
			case ARDebugMode.CentroidX				:	debugObj.guiTexture.texture = centTexX;			break;
			case ARDebugMode.Centroid0X				:	debugObj.guiTexture.texture = centTex0X;		break;
			case ARDebugMode.Centroid1X				:	debugObj.guiTexture.texture = centTex1X;		break;
			case ARDebugMode.CentroidY 				:	debugObj.guiTexture.texture = centTexY;			break;
			case ARDebugMode.Centroid0Y 			:	debugObj.guiTexture.texture = centTex0Y;		break;
			case ARDebugMode.Centroid1Y 			:	debugObj.guiTexture.texture = centTex1Y;		break;
			case ARDebugMode.Gray					:	debugObj.guiTexture.texture = debugRTex;		break;
			case ARDebugMode.Distance				:	debugObj.guiTexture.texture = debugRTex;		break;
			default : debugObj.guiTexture.texture = videoTexture;			break;
			}
		}		
	}

    /// <summary>
    /// Sets up a Texture2D for video capturing
    /// </summary>    
	private void MakeVideoTexture()
	{
#if UNITY_IOS && !UNITY_EDITOR
		videoTexture = new Texture2D( videoWidth, videoHeight, TextureFormat.BGRA32, false);
		backdropObj.guiTexture.texture = videoTexture;
		backdropObj.transform.localScale=new Vector3(1,-1,1);		
#else
		videoTexture = new Texture2D( videoWidth, videoHeight, TextureFormat.RGBA32, false);
		

		WebCamDevice[] cameras = WebCamTexture.devices;
        if ((cameras != null) && (cameras.Length > 0))
        {			
			deviceName = cameras[cameras.Length-1].name;
			wct = new WebCamTexture(deviceName, videoWidth, videoHeight, videoFPS);
			backdropObj.guiTexture.texture = wct;
			wct.Play();
			Debug.Log("Created web cam texture");
		}
		else
			Debug.Log("No cameras " + cameras.Length);

#endif // UNITY_IOS/EDITOR
		videoTexture.filterMode = FilterMode.Point;
	}

    /// <summary>
    /// For PC (not iphone): ensure that webcam image is in video texture
    /// </summary>   
	void UpdateVideoTexture()
	{
#if UNITY_EDITOR || !UNITY_IOS
		if (wct!=null && wct.isPlaying && wct.didUpdateThisFrame)
		{
			Color[] textureData = wct.GetPixels();
	 
			if (textureData!=null)
			{
	    		videoTexture.SetPixels(textureData);
	    		videoTexture.Apply();
			}
		}
#endif
	}
	
    /// <summary>
    /// Creates the centroid texture using grayscale image and scaling down
    /// </summary>  
	void MakeCentroidTexture()
	{
#if UNITY_IOS && !UNITY_EDITOR
		foreach (iPhoneGeneration gen in unsupportedDevices)
		{
			if (gen == iPhone.generation)
			{
				return;
			}
		}
#endif		
		// these have to be manually assigned for the build to include them...
		//centroid0XShader=Shader.Find("Custom/Centroid0X");
		//centroid0YShader=Shader.Find("Custom/Centroid0Y");
		//centroid1XShader=Shader.Find("Custom/Centroid1X");
		//centroid1YShader=Shader.Find("Custom/Centroid1Y");
		cent0XMaterial=new Material(centroid0XShader);
		cent1XMaterial=new Material(centroid1XShader);
		cent0YMaterial=new Material(centroid0YShader);
		cent1YMaterial=new Material(centroid1YShader);
		
		int passXSamples = numPassSamples;
		int passYSamples = numPassSamples;
		//int totalPassSamples = passXSamples*passYSamples;
		int passWidth = videoWidth/passXSamples;
		int passHeight = videoHeight/passYSamples;
		Debug.Log("Centroid texture axes pass 0:" + passWidth + "," + passHeight);
		
		offsets0 = new Vector4(offsetScale/videoWidth,offsetScale/videoHeight,offsetScale/videoWidth,offsetScale/videoHeight);
		cent0XMaterial.SetVector ("offsets", offsets0);
		cent0YMaterial.SetVector ("offsets", offsets0);
			
		centTex0X = new RenderTexture(passWidth,videoHeight,-1,RenderTextureFormat.RGHalf);
		centTex0X.filterMode=FilterMode.Point;
		centTex0Y = new RenderTexture(videoWidth,passHeight,-1,RenderTextureFormat.RGHalf);
		centTex0Y.filterMode=FilterMode.Point;
		
		offsets1 = new Vector4(offsetScale/passWidth,offsetScale/passHeight,offsetScale/passWidth,offsetScale/passHeight);
		cent1XMaterial.SetVector("offsets", offsets1);
		cent1YMaterial.SetVector("offsets", offsets1);

		passWidth/=passXSamples;
		passHeight/=passYSamples;
		Debug.Log("Centroid texture axes pass 1:" + passWidth + "," + passHeight);
		
		centTex1X = new RenderTexture(passWidth,videoHeight,-1,RenderTextureFormat.RGHalf);
		centTex1X.filterMode=FilterMode.Point;
		centTex1Y = new RenderTexture(videoWidth,passHeight,-1,RenderTextureFormat.RGHalf);
		centTex1Y.filterMode=FilterMode.Point;

		// converting RenderTexture to texture2D
		centTexX = new Texture2D(passWidth,videoHeight);
		centTexX.filterMode=FilterMode.Point;
		centTexY = new Texture2D(videoWidth,passHeight);
		centTexY.filterMode=FilterMode.Point;
	}
		
    /// <summary>
    /// Calculates the centroid for the momentGPU tracking method
    /// </summary>
	void CalculateCentroid()
	{
#if UNITY_IOS && !UNITY_EDITOR
		foreach (iPhoneGeneration gen in unsupportedDevices)
		{
			if (gen == iPhone.generation)
			{
				return;
			}
		}
#endif
		if (!centTex0X.IsCreated())	centTex0X.Create();
		if (!centTex0Y.IsCreated())	centTex0Y.Create();
		if (!centTex1X.IsCreated())	centTex1X.Create();
		if (!centTex1Y.IsCreated())	centTex1Y.Create();
		
		cent0XMaterial.SetColor("_Color",refColor);
		cent0YMaterial.SetColor("_Color",refColor);
		cent1XMaterial.SetColor("_Color",refColor);
		cent1YMaterial.SetColor("_Color",refColor);
		
		// 2-pass accumulating downsample: sum up all the gray scale values
		Graphics.Blit(videoTexture, centTex0X, cent0XMaterial);
		Graphics.Blit(centTex0X, centTex1X, cent1XMaterial);

		TextureFromRT(centTexX, centTex1X);

		float[] sumsX = new float[videoHeight];
		float[] volX = new float[videoHeight];
		for (int y=0; y<videoHeight; y++)
		{
			sumsX[y]=0.0f;
			volX[y]=0.0f;
			for (int x=0; x<centTexX.width; x++)
			{
				AccumulateFromTex(ref sumsX[y], ref volX[y], x, y);
			}
			sumsX[y]*=videoWidth/centTexX.width;
			volX[y]*=videoWidth/centTexX.width;
		}
		DoneAccumulation(); // release memory
		
		// compute average x
		float avgX = 0.0f;
		float hitX = 0.0f;
		for (int y=0; y<videoHeight; y++)
		{
			if (volX[y]>0.0f)
			{
				avgX+=sumsX[y]/volX[y];
				hitX+=1.0f;
			}
		}
		avgX/=hitX;
					
		Graphics.Blit(videoTexture, centTex0Y, cent0YMaterial);
		Graphics.Blit(centTex0Y, centTex1Y, cent1YMaterial);
		
		TextureFromRT(centTexY, centTex1Y);

		float[] sumsY = new float[videoWidth];
		float[] volY = new float[videoWidth];
		for (int x=0; x<videoWidth; x++)
		{
			sumsY[x]=0.0f;
			volY[x]=0.0f;
			for (int y=0; y<centTexY.height; y++)
			{
				AccumulateFromTex(ref sumsY[x], ref volY[x], x, y);
			}
			sumsY[x]*=videoHeight/centTexY.height;
			volY[x]*=videoHeight/centTexY.height;
		}
		DoneAccumulation(); // release memory
		
		// compute average y
		float avgY = 0.0f;
		float hitY = 0.0f;
		for (int x=0; x<videoWidth; x++)
		{
			if (volY[x]>0.0f)
			{
				avgY+=sumsY[x]/volY[x];
				hitY+=1.0f;
			}
		}
		avgY/=hitY;
		trackingCenter.Set (avgX, avgY);
		trackingScale=0.5f; // size of blob

#if SHOW_DEBUG
		// grey and color distance
		if (debugMode==ARDebugMode.Gray || debugMode==ARDebugMode.Distance)
		{
			if (debugMode==ARDebugMode.Gray)
			{
				Graphics.Blit(videoTexture, debugRTex, grayMaterial);
			}
			else
			{
				distMaterial.SetColor("_Color",refColor);
				Graphics.Blit(videoTexture, debugRTex, distMaterial);
			}
		}
		else if (debugMode==ARDebugMode.CentroidX) centTexX.Apply();
		else if (debugMode==ARDebugMode.CentroidY) centTexY.Apply();
		//Debug.Log("Moment: " + avgX + "," + avgY + " " + hitX + " " + hitY + " " + trackingFound);
#endif
		
	}
	
    /// <summary>
    /// Runs kMeans
    /// </summary>
    private void kMeanDetection()
    {
        // Find candidates
        List<Pixel> candidates = new List<Pixel>();

        for (int j = 0; j < videoTexture.height; j++)
        {
            for (int i = 0; i < videoTexture.width; i++)
            {
                Color cc = videoTexture.GetPixel(i, j);
                if (Detect(cc))
                {
                    Pixel p; p.x = i; p.y = j;
                    candidates.Add(p);

#if SHOW_DEBUG
                    debugTex.SetPixel(i, j, Color.black);
                }
                else
                    debugTex.SetPixel(i, j, Color.white);
            }
        }
        debugTex.Apply();
#else
				}
			}
		}
#endif

        if (candidates.Count < 3)
        {
            // Detection has problems, do something else (or do nothing...)
            return;
        }

        // Groups
        List<int> A = new List<int>(); List<int> B = new List<int>(); List<int> C = new List<int>();
        Pixel pA; Pixel pB; Pixel pC;

        // Set Group Seeds
        int a = 0; int b = (candidates.Count - 1); int c = b / 2;
        if ((oldA.x == 0) && (oldA.y == 0)) { pA = candidates[a]; pB = candidates[b]; pC = candidates[c]; }
        else                                { pA = oldA; pB = oldB; pC = oldC; }

        // Perform k-mean clustering
        for (int iteration = 0; iteration < 3; iteration++)
        {
            A.Clear(); B.Clear(); C.Clear();

            // Partition candidates into 3 groups
            for (int i = 0; i < candidates.Count; i++)
            {
                
                if (i == a) { A.Add(i); continue; }
                if (i == b) { B.Add(i); continue; }
                if (i == c) { C.Add(i); continue; }

                Pixel p = candidates[i];

                float dA = p.DistSquare(pA);
                float dB = p.DistSquare(pB);
                float dC = p.DistSquare(pC);

                // Add candidate to the appropriate group
                if (dA < dB) if (dA < dC) A.Add(i); else C.Add(i); else if (dB < dC) B.Add(i); else C.Add(i);
            }

            // Find the mean of every group
            Mean(A, candidates, out pA);
            Mean(B, candidates, out pB);
            Mean(C, candidates, out pC);
        }

#if SHOW_DEBUG
        // Color mean pixels
        debugTex.SetPixel(pA.x, pA.y, new Color(1, 1, 1, 1));
        debugTex.SetPixel(pB.x, pB.y, new Color(0, 1, 0, 1));
        debugTex.SetPixel(pC.x, pC.y, new Color(0, 0, 1, 1));
        debugTex.Apply();
#endif

        //setMarker(Camera.main.ScreenPointToRay(new Vector3(pA.x, pA.y, 0)), marker);
        //setMarker(Camera.main.ScreenPointToRay(new Vector3(pB.x, pB.y, 0)), marker2);
        //setMarker(Camera.main.ScreenPointToRay(new Vector3(pC.x, pC.y, 0)), marker3);

        oldA = pA;
        oldB = pB;
        oldC = pC;
    }
	

    /// <summary>
    /// Updates the blimp using kMeans
    /// </summary>
    private void kMeanUpdateBlimp()
    {
        Vector2 a, b, c;

        a = oldA.toVector2();
        b = oldB.toVector2();
        c = oldC.toVector2();

        trackingCenter = (a + b + c) / 3.0f;
        trackingScale = ((a - trackingCenter).magnitude + (b - trackingCenter).magnitude + (c - trackingCenter).magnitude) / 3.0f;
        trackingScale /= baseDistance;
    }

    	
    /// <summary>
    /// Updates the blimp using local window transformation
    /// </summary>
    private void UpdateBlimp()
    {
        Vector3 center = trackingCenter;
		if (float.IsNaN(center.x) || float.IsNaN(center.y) || center.x>1.0f || center.y>1.0f)
		{
			trackingFound = false;
			center = lastCenter;
		}
		else
		{
			trackingFound = true;
			lastCenter=center;
		}

#if UNITY_EDITOR || !UNITY_IOS
		Vector3 sVec=new Vector3(center.x*Screen.width, center.y*Screen.height, 0);
#else
		Vector3 sVec=new Vector3(center.x*Screen.width, Screen.height-center.y*Screen.height, 0);
#endif
        Ray blimpRay = Camera.main.ScreenPointToRay(sVec);

        var newPos = Camera.main.transform.position + blimpRay.direction * initialBlimpDistance / trackingScale;
        if (!float.IsInfinity(newPos.x))
            blimp.position = newPos;
    }

    /// <summary>
    /// Checks if transform intersects with a ray
    /// </summary>    
    private void setMarker(Ray ray, Transform m)
    {
        RaycastHit info;
        float distance = 1000.0f;

        if (this.gameObject.collider.Raycast(ray, out info, distance))
        {
            var pos = info.point;
            m.position = pos;
        }
    }

    /// <summary>
    /// Calculates the mean of pixels
    /// </summary>
    private void Mean(List<int> indices, List<Pixel> candidates, out Pixel meanPixel)
    {
        float x = 0.0f, y = 0.0f;
        for (int i = 0; i < indices.Count; i++)
        {
            x += candidates[indices[i]].x;
            y += candidates[indices[i]].y;
        }
        meanPixel.x = (int)(x / indices.Count + 0.5f);
        meanPixel.y = (int)(y / indices.Count + 0.5f);
    }

    /// <summary>
    /// Hard coded color threshold
    /// </summary>    
    private bool Detect(Color c)
    {
        return (c.r > 0.9 && c.g < 0.75 && c.b < 0.75);

        //float i = c.r - 0.5f * (c.g + c.b);
        //return (i > 0.6f);
    }	
    #endregion // PRIVATE_MEMBERS

    #region UNITY_MONOBEHAVIOUR_METHODS

    /// <summary>
    /// Starts up the camera 
    /// </summary>
    void Start()
    {
		Application.targetFrameRate=videoFPS;
		Screen.SetResolution (1024, 768, true);
			
		if (trackingMode==ARFeatureTrackingMode.MomentGPU) {
			MakeCentroidTexture(); // prepare texture results
		}

		// setup camera capture properties
		setFocusMode(focusMode);
		setExposureMode(exposureMode);
		startCameraCapture(useFrontCamera);
		
        initialBlimpDistance = (blimp.position - Camera.main.transform.position).magnitude;
        initialScreenPos = Camera.main.WorldToScreenPoint(blimp.position);

		DebugSetup(); // create debug texture and window
	}
	
    
    /// <summary>
    /// Updates the scene with new tracking data. Calls registered ITrackerEventHandlers
    /// </summary>
    void Update()
    {
		UpdateVideoTexture();

		// two tracking implementations available
		if (trackingMode==ARFeatureTrackingMode.kMeansCPU) // works well on PC but too slow on iPhone
		{
			kMeanDetection();
		    kMeanUpdateBlimp();
		}
		else
		{
			// works well on iphone
			CalculateCentroid();
		}

		UpdateBlimp();

		DebugUpdate();
	}
	
    /// <summary>
    /// Does currently nothing
    /// </summary>
    void OnDrawGizmosSelected()
    {
        //Gizmos.DrawLine(from, to);
    }

 	#endregion // UNITY_MONOBEHAVIOUR_METHDS
}
