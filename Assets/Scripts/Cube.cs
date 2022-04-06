using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cube : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    private Board board;
    public Vector3 position;
    private int cubeIndex;
    public int row;
    public int col;

    public void UpdateCubeIndex()
    {
        cubeIndex = System.Array.IndexOf(board.arrCubes, this);
        row = cubeIndex / 5;
        col = cubeIndex % 5;
        position = this.transform.position;
    }

    void Start()
    {
        UpdateCubeIndex();
        board.winLabel.gameObject.SetActive(false);
    }

    public void SetBoardReference(Board b) 
    {
        board = b;
    }

    void OnMouseDown()
    {
        UpdateCubeIndex();
        board.RotateACube(cubeIndex, this);
    }

    void OnMouseDrag()
    {
        board.DragACube(cubeIndex, this);
    }

    void OnMouseUp() 
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) 
            board.PushACube(cubeIndex, this, hit);
        for (int i = 0; i < board.arrowsArr.Length; i++)
        {
            Destroy(board.arrowsArr[i]);
        }
    } 
}
