using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ControlJoints : MonoBehaviour
{
    public ArticulationBody ab_neck;
    public ArticulationBody ab_head;
    public ArticulationBody ab_shoulder;
    public ArticulationBody ab_collarbone;
    public ArticulationBody ab_upperarm;
    public ArticulationBody ab_lowerarm;
    public ArticulationBody ab_forearm;
    public ArticulationBody ab_palm;
    public ArticulationBody ab_thumb1;
    public ArticulationBody ab_thumb2;
    public ArticulationBody ab_thumb3;
    public ArticulationBody ab_thumb4;
    public ArticulationBody ab_index1;
    public ArticulationBody ab_index2;
    public ArticulationBody ab_index3;
    public ArticulationBody ab_middle1;
    public ArticulationBody ab_middle2;
    public ArticulationBody ab_middle3;
    public ArticulationBody ab_pinky1;
    public ArticulationBody ab_pinky2;
    public ArticulationBody ab_pinky3;

    [Range(-89f, 89f)]
    public float neck = 0f;
    [Range(-49f, 34f)]
    public float head = 0f;
    [Range(-89f, 89f)]
    public float shoulder = 0f; 
    [Range(-89f, 89f)]
    public float collarbone = 0f;
    [Range(-99f, 99f)]
    public float upperarm = 0f;
    [Range(0f, 149f)]
    public float lowerarm = 0f;
    [Range(-89f, 149f)]
    public float forearm = 0f;
    [Range(-59f, 59f)]
    public float palm = 0f;
    [Range(-49f, 69f)]
    public float thumb1 = 20f;
    [Range(-59f, 0f)]
    public float thumb2 = 0f;
    [Range(0f, 69f)]
    public float index = 0f;
    [Range(0f, 69f)]
    public float fingers = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ab_neck.SetDriveTarget(ArticulationDriveAxis.X, neck);
        ab_head.SetDriveTarget(ArticulationDriveAxis.X, head);
        ab_shoulder.SetDriveTarget(ArticulationDriveAxis.X, shoulder);
        ab_collarbone.SetDriveTarget(ArticulationDriveAxis.X, collarbone);
        ab_upperarm.SetDriveTarget(ArticulationDriveAxis.X, upperarm);
        ab_lowerarm.SetDriveTarget(ArticulationDriveAxis.X, lowerarm);
        ab_forearm.SetDriveTarget(ArticulationDriveAxis.X, forearm);
        ab_palm.SetDriveTarget(ArticulationDriveAxis.X, palm);

        //ab_neck.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * neck);
        //ab_head.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * head);
        //ab_shoulder.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * shoulder);
        //ab_collarbone.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * collarbone);
        //ab_upperarm.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * upperarm);
        //ab_lowerarm.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * lowerarm);
        //ab_forearm.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * forearm);
        //ab_palm.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * palm);

        RotateFingers(thumb1, thumb2, index, fingers, ab_thumb1, ab_thumb2, ab_thumb3, ab_thumb4, ab_index1, ab_index2, ab_index3, 
            ab_middle1, ab_middle2, ab_middle3, ab_pinky1, ab_pinky2, ab_pinky3);
    }

    private void RotateFingers(float angle1, float angle2, float angle3, float angle4, 
        ArticulationBody t1, ArticulationBody t2, ArticulationBody t3, ArticulationBody t4,
        ArticulationBody i1, ArticulationBody i2, ArticulationBody i3,
        ArticulationBody m1, ArticulationBody m2, ArticulationBody m3,
        ArticulationBody p1, ArticulationBody p2, ArticulationBody p3)
    {
        // The first DoF controls thumb rotation
        t1.SetDriveTarget(ArticulationDriveAxis.X, Mathf.Clamp(angle1, t1.xDrive.lowerLimit, t1.xDrive.upperLimit));
        //t1.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * angle1);

        // The second DoF "closes" the thumb

        float ang2 = Mathf.Clamp(angle2, t2.xDrive.lowerLimit, t2.xDrive.upperLimit);
        t2.SetDriveTarget(ArticulationDriveAxis.X, ang2);
        t3.SetDriveTarget(ArticulationDriveAxis.X, ang2/4);
        t4.SetDriveTarget(ArticulationDriveAxis.X, ang2/4);
        //t2.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang2);
        //t3.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang2/4);
        //t4.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang2/4);

        // The third DoT "closes" the index finger
        float ang3 = Mathf.Clamp(angle3, i1.xDrive.lowerLimit, i1.xDrive.upperLimit);
        i1.SetDriveTarget(ArticulationDriveAxis.X, ang3);
        i2.SetDriveTarget(ArticulationDriveAxis.X, ang3);
        i3.SetDriveTarget(ArticulationDriveAxis.X, ang3);
        //i1.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang3);
        //i2.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang3);
        //i3.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang3);

        // The fourth DoF "closes" both middle and pinky finger at once
        float ang4 = Mathf.Clamp(angle4, m1.xDrive.lowerLimit, m1.xDrive.upperLimit);
        m1.SetDriveTarget(ArticulationDriveAxis.X, ang4);
        m2.SetDriveTarget(ArticulationDriveAxis.X, ang4);
        m3.SetDriveTarget(ArticulationDriveAxis.X, ang4);
        p1.SetDriveTarget(ArticulationDriveAxis.X, ang4);
        p2.SetDriveTarget(ArticulationDriveAxis.X, ang4);
        p3.SetDriveTarget(ArticulationDriveAxis.X, ang4);
        //m1.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang4);
        //m2.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang4);
        //m3.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang4);
        //p1.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang4);
        //p2.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang4);
        //p3.jointPosition = new ArticulationReducedSpace(Mathf.Deg2Rad * ang4);

    }
}
