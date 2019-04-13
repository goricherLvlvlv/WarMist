using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using static UnityEngine.Mathf;



public class WallPositions : MonoBehaviour
{
    public LineRenderer line;
    public GameObject player;

    private List<WallPos> walls;
    private Vector3[] emptyVector3s;

    void Start()
    {
        walls = new List<WallPos>();
        line.positionCount = transform.childCount * 4 * 2 + 4 * 2;
        emptyVector3s = new Vector3[line.positionCount];
        for (int i = 0; i < emptyVector3s.Length; ++i)
        {
            emptyVector3s[i] = Vector3.zero;
        }

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            // 点的计算
            WallPos wallPos = new WallPos();

            var wall = this.transform.GetChild(i);
            var tmp_x = wall.localScale.x / 2;
            var tmp_y = wall.localScale.y / 2;
            var angle = wall.localEulerAngles.z * PI / 180;

            wallPos.pos_00 = GetPositionByAngle3(-tmp_x, -tmp_y, angle, wall);
            wallPos.pos_01 = GetPositionByAngle3(-tmp_x, tmp_y, angle, wall);
            wallPos.pos_10 = GetPositionByAngle3(tmp_x, -tmp_y, angle, wall);
            wallPos.pos_11 = GetPositionByAngle3(tmp_x, tmp_y, angle, wall);

            walls.Add(wallPos);
        }
    }

    void Update()
    {
        line.SetPositions(emptyVector3s);

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            // 射线检测
            DrawLine(walls[i].pos_00, i * 4);
            DrawLine(walls[i].pos_01, i * 4 + 1);
            DrawLine(walls[i].pos_10, i * 4 + 2);
            DrawLine(walls[i].pos_11, i * 4 + 3);
        }

        DrawLine(new Vector3(-10.75f, -6.0f, 0.0f), transform.childCount * 4);
        DrawLine(new Vector3(-10.75f, 6.0f, 0.0f), transform.childCount * 4 + 1);
        DrawLine(new Vector3(10.75f, -6.0f, 0.0f), transform.childCount * 4 + 2);
        DrawLine(new Vector3(10.75f, 6.0f, 0.0f), transform.childCount * 4 + 3);
    }

    #region DATA_STRUCT

    // 墙体的平面坐标
    struct WallPos
    {
        public Vector3 pos_00;
        public Vector3 pos_01;
        public Vector3 pos_10;
        public Vector3 pos_11;
    }

    #endregion

    #region FUNCTION::POSITION_ROTATE_BY_ANGLE

    Vector2 GetPositionByAngle2(float tmp_x, float tmp_y, float angle, Transform wall)
    {
        float x = tmp_x * Cos(angle) - tmp_y * Sin(angle) + wall.localPosition.x;
        float y = tmp_y * Cos(angle) + tmp_x * Sin(angle) + wall.localPosition.y;

        return new Vector2(x, y);
    }

    Vector3 GetPositionByAngle3(float tmp_x, float tmp_y, float angle, Transform wall)
    {
        float x = tmp_x * Cos(angle) - tmp_y * Sin(angle) + wall.localPosition.x;
        float y = tmp_y * Cos(angle) + tmp_x * Sin(angle) + wall.localPosition.y;

        return new Vector3(x, y, 0.0f);
    }

    #endregion

    #region FUNCTION::DRAW_LINE

    void DrawLine(Vector3 pos, int i)
    {
        Ray ray = new Ray(player.transform.position, pos - player.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        int min_index = 0;
        float min_len = Mathf.Infinity;

        for (int index = 0; index < hits.Length; ++index)
        {
            var point = hits[index].point;
            float cur_len = point.x * point.x + point.y * point.y;

            // 如果当前长度小于最小长度
            if (cur_len < min_len)
            {
                min_len = cur_len;
                min_index = index;
            }
        }

        line.SetPosition(i * 2, player.transform.position);
        if (hits[min_index].point == pos)
        {
            line.SetPosition(i * 2 + 1, pos);
        }
        else
        {
            line.SetPosition(i * 2 + 1, player.transform.position);
        }
    }

    void DrawLineByPosition(Vector3 pos, int i)
    {
        Ray ray = new Ray(player.transform.position, pos - player.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        int min_index = 0;
        float min_len = Mathf.Infinity;

        for (int index = 0; index < hits.Length; ++index)
        {
            var point = hits[index].point;
            float cur_len = point.x * point.x + point.y * point.y;

            // 如果当前长度小于最小长度
            if (cur_len < min_len)
            {
                min_len = cur_len;
                min_index = index;
            }
        }

        line.SetPosition(i * 2, player.transform.position);
        if (hits[min_index].transform.position == pos)
        {
            line.SetPosition(i * 2 + 1, pos);
        }
        else
        {
            line.SetPosition(i * 2 + 1, player.transform.position);
        }
    }

    #endregion

}
