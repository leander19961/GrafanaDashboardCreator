using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaDashboardCreator.Helper
{
    internal class GridPos
    {
        //This is for positioning of panels and rows
        private int x = 0;
        private int y = 0;

        internal int X { get { return x; } }
        internal int Y { get { return y; } }

        internal void IncrementGridPos(int gridIncrementForX, int gridIncrementForY)
        {
            if (x != 0)
            {
                x = 0;
                y += gridIncrementForY;
            }
            else if (x == 0)
            {
                x = gridIncrementForX;
            }
        }

        internal void NewRow(int gridIncrementForY)
        {
            if (x != 0)
            {
                x = 0;
                y += gridIncrementForY;
            }
        }

        internal void NewRowInner()
        {
            y += 1;
        }
    }
}
