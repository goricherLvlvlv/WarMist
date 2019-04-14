using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class DrawByRectangle : MonoBehaviour
{
    #region CONST_VALUE

    private const int RECT_LINES = 4;
    private const int TRIANGLE_LINES = 3;
    private const int STRAIGHT_LINES = 2;

    #endregion
    
    public GameObject walls;
    public LineRenderer line;

    private List<WallPoints> wallPointsList;
    
    private void Start()
    {
        line.positionCount = (walls.transform.childCount + 1) * RECT_LINES * STRAIGHT_LINES;
        wallPointsList = new List<WallPoints>();

        /*
         *    计算点在旋转过后的坐标
         *    填充到wallPointsList当中
         */
        for (int i = 0; i < walls.transform.childCount; ++i)
        {
            var wallPoints = new WallPoints();
            var wall = walls.transform.GetChild(i);
            var x = wall.localScale.x / 2;
            var y = wall.localScale.y / 2;
            var angle = wall.localEulerAngles.z * PI / 180.0f;

            var position = wall.position;
            wallPoints.Point00 = GetPointByRotation2D(-x, -y, angle, position);
            wallPoints.Point01 = GetPointByRotation2D(-x, y, angle, position);
            wallPoints.Point10 = GetPointByRotation2D(x, -y, angle, position);
            wallPoints.Point11 = GetPointByRotation2D(x, y, angle, position);
            
            wallPointsList.Add(wallPoints);
        }
    }

    private void Update()
    {
        for (int i = 0; i < walls.transform.childCount; ++i)
        {
            
        }
    }

    Vector3 GetPointByRotation2D(float x, float y, float angle, Vector3 origin)
    {
        return new Vector3(
            x * Cos(angle) - y * Sin(angle) + origin.x,
            y * Cos(angle) + x * Sin(angle) + origin.y,
            0.0f
            );
    }

    void DrawLine(Vector3 pos, int i)
    {
        Vector3 playerPos = this.transform.position;
        Ray ray = new Ray(playerPos, pos - playerPos);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int minIndex = 0;
        float minLength = Infinity;

        for (int index = 0; index < hits.Length; ++index)
        {
            var point = hits[index].point;
        }
    }
    
    private struct WallPoints
    {
        public Vector3 Point00;
        public Vector3 Point01;
        public Vector3 Point10;
        public Vector3 Point11;
    }
}
