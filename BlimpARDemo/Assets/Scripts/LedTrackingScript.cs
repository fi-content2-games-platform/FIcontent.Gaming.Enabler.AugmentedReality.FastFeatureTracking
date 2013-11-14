using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LedTrackingScript : MonoBehaviour
{

    private WebCamTexture texture;

    //public Transform marker;
    //public Transform marker2;
    //public Transform marker3;

    public Transform blimp;
    //public Transform camera;

    Texture2D tex;

    private Pixel oldA, oldB, oldC;
    public float baseDistance = 40;
    public float initialBlimpDistance;
    private Vector3 initialScreenPos;

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

    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (var i = 0; i < devices.Length; i++)
            Debug.Log("Device detected (" + devices[i].name + ")");

        if (devices.Length == 0)
        {
            Debug.Log("No webcams detected");
            return;
        }

        int resX = 640, resY = 480, fps = 60;
        texture = new WebCamTexture(resX, resY, fps);
        //this.gameObject.renderer.material.mainTexture = texture;        
        texture.Play();

        tex = new Texture2D(resX, resY, TextureFormat.RGBA32, false);
		guiTexture.texture = tex;
		
        GameObject bgimg = GameObject.Find("Background Image");
        bgimg.guiTexture.texture = texture;

        initialBlimpDistance = (blimp.position - Camera.main.transform.position).magnitude;
        initialScreenPos = Camera.main.WorldToScreenPoint(blimp.position); //camera.GetComponent<Camera>().WorldToScreenPoint(blimp.position);

        //Debug.Log(initialScreenPos);
        //Debug.Log(initialBlimpDistance);
    }

    // Update is called once per frame
    void Update()
    {
        kMeanDetection();

        UpdateBlimp();
    }

    private void kMeanDetection()
    {
        // Find candidates
        List<Pixel> candidates = new List<Pixel>();

        for (int j = 0; j < texture.height; j++)
        {
            for (int i = 0; i < texture.width; i++)
            {
                Color cc = texture.GetPixel(i, j);
                if (Detect(cc))
                {
                    Pixel p; p.x = i; p.y = j;
                    candidates.Add(p);

                    tex.SetPixel(i, j, Color.black);
                }
                else
                    tex.SetPixel(i, j, Color.white);
            }
        }
        tex.Apply();

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
        else { pA = oldA; pB = oldB; pC = oldC; }

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

        // Color mean pixels
        tex.SetPixel(pA.x, pA.y, new Color(1, 1, 1, 1));
        tex.SetPixel(pB.x, pB.y, new Color(0, 1, 0, 1));
        tex.SetPixel(pC.x, pC.y, new Color(0, 0, 1, 1));
        tex.Apply();


        //setMarker(Camera.main.ScreenPointToRay(new Vector3(pA.x, pA.y, 0)), marker);
        //setMarker(Camera.main.ScreenPointToRay(new Vector3(pB.x, pB.y, 0)), marker2);
        //setMarker(Camera.main.ScreenPointToRay(new Vector3(pC.x, pC.y, 0)), marker3);

        oldA = pA;
        oldB = pB;
        oldC = pC;
    }

    private void UpdateBlimp()
    {
        Vector2 a, b, c;

        a = oldA.toVector2();
        b = oldB.toVector2();
        c = oldC.toVector2();

        Vector2 center = (a + b + c) / 3.0f;
        float scaling = ((a - center).magnitude + (b - center).magnitude + (c - center).magnitude) / 3.0f;
        scaling /= baseDistance;

        center *= Screen.width / 640.0f;

        //Camera compCam = camera.GetComponent<Camera>();
        Ray blimpRay = Camera.main.ScreenPointToRay(new Vector3(center.x, center.y, initialScreenPos.z));

        var newPos = Camera.main.transform.position + blimpRay.direction * initialBlimpDistance / scaling;

        if (!float.IsInfinity(newPos.x))
            blimp.position = newPos;

    }

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

    void OnDrawGizmosSelected()
    {
        //Gizmos.DrawLine(from, to);
    }

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

    private bool Detect(Color c)
    {
        return (c.r > 0.9 && c.g < 0.75 && c.b < 0.75);

        //float i = c.r - 0.5f * (c.g + c.b);
        //return (i > 0.6f);
    }
}
