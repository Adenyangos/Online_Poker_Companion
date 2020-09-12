using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// A custom Chart class. The only thing I added to the Chart class was the ability to save two indicies, "I" and "J".
    /// </summary>
    class MyChart : System.Windows.Forms.DataVisualization.Charting.Chart
    {
        /// <summary>
        /// The index of the set of 4 charts representing the data for one player
        /// </summary>
        public int I { get; set; }

        /// <summary>
        /// The index of the specific chart within the set of 4 charts
        /// </summary>
        public int J { get; set; }
    }
}