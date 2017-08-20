using Nox7atra.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.Mazes.Generators
{
    public abstract class MazeGenerator
    {
        public abstract W4Maze Generate(int width, int height);
    }
}