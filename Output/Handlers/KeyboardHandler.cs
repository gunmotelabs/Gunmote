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
using WindowsInput;
using WindowsInput.Native;

namespace WiiTUIO.Output.Handlers
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a KeyboardHandler.
    /// </summary>
    public class KeyboardHandler : IButtonHandler
    {
        private InputSimulator inputSimulator;

        private HashSet<VirtualKeyCode> keysDown;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a KeyboardHandler.
        /// </summary>
        public KeyboardHandler()
        {
            this.inputSimulator = new InputSimulator();

            this.keysDown = new HashSet<VirtualKeyCode>();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a reset.
        /// </summary>
        public bool reset()
        {
            foreach(VirtualKeyCode keyCode in keysDown)
            {
                this.inputSimulator.Keyboard.KeyUp(keyCode);
            }
            return true;
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        public bool setButtonDown(string key)
        {
            if (Enum.IsDefined(typeof(VirtualKeyCode), key.ToUpper()))
            {
                VirtualKeyCode theKeyCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), key, true);
                this.inputSimulator.Keyboard.KeyDown(theKeyCode);
                this.keysDown.Add(theKeyCode);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        public bool setButtonUp(string key)
        {
            if (Enum.IsDefined(typeof(VirtualKeyCode), key.ToUpper()))
            {
                VirtualKeyCode theKeyCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), key, true);
                this.inputSimulator.Keyboard.KeyUp(theKeyCode);
                this.keysDown.Remove(theKeyCode);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        public bool connect()
        {
            return true;
        }

        public bool disconnect()
        {
            return true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a startUpdate.
        /// </summary>
        public bool startUpdate()
        {
            return true;
        }

        public bool endUpdate()
        {
            return true;
        }
    }
}
