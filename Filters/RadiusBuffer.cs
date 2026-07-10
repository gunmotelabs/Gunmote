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
using System.Windows;
using WiiTUIO.Properties;

namespace WiiTUIO.Filters
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a RadiusBuffer.
    /// </summary>
    class RadiusBuffer : CircularBuffer<Vector> {
        private double Radius = Settings.Default.pointer_positionRadius;

        private void SetValue(Vector newOne) {
            Vector d = newOne - Value;
            if (d.Length > Radius) {

                Vector D = d;
                D.Normalize();

                d -= (D * Radius);
                Value += d;
            }
        }
        public Vector Value { get; protected set; }

        public override Vector this[int index] {
            get {
                return base[index];
            }
            set {
                SetValue(value);
                base[index] = value;
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a RadiusBuffer.
        /// </summary>
        public RadiusBuffer(int count)
            : base(count, new Vector(0, 0)) {
                Value = new Vector(0, 0);
        }

        public override void Add(Vector item) {
            SetValue(item);
            base.Add(item);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a AddAndGet.
        /// </summary>
        public Vector AddAndGet(Vector item) {
            Add(item);
            return Value;
        }

    }

}
