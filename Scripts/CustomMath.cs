using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Sangmin.Web2018
{
    public class CustomMath
    {
        /// <summary>
        /// Returns position that surrounds the starting position. 
        ///   2 3 4
        ///   1 o 5
        /// 9 8 7 6
        /// </summary>
        /// <param name="startPos">Surrounding this position.</param>
        /// <param name="trial">How many times snail calculate.</param>
        /// <param name="offset">offset 'start position' to 'surround position'.</param>
        /// <returns></returns>
        public static Vector3 SnailPosition(Vector3 startPos, int trial, float offset)
        {
            float _x = startPos.x - offset;
            float _z = startPos.z;

            int _depth = 1;
            int _countX = 0;
            int _countZ = 1;
            bool _positive = false;
            for (int i = 0; i < trial; i++)
            {
                if (_countX > 0)
                {
                    _countX--;
                    if (_positive)
                    {
                        _x += offset;
                    }
                    else
                    {
                        _x -= offset;
                    }
                }
                else if (_countZ > 0)
                {
                    _countZ--;
                    if (_positive)
                    {
                        _z += offset;
                    }
                    else
                    {
                        _z -= offset;
                    }
                }
                else
                {
                    _depth++;
                    _countZ = _depth;
                    _countX = _depth;
                    _positive = !_positive;
                    i--;
                }
            }

            return new Vector3(_x, startPos.y, _z);
        }
    }
}