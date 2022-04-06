using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Cube[] arrCubes;
    public GameObject arrow;
    public Dictionary<int, string[]> arrowsDict = new Dictionary<int, string[]>
    {
        {0, new string[] {"right", "down"}},
        {1, new string[] {"left", "right", "down"}},
        {2, new string[] {"left", "right", "down"}},
        {3, new string[] {"left", "right", "down"}},
        {4, new string[] {"left", "down"}},
        {5, new string[] {"up", "down", "right"}},
        {10, new string[] {"up", "down", "right"}},
        {15, new string[] {"up", "down", "right"}},
        {20, new string[] {"up", "right"}},
        {9, new string[] {"up", "down", "left"}},
        {14, new string[] {"up", "down", "left"}},
        {19, new string[] {"up", "down", "left"}},
        {24, new string[] {"up", "left"}},
        {21, new string[] {"up", "left", "right"}},
        {22, new string[] {"up", "left", "right"}},
        {23, new string[] {"up", "left", "right"}} 
    };

    public int playerTurn = 1;
    public ulong xBitboard = 0b0000000000000000000000000;
    public ulong oBitboard = 0b0000000000000000000000000;
    public Text winLabel;
    public ulong[] winMasks = new ulong[12] 
    {
        0b1111100000000000000000000,
        0b0000011111000000000000000,
        0b0000000000111110000000000,
        0b0000000000000001111100000,
        0b0000000000000000000011111,
        0b1000010000100001000010000,
        0b0100001000010000100001000,
        0b0010000100001000010000100,
        0b0001000010000100001000010,
        0b0000100001000010000100001,
        0b1000001000001000001000001,
        0b0000100010001000100010000,
    };
    public GameObject[] arrowsArr = new GameObject[4];
    public int speed = 300;
    public bool isMoving = false;
    private Vector3 mOffset;
    private float mZCoord;
    private const ulong rightLeftB = 0b1111100000000000000000000;
    private const ulong upDownB = 0b1000010000100001000010000;
    private bool isWin = false;
    //GameOverScreen GameOverScreen;
    
    public void Awake()
    {
        SetBoardReferenceOnCubes();
    }

    public void SetBoardReferenceOnCubes()
    {
        for (int i = 0; i < arrCubes.Length; i++)
        {
            arrCubes[i].GetComponentInParent<Cube>().SetBoardReference(this);
        }
    }

    public void ChangePlayer()
    {
        playerTurn *= -1;
    }

    public bool IsPossibleCube(int cubeIndex, int currX)
    {
        ulong indexBitboard = (ulong)0b1000000000000000000000000 >> cubeIndex;
        if (((currX == -1 ? xBitboard : oBitboard) & indexBitboard) == indexBitboard)
            return false;
        return true;
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition; 
        mousePoint.z = mZCoord; 
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public IEnumerator Roll(int rotateDirection, Cube cube) {
        isMoving = true;
        float remainingAngle = 90;
        while (remainingAngle > 0) {
            float rotationAngle = Mathf.Min(Time.deltaTime * speed, remainingAngle);
            cube.transform.Rotate(rotationAngle * rotateDirection, 0, 0); //x = 1, o = -1. If x, rotate -90, else rotate 90.
            remainingAngle -= rotationAngle;
            yield return null;
        }
        isMoving = false;
    }

    public void RotateACube(int cubeIndex, Cube cube)
    {
        if(!arrowsDict.ContainsKey(cubeIndex) || !IsPossibleCube(cubeIndex, playerTurn * -1) || isWin)
            return;

        if (IsPossibleCube(cubeIndex, playerTurn))
        {
            if (!isMoving)
                StartCoroutine(Roll(playerTurn, cube)); 
        }
        mZCoord = Camera.main.WorldToScreenPoint(cube.transform.position).z; 
        mOffset = cube.transform.position - GetMouseAsWorldPoint();
        PrintArrows(arrowsDict, arrow, cubeIndex, cube);   
    }

    void PrintArrows(Dictionary<int,string[]> arrowsDict, GameObject arrow, int index, Cube cube)
    {
        float newCoor = 0;
        int yRotation = 0;
        GameObject duplicate;
        for (int i = 0; i < arrowsDict[index].Length; i++)
        {
            switch (arrowsDict[index][i])
            {
                case "right":
                    newCoor = arrCubes[index + 4 - cube.col].transform.position.x + 21;
                    yRotation = 180;
                    break;
                case "left":
                    newCoor = arrCubes[index - cube.col].transform.position.x - 21;
                    yRotation = 0;
                    break;
                case "down":
                    newCoor = arrCubes[index + 5 * (4 - cube.row)].transform.position.z - 21;
                    yRotation = 270;
                    break;
                case "up":
                    newCoor = arrCubes[index - 5 * cube.row].transform.position.z + 21;
                    yRotation = 90;
                    break;
            }
            if (arrowsDict[index][i] == "right" || arrowsDict[index][i] == "left")
                duplicate = Instantiate(arrow, new Vector3(newCoor, cube.transform.position.y + 20, cube.transform.position.z), Quaternion.Euler(0, yRotation, 0));
            else
                duplicate = Instantiate(arrow, new Vector3(cube.transform.position.x, cube.transform.position.y + 20, newCoor), Quaternion.Euler(0, yRotation, 0));
            duplicate.name = arrowsDict[index][i];
            arrowsArr[i] = duplicate;
        }
    }

    public void DragACube(int cubeIndex, Cube cube)
    {
        if((arrowsDict.ContainsKey(cubeIndex) && IsPossibleCube(cubeIndex, playerTurn * -1)) && !isWin)
            cube.transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    IEnumerator Move(Cube cube, Vector3 beginPos, Vector3 endPos, float time)
    {
        for(float t = 0; t < 1; t += Time.deltaTime / time)
        {
            cube.transform.position = Vector3.Lerp(beginPos, endPos, t);
            yield return null;
        }
    }

    void ArrElementsSwap(Cube[] arr, int x, int y)
    {
        if (x > -1 && x < 25 && y > -1 && y < 25)
        {
            var buffer = arr[x];
            arr[x] = arr[y];
            arr[y] = buffer;
        }
    }

    void PushAndSwap(int startIndex, int endIndex, int num, int addToX, int addToZ, int swapFrom, int swapTo, int rowCol, bool b)
    {
        for (int i = startIndex; i < endIndex; i += num) //push
        {
            var curPos = arrCubes[i].transform.position;
            var targetPos = new Vector3(curPos.x + addToX, 10, curPos.z + addToZ);
            StartCoroutine(Move(arrCubes[i], curPos, targetPos, 0.5f));
        }
        for (int i = (b ? startIndex : startIndex + num * rowCol); b ? i < endIndex - num : i > endIndex - num * rowCol - num; i += (b ? num : -num)) //swap
            ArrElementsSwap(arrCubes, i + swapFrom, i + swapTo);   
    }

    void PushBitboard(string direction, int cubeIndex, int targetIndex, int howMany)
    {
        ulong A = (ulong)0b1000000000000000000000000 >> cubeIndex;
        if (playerTurn == 1)
            xBitboard &= ~A; 
        else
            oBitboard &= ~A; 
        ulong C = (ulong)0b1000000000000000000000000 >> targetIndex;
        ulong help = (ulong)0b1111111111111111111111111 >> (cubeIndex + 1);
        ulong B = (direction == "right" || direction == "left" ? rightLeftB : upDownB) >> (direction == "right" || direction == "down" ? cubeIndex : targetIndex);
        B = direction == "left" || direction == "up" ? (B ^ help) & B : B;
        if (direction == "right" || direction == "down")
        {
            xBitboard = (((xBitboard & B) << howMany) & B) | (xBitboard & ~B); 
            oBitboard = (((oBitboard & B) << howMany) & B) | (oBitboard & ~B); 
        }
        else
        {
            xBitboard = (((xBitboard & B) >> howMany) & B) | (xBitboard & ~B); 
            oBitboard = (((oBitboard & B) >> howMany) & B) | (oBitboard & ~B); 
        }
        if (playerTurn == 1)
            xBitboard |= C; 
        else
            oBitboard |= C; 
        print("x " + xBitboard);
        print("o " + oBitboard);
    }

    bool Win()
    {
        string winner = null;
        foreach (ulong mask in winMasks)
        {
            if (mask != 0 && (mask & xBitboard) == mask)
            {
                winner = "X";
                break;
            }
            if (mask != 0 && (mask & oBitboard) == mask)
            {
                winner = "O";
                break;
            }
        }
        if (winner != null)
        {   
            isWin = true;
            winLabel.text = winner + " Won!";
            winLabel.gameObject.SetActive(true);
           // GameOverScreen.Setup(winner);
            //GameOver();
            return true;
        }
        return false;
    }

    public void PushACube(int cubeIndex, Cube cube, RaycastHit hit)
    {
        if(!arrowsDict.ContainsKey(cubeIndex) || !IsPossibleCube(cubeIndex, playerTurn * -1) || isWin)
            return;
        if (hit.collider.name == "right" || hit.collider.name == "left" || hit.collider.name == "down" || hit.collider.name == "up")
        {
            ChangePlayer();
            cube.transform.position = hit.collider.gameObject.transform.position; 
            switch (hit.collider.name)
            {
                case "right":
                    PushAndSwap(cubeIndex, cubeIndex + 5 - cube.col, 1, -21, 0, 0, 1, 0, true);   //change the plus 5           
                    PushBitboard("right", cubeIndex, cubeIndex + 4 - cube.col, 1);
                    break;
                case "left":
                    PushAndSwap(cubeIndex - cube.col, cubeIndex + 1, 1, 21, 0, 0, -1, cube.col, false);      //minus 4         
                    PushBitboard("left", cubeIndex, cubeIndex - cube.col, 1);
                    break;
                case "down":
                    PushAndSwap(cubeIndex, cubeIndex + 5 * (5 - cube.row), 5, 0, 21, 0, 5, 0, true);            
                    PushBitboard("down", cubeIndex, cubeIndex + 5 * (4 - cube.row), 5);
                    break;
                case "up":
                    PushAndSwap(cubeIndex - 5 * cube.row, cubeIndex + 1, 5, 0, -21, 0, -5, cube.row, false); 
                    PushBitboard("up", cubeIndex, cubeIndex - 5 * cube.row, 5);                  
                    break;
                default:
                    break;
            }
            if (Win())
            {
                //print("WIN!");
            }
        }
        else
        {
            cube.transform.position = cube.position;
            if (IsPossibleCube(cubeIndex, playerTurn))
            //{
                //if(!isMoving)
                    StartCoroutine(Roll(playerTurn * -1, cube));
            //}
        }
    }
}