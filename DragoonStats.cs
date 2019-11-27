using System;
using System.Collections.Generic;
using System.Text;

namespace LoDDict_csharp
{

    public class DragoonStats
    {
        int dat = 0;
        int dmat = 0;
        int ddf = 0;
        int dmdf = 0;

        public int DAT
        {
            get { return dat; }
        }

        public int DMAT
        {
            get { return dmat; }
        }

        public int DDF
        {
            get { return ddf; }
        }

        public int DMDF
        {
            get { return dmdf; }
        }

        public DragoonStats()
        {

        }
        public DragoonStats(int ndat, int ndmat, int nddf, int ndmdf)
        {
            dat = ndat;
            dmat = ndmat;
            ddf = nddf;
            dmdf = ndmdf;
        }
    }
}
