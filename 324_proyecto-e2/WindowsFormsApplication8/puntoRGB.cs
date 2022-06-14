using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication8
{
    [Serializable]
    class puntoRGB
    {
        public int R;
        public int G;
        public int B;
        public puntoRGB(int a, int b, int c)
        {
            R = a;
            G = b;
            B = c;
        }
    }
    
}


