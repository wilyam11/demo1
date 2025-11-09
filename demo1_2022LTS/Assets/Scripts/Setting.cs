using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Setting
    {
        public int[] HitKey = new int[4];

        public void SetKey(int k1, int k2, int k3, int k4)
        {
            HitKey[0] = k1;
            HitKey[1] = k2;
            HitKey[2] = k3;
            HitKey[3] = k4;
        }
    }
}
