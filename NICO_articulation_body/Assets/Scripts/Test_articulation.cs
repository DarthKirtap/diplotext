using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test_articulation : MonoBehaviour
{
    public ArticulationBody articulation_body1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 change = new Vector3(Random.Range(-1f, 1f),0,0);
        Vector3 change = new Vector3(-10f, 10f, 10f);
        //articulation_body1.AddForce(change, ForceMode.Acceleration);
        //articulation_body1.jointPosition = new ArticulationReducedSpace(articulation_body1.jointPosition[0] - 0.001f);
        //articulation_body1.SetDriveTarget(ArticulationDriveAxis.X, -10f);
        //Debug.Log(articulation_body1.jointVelocity[0]);
        Debug.Log(Mathf.Rad2Deg * articulation_body1.jointPosition[0]);

    }
}
