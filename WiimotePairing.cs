// -----------------------------------------------------------------------------
// Nuevo desarrollo y modificaciones Gunmote:
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

namespace WiiTUIO
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a WiimotePairing.
    /// </summary>
    class WiimotePairing
    {

        public Action WiimotePaired;
        public Action WiimotePairingStart;
        public Action WiimotePairingStop;
        public Action<string> WiimoteFound;
        public Action<string> WiimoteRemoved;
        /// <summary>
        /// Devuelve un valor calculado o configurado utilizado por la lógica específica de Gunmote.
        /// </summary>
        public List<string> getPairedWiimotes()
        {
            return new List<string>();
        }

        public void start()
        {

            WiimotePairingStart();
        }
        /// <summary>
        /// Cierra la conexión o detiene el componente externo asociado de forma controlada.
        /// </summary>
        public void stop()
        {

            WiimotePairingStop();
        }
    }
}
