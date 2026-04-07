using UnityEngine;

public class CameraController : MonoBehaviour
{   public int gridwidth ;
    public int gridheight;
    public float padding = 1f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FitCamera();
    }

    public void FitCamera()
    {
        Camera cam = Camera.main;
        float aspect =(float)Screen.width /Screen.height;
        float gridRatio = (float)gridwidth/gridheight;

        if (gridRatio >= aspect)
        {
            cam.orthographicSize = gridwidth/aspect /2f +padding;
        }
        else
        {
            cam.orthographicSize = gridheight /2f+padding;
        }

        transform.position = new Vector3((gridwidth - 1)/2f, (gridheight-1 )/2f, -10 );
    }
}
