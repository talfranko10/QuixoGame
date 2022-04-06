using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private ulong bitboard;
    public bool isMoving = false;
    public int speed = 300;
    private readonly float mZCoord; /////////////

    public Player()
    {
        bitboard = 0b0000000000000000000000000;
    }

    ulong GetBitboard()
    {
        return bitboard;
    }

    public void SetBitboard(ulong bitboard)
    {
        this.bitboard = bitboard;
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition; 
        mousePoint.z = mZCoord; 
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
