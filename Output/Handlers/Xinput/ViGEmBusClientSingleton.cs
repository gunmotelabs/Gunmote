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
using System.Threading.Tasks;

namespace WiiTUIO.Output.Handlers.Xinput
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a ViGEmBusClientSingleton.
    /// </summary>
    public static class ViGEmBusClientSingleton
    {
        private static ViGEmBusClient defaultInstance;

        public static ViGEmBusClient Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new ViGEmBusClient();
                }

                return defaultInstance;
            }
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        public static void Disconnect()
        {
            if (defaultInstance != null)
            {
                defaultInstance.Disconnect();
                defaultInstance = null;
            }
        }
    }
}
