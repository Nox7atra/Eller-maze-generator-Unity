using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.Utils
{
    public static class MathTools
    {
        //Возвращает 2 в степени power
        public static int TwoPowX(int power)
        {
            return (1 << power);
        }
        public static string To4Bin(int value)
        {
            var result = new string [4];
            for(int i = 0; i < 4; i++)
            {                
                result[i] = (value & 1) != 0 ? "1" : "0";
                value = value >> 1;
            }
            return result[3] +
                result[2] +
                result[1] +
                result[0];
        }
        public static Vector3 Vec2XYToVec3XZ(this Vector2 vec2)
        {
            return new Vector3(vec2.x, 0, vec2.y);
        }
    }
}