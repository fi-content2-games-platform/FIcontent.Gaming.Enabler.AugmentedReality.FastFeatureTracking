using UnityEngine;
using System.Collections;

public class BackgroundWebcamFeed : MonoBehaviour
{
    WebCamDevice[] cameras;
    WebCamTexture camfeed;
	
	private int writeIndex;
	private int readIndex;
	public bool delay = false;
	public int bufferSize = 20;
	public int fps = 60;
	
	private float elapsedTime;
	UnityEngine.Texture2D[] buffer;
	GameObject bgimg;
    public void Start()
    {
        cameras = WebCamTexture.devices;
		buffer = new UnityEngine.Texture2D[bufferSize];
		for(int i = 0; i < bufferSize; i++)
		{
			buffer[i] = new Texture2D(640, 480, TextureFormat.ARGB32, false);	
		}
		writeIndex = 0;
		readIndex = 10;
		elapsedTime = 0;
        if ((cameras != null) && (cameras.Length > 0))
        {
            Debug.Log("Webcam found:" + cameras.Length);
            camfeed = new WebCamTexture(cameras[cameras.Length-1].name, 640, 480, fps);
            camfeed.Play();

            bgimg = GameObject.Find("Background Image");
			bgimg.guiTexture.texture = camfeed;			
        }
    }
	
	public void Update()
	{
		if(delay)
		{
			elapsedTime += Time.deltaTime;
		
			while(elapsedTime > 1.0f / fps)
				stepIndices();
		}
	}
	
	private void stepIndices()
	{
		elapsedTime -= 1.0f / fps;
		
		writeIndex++; if(writeIndex >= buffer.Length) writeIndex = 0;
		readIndex++; if(readIndex >= buffer.Length) readIndex = 0;
		
		buffer[writeIndex].SetPixels(camfeed.GetPixels());
		buffer[writeIndex].Apply();
		
		bgimg.guiTexture.texture = buffer[readIndex];
	}
}
