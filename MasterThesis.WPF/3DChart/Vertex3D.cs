// A dot in 3D space
// version 0.1

namespace MasterThesis.WPF
{
    public class Vertex3D
    {
        public System.Windows.Media.Color color;    // color of the dot
        public float x, y, z;                       // location of the dot
        public int nMinI, nMaxI;                    // link to the viewport positions array index
        public bool selected = false;               // is this dot selected by user
    }
}
