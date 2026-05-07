using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Articulation_controller : MonoBehaviour
{
    ArticulationBody nico;
    List<float> initial_targets = new List<float>();
    List<float> initial_positions = new List<float>();
    List<float> initial_velocities = new List<float>();
    List<int> inds = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        nico = GetComponent<ArticulationBody>();
        nico.GetDriveTargets(initial_targets);
        nico.GetJointPositions(initial_positions);
        nico.GetJointVelocities(initial_velocities);

        nico.GetDofStartIndices(inds);
    }

    // Update is called once per frame
    void Update()
    {
        nico.GetDriveTargets(initial_targets);
        foreach (int i in inds)
        {
            Debug.Log(initial_targets[i]);
        }

    }
}
