using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public TouchPhase left_phase;
    public Vector3 left_position;
    public TouchPhase right_phase;
    public Vector3 right_position;
    public bool has_right;
    public bool has_left;
}