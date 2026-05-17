using UnityEngine;

public class MiniMap : MonoBehaviour
{
   //[SerializeField] private LineRenderer line;
   [SerializeField] private GameObject trackPath;
   [SerializeField] public GameObject localPlayer;
   [SerializeField] public GameObject miniMapCam;
   [SerializeField] public GameObject playerPositionSphere;
    void Start()
    {
       /* line = GetComponent<LineRenderer>();
        trackPath = this.gameObject;

        int numPoints = trackPath.transform.childCount;
        line.positionCount = numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            line.SetPosition(i, new Vector3(trackPath.transform.GetChild(i).position.x, 4, trackPath.transform.GetChild(i).position.z));
        }
        //line.SetPosition(numPoints, line.GetPosition(1));
        line.startWidth = 15f;
        line.endWidth = 15f;*/
    }

    // Update is called once per frame
    void Update()
    {
        miniMapCam.transform.position = new Vector3(localPlayer.transform.position.x, miniMapCam.transform.position.y, localPlayer.transform.position.z);
        playerPositionSphere.transform.position = new Vector3(localPlayer.transform.position.x, playerPositionSphere.transform.position.y, localPlayer.transform.position.z);
    }
}
