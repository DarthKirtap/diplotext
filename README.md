# Simulated model of NICO in Unity[^1]

## Quickstart
Download Unity developer version.  
The project is made for Unity version 2022.3.4f1, other versions might work too, but it is not guaranteed.

Folder `NICO_articulation_body` contains the Unity project. Import it into your Unity hub by clicking `Add project from
disk` and choosing it.

If you wish to use ML-agents for RL, you need to have a python environment with pytorch and ML-agents packages 
(follow the installation instructions here 
https://github.com/Unity-Technologies/ml-agents/blob/release_20_docs/docs/Installation.md).  
If you do not wish to use the ML-agents, you may need to remove some components from `nico` GameObject (depending on the
scene that you choose to work with). 
Remove `Behavior Parameters`, `Nico Agent (Script)` and `Decision Requester` to unlink the ML-agents script from NICO, 
if necessary.

## Scenes
Navigate to `Assets -> Scenes` and choose one of the scenes to work with.  
There are 3 scenes in the project. `With RL` is the original scene, with articulation bodies and ML-agents script 
attached, but without articulation bodies on fingers and neck + head to simplify training. `With RL with fingers` is
a copy of `With RL` but with articulation bodies on entire body (except the left arm) - this version is highly unstable 
during training. `Without RL` has articulation bodies set, but without ML-agents components attached. Instead, there is 
a script to modify joint targets via sliders. This scene was intended for testing different physics settings.

## How does it work?
The NICO model is created using a number of GameObjects with meshes and `Articulation Body` 
(https://docs.unity3d.com/Manual/class-ArticulationBody.html) 
components. Game Object `nico` is the root of articulation and is set as immovable. Neck + head and right arm are set 
as articulation bodies (in one of the available scenes), fingers can move too. The real NICO has only 4 DoF in fingers, 
two for rotating/closing thumb, one for closing index finger, and the last one for closing middle + pinky fingers. 
There is a short function in `ControlJoints.cs` called `RotateFingers` that manages conversion between values for 
individual DoF and joint rotations.

In the training script, the option to control fingers with less DoF is also implemented. There is public bool 
`constrain_fingers` (showed as a checkbox in inspector), check it to use only 4 DoF for fingers. Please note that to
make this work, the individual fingerparts need to be tagged properly. Check the tagging in scene `With RL with fingers`
if you are not sure. Also note that this option only works with 4 DoF and 3 "root" objects (from which rotations are 
copied).

When modifying the provided scenes, don't forget to set the appropriate observation and action spaces. These should be 
(24, 21) if each finger joint moves independently, (15, 12) if all fingers have 4 DoF and (11, 8) if fingers do not move
at all. Subtract 2 from all listed values if you do want not move neck + head.

Entire model of NICO has colliders added, so collisions should work also with the left arm. For information on how to 
move the NICO, if you want to modify the scenes and/or some settings, best would be to read the documentation on 
articulation bodies, but here are a few tips:
* Check `NicoAgent.cs` script for some useful functions.
* Keep in mind that there are more possible Drive Types to control joints (`Force/Velocity/Target/Target Velocity`), 
use what works best for you. Right now, the model is set to work in `Target` mode.
* Don't forget that the movement is just a physics simulation and setting different physics parameters may have huge
impact on how the movement looks. Don't be afraid to experiment with `Damping`, `Friction`, `Mass` and `Force Limit`.
* Global physics settings (https://docs.unity3d.com/Manual/class-PhysicsManager.html) also have huge impact on the 
behavior. Try modifying `Solver Type`, `Solver Iterations` and `Solver Velocity Iterations`.
* If you want to add more objects into the scene for NICO to interact with, don't forget that each new object needs to
also have a `collider` and a `rigid body` or `articulation body` component. 
  
## Articulation body properties:

This is not necessarily the best-working setup, just a table showing the current values.

| GameObject | Mass | Damping | Joint limits     | Force Limit | Target |
|------------|------|---------|------------------|-------------|--------|
| neck       | 100  | 1       | (-90, 90)    | 100         | 0      |
| head       | 100  | 1       | (-50, 35)    | 100         | 0      |
| shoulder   | 100  | 1       | (-90, 90)    | 1000        | 0      |
| collarbone | 100  | 1       | (-90, 90)    | 100         | 0      |
| upper arm  | 100  | 1       | (-100, 90)   | 1000        | 0      |
| lower arm  | 100  | 1       | (0, 150)     | 500         | 0      |
| forearm    | 100  | 1       | (-90, 150)   | 100         | 0      |
| palm       | 100  | 1       | (-60, 60)    | 100         | 0      |
| thumb1     | 1    | 1e+21   | (-50, 70)    | 100         | 20     |
| thumb2/3/4 | 1    | 1e+21   | (-60, 0)[^2] | 100         | 0      |
| fingers    | 1    | 1e+21   | (0, 60)      | 100         | 0      |

[^2]: The ranges of thumb3/4 are (-15, 0) and their rotation should be 1/4 of the rotation of thumb2. This is due to the
fact, how thumb works in real NICO.

## Notes:
* Don't forget that it is still one project, i. e. the scripts and other assets are shared across different scenes, so 
modifying a script in a scene changes it for other scenes too.
* Currently, the training script does not need to be modified when changing from one scene to another, it automatically 
detects all articulation bodies and controls them.
* The physics seems to work OK-ish. However, it **cannot** manage big sudden changes in joint targets. Keep in mind that
your scripts need to take care of this and prevent big modifications.
  
[^1]: We would like to thank Mima Mattová for providing the model of NICO in blender.
