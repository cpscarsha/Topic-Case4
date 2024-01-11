using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public TouchPhase input1_phase;
    public Vector3 input1_position;
    public TouchPhase input2_phase;
    public Vector3 input2_position;

}