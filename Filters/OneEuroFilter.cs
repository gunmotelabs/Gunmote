// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//http://www.lifl.fr/~casiez/1euro/

namespace WiiTUIO.Filters
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a OneEuroFilter.
    /// </summary>
    public class OneEuroFilter
    {
        public OneEuroFilter(double minCutoff, double beta, double dcutoff)
        {
            firstTime = true;
            this.minCutoff = minCutoff;
            this.beta = beta;

            xFilt = new LowpassFilter();
            dxFilt = new LowpassFilter();
            this.dcutoff = dcutoff;
        }

        protected bool firstTime;
        protected double minCutoff;
        protected double beta;
        protected LowpassFilter xFilt;
        protected LowpassFilter dxFilt;
        protected double dcutoff;

        public double MinCutoff
        {
            get { return minCutoff; }
            set { minCutoff = value; }
        }

        public double Beta
        {
            get { return beta; }
            set { beta = value; }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Filter.
        /// </summary>
        public double Filter(double x, double rate)
        {
            double dx = firstTime ? 0 : (x - xFilt.Last()) * rate;
            if (firstTime)
            {
                firstTime = false;
            }

            var edx = dxFilt.Filter(dx, Alpha(rate, dcutoff));
            var cutoff = minCutoff + beta * Math.Abs(edx);

            return xFilt.Filter(x, Alpha(rate, cutoff));
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Alpha.
        /// </summary>
        protected double Alpha(double rate, double cutoff)
        {
            var tau = 1.0 / (2 * Math.PI * cutoff);
            var te = 1.0 / rate;
            return 1.0 / (1.0 + tau / te);
        }
    }
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a LowpassFilter.
    /// </summary>
    public class LowpassFilter
    {
        public LowpassFilter()
        {
            firstTime = true;
        }

        protected bool firstTime;
        protected double hatXPrev;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Last.
        /// </summary>
        public double Last()
        {
            return hatXPrev;
        }

        public double Filter(double x, double alpha)
        {
            double hatX = 0;
            if (firstTime)
            {
                firstTime = false;
                hatX = x;
            }
            else
                hatX = alpha * x + (1 - alpha) * hatXPrev;

            hatXPrev = hatX;

            return hatX;
        }
    }
}
