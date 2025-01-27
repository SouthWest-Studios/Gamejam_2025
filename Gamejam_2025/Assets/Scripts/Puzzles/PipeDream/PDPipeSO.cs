using UnityEngine;

[CreateAssetMenu(fileName = "NewPipeType", menuName = "PipeDream/Pipe Type")]
public class PDPipeType : ScriptableObject
{
    public string pipeName;
    public Sprite emptySprite;
    public Sprite[] fillAnimationSprites;
    public bool[] connections = new bool[4]; // Arriba, Derecha, Abajo, Izquierda
    public int initialRotation = 0;
    public bool isFilled = false;

    public bool IsConnectedTo(PDPipeType other, int direction)
    {
        return connections[direction] && other.connections[(direction + 2) % 4];
    }

    public void RotatePipe(int rotations)
    {
        bool[] newConnections = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            newConnections[(i + rotations) % 4] = connections[i];
        }
        connections = newConnections;
    }
}
