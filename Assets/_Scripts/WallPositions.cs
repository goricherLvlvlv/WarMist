using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using static UnityEngine.Mathf;
using static System.Convert;

public class WallPositions : MonoBehaviour
{
    #region CONST_VALUE

    public const int RECT_LINES = 4;
    public const int TRIANGLE_LINES = 3;
    public const int STRAIGHT_LINES = 2;

    #endregion

    public LineRenderer line;
    public GameObject player;

    private List<WallPos> walls;
    private Vector3[] emptyVector3s;

    void Start()
    {
        walls = new List<WallPos>();
        line.positionCount = (transform.childCount + 1) * RECT_LINES * STRAIGHT_LINES;  // Walls的数量加上最外围的墙
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
            Vector3 point_00_min, point_00_max;
            Vector3 point_01_min, point_01_max;
            Vector3 point_10_min, point_10_max;
            Vector3 point_11_min, point_11_max;

            // 射线检测
            bool res_00 = DrawLine(walls[i].pos_00, i * 4, out point_00_min, out point_00_max);
            bool res_01 = DrawLine(walls[i].pos_01, i * 4 + 1, out point_01_min, out point_01_max);
            bool res_10 = DrawLine(walls[i].pos_10, i * 4 + 2, out point_10_min, out point_10_max);
            bool res_11 = DrawLine(walls[i].pos_11, i * 4 + 3, out point_11_min, out point_11_max);

            var trueCounts = ToInt32(res_00) + ToInt32(res_01) + ToInt32(res_10) + ToInt32(res_11);
            

            #region LINE_EXTEND_JUDGE_TRIPLE_TRUE

            if (trueCounts == 3)
            {
                if (res_00 && res_11)
                {
                    // 都是true, 则都延长
                    line.SetPosition((i * 4) * 2 + 1, point_00_max);
                    line.SetPosition((i * 4 + 3) * 2 + 1, point_11_max);
                }
                else
                {
                    // 一个是false, 一个是true
                    if (res_00)
                    {
                        // res_00是true, 00画线, 但不延长
                        line.SetPosition((i * 4) * 2 + 1, point_00_min);
                    }
                    else
                    {
                        line.SetPosition((i * 4 + 3) * 2 + 1, point_11_min);
                    }
                }

                if (res_01 && res_10)
                {
                    // 都是true, 则都延长
                    line.SetPosition((i * 4 + 1) * 2 + 1, point_01_max);
                    line.SetPosition((i * 4 + 2) * 2 + 1, point_10_max);
                }
                else
                {
                    // 一个是false, 一个是true
                    if (res_01)
                    {
                        // res_01是true, 01画线, 但不延长
                        line.SetPosition((i * 4 + 1) * 2 + 1, point_01_min);
                    }
                    else
                    {
                        line.SetPosition((i * 4 + 2) * 2 + 1, point_10_min);
                    }
                }

            }

            #endregion

            #region LINE_EXTEND_JUDGE_DOUBLE_TRUE

            else if (trueCounts <= 2)
            {
                if (res_00)
                {
                    line.SetPosition((i * 4) * 2 + 1, point_00_max);
                }

                if (res_01)
                {
                    line.SetPosition((i * 4 + 1) * 2 + 1, point_01_max);
                }

                if (res_10)
                {
                    line.SetPosition((i * 4 + 2) * 2 + 1, point_10_max);
                }

                if (res_11)
                {
                    line.SetPosition((i * 4 + 3) * 2 + 1, point_11_max);
                }
            }

            #endregion

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

    bool DrawLine(Vector3 pos, int i, out Vector3 point_min, out Vector3 point_max)
    {
        Ray ray = new Ray(player.transform.position, pos - player.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int min_index = 0;
        float min_len = Mathf.Infinity;

        int max_index = 0;
        float max_len = 0.0f;

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

            if (cur_len > max_len)
            {
                max_len = cur_len;
                max_index = index;
            }
        }

        point_min = hits[min_index].point;
        point_max = hits[max_index].point;

        line.SetPosition(i * 2, player.transform.position);
        // 本层if来判断是否应该画出线段
        if (hits[min_index].point == pos)
        {
            return true;
        }
        else
        {
            line.SetPosition(i * 2 + 1, player.transform.position);
            return false;
        }
    }

    void DrawLine(Vector3 pos, int i)
    {
        Ray ray = new Ray(player.transform.position, pos - player.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int min_index = 0;
        float min_len = Mathf.Infinity;

        int max_index = 0;
        float max_len = 0.0f;

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

            if (cur_len > max_len)
            {
                max_len = cur_len;
                max_index = index;
            }
        }

        line.SetPosition(i * 2, player.transform.position);
        // 本层if来判断是否应该画出线段
        if (hits[min_index].point == pos)
        {
            line.SetPosition(i * 2 + 1, hits[min_index].point);
        }
        else
        {
            line.SetPosition(i * 2 + 1, player.transform.position);
        }
    }

    #endregion

}
