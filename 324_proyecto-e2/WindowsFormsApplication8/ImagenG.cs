using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace WindowsFormsApplication8
{
    [Serializable]
    class ImagenG
    {
        public Bitmap btmImagenOriginal;
        public Bitmap btmImagenCambiada;
        public puntoRGB[] puntos = new puntoRGB[3];
        public ImagenG(Bitmap btm, Bitmap btm2, puntoRGB a, puntoRGB b, puntoRGB c)
        {
            btmImagenOriginal = btm;
            btmImagenCambiada = btm2;
            puntos[0] = a;
            puntos[1] = b;
            puntos[2] = c;
        }
    }
}
