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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Nefarius.ViGEm.Client;

namespace WiiTUIO.Output.Handlers.Xinput
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a ViGEmBusClient.
    /// </summary>
    public class ViGEmBusClient
    {
        private ViGEmClient vigemTestClient = null;
        public ViGEmClient VigemTestClient => vigemTestClient;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a ViGEmBusClient.
        /// </summary>
        public ViGEmBusClient()
        {
            try
            {
                vigemTestClient = new ViGEmClient();
            }
            catch (Exception) { }
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        public void Disconnect()
        {
            if (vigemTestClient != null)
            {
                // Allow some time for controllers to disconnect
                // before disconnecting from ViGEmBus
                Thread.Sleep(500);

                vigemTestClient.Dispose();
                vigemTestClient = null;
            }
        }
    }
}
