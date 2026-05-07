using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using TMPro;

public class NicoAgent : Agent
{
    // initialize used variables

    ArticulationBody nico;

    private List<float> initial_targets = new List<float>();
    private List<float> initial_positions = new List<float>();
    private List<float> initial_velocities = new List<float>();
    private List<float> targets = new List<float>();
    private List<float> initial_changes = new List<float>();
    private List<float> changes = new List<float>();

    private List<int> dof_ind = new List<int>();

    private List<float> low_limits;
    private List<float> high_limits;

    private int dofs;
    private int abs;

    private float averageAngle = 0;
    private float averageDistance = 0;
    private float minAngle = 180;
    private float minDistance = 100;
    private int steps = 0;

    private bool epFinished = false;

    [Range(0, 4)]
    public int targetedSide; //0-red, yellow, green, blue, white

    [Tooltip("SCube")]
    public GameObject targetCube;

    [Tooltip("Random target")]
    public bool randomTarget = true;

    [Tooltip("Reset after episode")]
    public bool reset = true;

    [Tooltip("The target position")]
    public GameObject targetFar;

    [Tooltip("Base")]
    public GameObject baseBlock;

    [Tooltip("Red, yellow, green, blue, white")]
    public List<GameObject> sides; //0-red, yellow, green, blue, white

    [Tooltip("Fixed vector")]
    public GameObject rayPoint;

    [Range(0, 200f)]
    public float angleRewardModifier;
    [Range(0, 200f)]
    public float distanceRewardModifier;

    public GameObject textField;

    private float last_point = 0;
    private void GetLimits(ArticulationBody root, List<float> llimits, List<float> hlimits)
    {
        GameObject curr_obj = root.gameObject;
        int num_ch = curr_obj.transform.childCount;
        for (int i = 0; i < num_ch; ++i)
        {
            // get articulation body component from child with index i, get its drive limits, write them to lists, call recursively on children
            GameObject child = curr_obj.transform.GetChild(i).gameObject;
            ArticulationBody child_ab = child.GetComponent<ArticulationBody>();
            if (child_ab != null && child_ab.enabled)
            {
                int j = child_ab.index;
                // getting limits
                llimits[j - 1] = child_ab.xDrive.lowerLimit;
                hlimits[j - 1] = child_ab.xDrive.upperLimit;
                GetLimits(child_ab, llimits, hlimits);
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        // remember initial joint positions and velocities so we can reset them at next episode start

        nico = GetComponent<ArticulationBody>();
        nico.GetDofStartIndices(dof_ind);
        abs = nico.GetDriveTargets(initial_targets);
        nico.GetJointPositions(initial_positions);
        nico.GetJointVelocities(initial_velocities);

        low_limits = new List<float>(new float[abs]);
        high_limits = new List<float>(new float[abs]);

        GetLimits(nico, low_limits, high_limits);

        dofs = abs;
        for (int i = 0; i < dofs; ++i)
        {
            initial_changes.Add(0f);
        }

        changes = new List<float>(initial_changes);
        targets = new List<float>(initial_targets);
    }

    public override void OnEpisodeBegin()
    {
        if (!reset && steps > 0)
        {
            epFinished = true;
            return;
        }

        // reset joint positions, velocities, targets

        nico.SetDriveTargets(initial_targets);
        nico.SetJointPositions(initial_positions);
        nico.SetJointVelocities(initial_velocities);

        changes = new List<float>(initial_changes);
        targets = new List<float>(initial_targets);

        if (randomTarget)
        {
            targetedSide = Random.Range(0, 5);
        }

        //Debug.Log("Targeted side: " + targetedSide + " red, yellow, green, blue, white");
        //targetPoint = Random.Range(0, 2);
        //Debug.Log("Targeted point: " + targetPoint);

        switch (targetedSide)
        {
            case 0: baseBlock.GetComponent<Renderer>().material.color = Color.red; break;
            case 1: baseBlock.GetComponent<Renderer>().material.color = Color.yellow; break;
            case 2: baseBlock.GetComponent<Renderer>().material.color = Color.green; break;
            case 3: baseBlock.GetComponent<Renderer>().material.color = Color.blue; break;
            case 4: baseBlock.GetComponent<Renderer>().material.color = Color.white; break;
        }

        averageAngle = 0;
        averageDistance = 0;
        minAngle = 180;
        minDistance = 100;
        steps = 0;
}

    public override void CollectObservations(VectorSensor sensor)
    {
        // observe current targets
        List<float> observation = new List<float>();

        nico.GetDriveTargets(observation);

        sensor.AddObservation(observation);
       

        // get vector from end effector to target

        sensor.AddObservation(targetCube.transform.position);
        sensor.AddObservation(targetCube.transform.rotation);
        for (int i = 0; i < 5; i++)
        {
            if (i == targetedSide)
            {
                sensor.AddObservation(1);
            }
            else
            {
                sensor.AddObservation(0);
            }
        }
            
        sensor.AddObservation((getAngle() - 90f) / 90f);

        switch (targetedSide)
        {
            case 0: baseBlock.GetComponent<Renderer>().material.color = Color.red; break;
            case 1: baseBlock.GetComponent<Renderer>().material.color = Color.yellow; break;
            case 2: baseBlock.GetComponent<Renderer>().material.color = Color.green; break;
            case 3: baseBlock.GetComponent<Renderer>().material.color = Color.blue; break;
            case 4: baseBlock.GetComponent<Renderer>().material.color = Color.white; break;
        }
    }

    private float getAngle() {

        GameObject target;

        target = targetFar;

        Vector3 rayStart = rayPoint.transform.position;

        // Define the direction for the ray
        Vector3 rayDirection = rayStart - target.transform.position;

        // Perform the raycast
        // Draw the ray if it didn't hit anything, up to max distance
        Debug.DrawLine(rayStart, rayStart - rayDirection * 1, Color.blue);
        Debug.DrawLine(sides[targetedSide].transform.position, sides[targetedSide].transform.position - sides[targetedSide].transform.forward * 1, Color.red);

        var normal = sides[targetedSide].transform.forward;
        var angle = Vector3.Angle(normal, rayDirection);


        return angle;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (epFinished) return;
        float max_range = Mathf.Deg2Rad * 0.1f;
        float change_magnitude = Mathf.Deg2Rad * 0.05f;

        // modify incremental changes according to policy outputs

        for (int i = 0; i < dofs; ++i)
        {
            changes[i] = Mathf.Clamp(changes[i] + actions.ContinuousActions[i] * change_magnitude, -max_range, max_range);
        }

        // add changes to targets, then clamp to joint limits

        for (int i = 0; i < abs; ++i)
        {
            targets[i] = Mathf.Clamp(targets[i] + changes[i], Mathf.Deg2Rad * low_limits[i], Mathf.Deg2Rad * high_limits[i]);
        }

        // set joint targets to modified values

        nico.SetDriveTargets(targets);

        // calculate reward

        GameObject target;

        target = targetFar;

        float new_dist = (targetCube.transform.position - target.transform.position).magnitude;

        float angle = 180 - getAngle();

        averageAngle = (averageAngle * steps * 0.9f + angle) / (steps * 0.9f + 1);
        averageDistance = (averageDistance * steps * 0.9f + new_dist) / (steps * 0.9f + 1);
        steps++;

        minAngle = Mathf.Min(minAngle, angle);
        minDistance = Mathf.Min(minDistance, new_dist);

        textField.GetComponent<TextMeshProUGUI>().text = "Average angle: " + averageAngle.ToString("#.000") +
            "\r\nMin angle: " + minAngle.ToString("#.000") +
            "\r\n\r\nAverage distance: " + averageDistance.ToString("#.000") +
            "\r\nMin distance: " + minDistance.ToString("#.000");

        AddReward( GetReward(new_dist) );
    }

    private float GetReward(float new_dist) {
        float movement_reward = 0f;

        // penalize for movements so the arm does not shake
        for (int i = 0; i < dofs; ++i)
        {
            movement_reward += 2f * Mathf.Abs(changes[i]);
        }

        float limit_reward = 0f;

        for(int i = 0; i < abs; ++i)
        {
            if (targets[i] + Mathf.Deg2Rad * (10f) > Mathf.Deg2Rad * high_limits[i] || targets[i] - Mathf.Deg2Rad * (10f) < Mathf.Deg2Rad * low_limits[i])
            {
                limit_reward += 0.05f;
            }
        }

        var angle_reward = (getAngle() - 90f)/angleRewardModifier;
        float distance_reward = new_dist * new_dist * distanceRewardModifier;


        //Debug.Log("Angle reward: " + angle_reward + ", distance_reward: " + distance_reward + ", movement_reward: " + movement_reward + ", limit_reward: " + limit_reward);
        // add all reward components together and provide reward to agent
        return angle_reward - distance_reward - movement_reward - limit_reward;
    }
}
