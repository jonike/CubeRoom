using UnityEngine;
using System.Collections;
using System.IO;

/*

Usage:
1. Create a camera for rendering texture.
2. Create a RenderTexture object and set it as targetTexture on renderer camera.
1. Attach this script to the renderer camera's game object.
2. Set renderer camera's Clear Flags field to Solid Color and set Background color's alpha to zero.
3. Use the inspector to set frameRate and framesToCapture.
4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution).
5. Turn on "Maximise on Play".
6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.

*/

public class ScreenshotRecorder : MonoBehaviour
{

    #region public fields

    [Tooltip("A folder will be created with this base name in your project root")]
    public string folderBaseName = "Screenshots";
    [Tooltip("How many frames should be captured per second of game time")]
    public int frameRate = 24;
    [Tooltip("How many frames should be captured before quitting")]
    public int framesToCapture = 24;
    public RenderTexture renderTexture;
    
    #endregion

    #region private fields

    private string folderName = "";
    private Camera mainCamera;
    private Camera renderCamera;
    private int renderFrame = 0; // how many frames we've rendered
    private float originalTimescaleTime;
    private bool done = false;
    private int width;
    private int height;
    private Texture2D outputTexture;

    #endregion

    void Awake()
    {
        mainCamera = gameObject.GetComponent<Camera>();

        GameObject cameraGO = transform.Find("/RenderCamera").gameObject;
        if (!cameraGO)
            cameraGO = new GameObject("RenderCamera");

        renderCamera = cameraGO.GetComponent<Camera>();
        if (!renderCamera)
            renderCamera = cameraGO.AddComponent<Camera>();

        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = Color.clear;
        renderCamera.transform.position = mainCamera.transform.position;
        renderCamera.transform.rotation = mainCamera.transform.rotation;
        renderCamera.orthographicSize = mainCamera.orthographicSize;
        
        CacheAndInitialiseFields();
        CreateNewFolderForScreenshots();

        Time.captureFramerate = frameRate;
    }

    void LateUpdate()
    {
        if (!done)
        {
            StartCoroutine(CaptureFrame());
        }
        else
        {
            Complete();
            Debug.Log("Complete! " + renderFrame + " videoframes rendered. File names are 0 indexed)");
            Debug.Break();
        }
    }

    IEnumerator CaptureFrame()
    {
        yield return new WaitForEndOfFrame();
        if (renderFrame < framesToCapture)
        {
            RenderTextureToPNG();
            renderFrame++;
            Debug.Log("Rendered frame " + renderFrame);
        }
        else
        {
            done = true;
            StopCoroutine("CaptureFrame");
        }
    }

    void CacheAndInitialiseFields()
    {
        originalTimescaleTime = Time.timeScale;
        width = Screen.width;
        height = Screen.height;

        renderTexture.width = width;
        renderTexture.height = height;
        renderCamera.targetTexture = renderTexture;

        outputTexture = new Texture2D(width, height);
    }

    void CreateNewFolderForScreenshots()
    {
        // Find a folder name that doesn't exist yet. Append number if necessary.
        folderName = folderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(folderName))
        {
            folderName = folderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(folderName); // Create the folder
    }

    void Complete()
    {
        Destroy(renderCamera.gameObject);
    }

    void RenderTextureToPNG()
    {
        RenderTexture oldRT = RenderTexture.active; // Save old active render texture

        RenderTexture.active = renderTexture;

        outputTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        outputTexture.Apply();

        SavePng();

        RenderTexture.active = oldRT;
    }

    void SavePng()
    {
        string path = string.Format("{0}/{1:D04} shot.png", folderName, renderFrame);
        var pngShot = outputTexture.EncodeToPNG();
        File.WriteAllBytes(path, pngShot);
    }

}