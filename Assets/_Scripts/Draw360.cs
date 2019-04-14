using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace _Script
{
    public class Draw360 : MonoBehaviour
    {

        private LineRenderer line;

        public int lineCounts = 360;

        private void Start()
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = lineCounts * 2;

        }

        private void Update()
        {
            for (int i = 0; i < lineCounts; ++i)
            {
                float angle = i * 360.0f / lineCounts;
                float radius = angle / 180.0f * PI;
                Vector3 direction = new Vector3(Cos(radius), Sin(radius), 0.0f);

                Ray ray = new Ray(this.transform.position, direction);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                int min_index = 0;
                float min_len = Mathf.Infinity;

                for (int index = 0; index < hits.Length; ++index)
                {
                    var point = hits[index].point;
                    var x = point.x - transform.position.x;
                    var y = point.y - transform.position.y;
                    float cur_len = x * x + y * y;

                    // 如果当前长度小于最小长度
                    if (cur_len < min_len)
                    {
                        min_len = cur_len;
                        min_index = index;
                    }
                }

                line.SetPosition(i * 2, this.transform.position);
                line.SetPosition(i * 2 + 1, hits[min_index].point);
            }
        }
    }

}

