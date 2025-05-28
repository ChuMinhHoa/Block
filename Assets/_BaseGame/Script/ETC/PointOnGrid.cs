using UnityEngine;

namespace _BaseGame.Script.ETC
{
    [System.Serializable]
    public class PointOnGrid
    {
        public int x;
        public int y;
        
        public void SetPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
