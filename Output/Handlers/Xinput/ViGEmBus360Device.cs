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
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace WiiTUIO.Output.Handlers.Xinput
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a ViGEmBus360Device.
    /// </summary>
    public class ViGEmBus360Device
    {
        public const int outputResolution = 32767 - (-32768);

        private IXbox360Controller cont;
        public event Action<byte, byte> OnRumble;

        public IXbox360Controller Cont { get => cont; }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a ViGEmBus360Device.
        /// </summary>
        public ViGEmBus360Device(ViGEmClient client)
        {
            cont = client.CreateXbox360Controller();
            cont.AutoSubmitReport = false;
            cont.FeedbackReceived += FeedbackProcess;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a FeedbackProcess.
        /// </summary>
        private void FeedbackProcess(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            OnRumble?.Invoke(e.LargeMotor, e.SmallMotor);
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        public bool Connect()
        {
            cont.Connect();
            return true;
        }

        public bool Disconnect()
        {
            cont.FeedbackReceived -= FeedbackProcess;
            cont.Disconnect();
            cont = null;
            //cont.Dispose();
            return true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Update.
        /// </summary>
        public bool Update()
        {
            cont.SubmitReport();
            return true;
        }

        public void Reset()
        {
            cont.ResetReport();
            //report = new Xbox360Report();
        }
    }
}
