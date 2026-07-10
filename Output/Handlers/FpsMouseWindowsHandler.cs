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
using WiiTUIO.Provider;

namespace WiiTUIO.Output.Handlers
{
    // Wrapper: usa MouseHandler (SendInput) pero SOLO para "fpsmouse"
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a FpsMouseWindowsHandler.
    /// </summary>
    public class FpsMouseWindowsHandler : ICursorHandler
    {
        private readonly MouseHandler inner = new MouseHandler();

        public bool setPosition(string key, CursorPos cursorPos)
        {
            if (!key.Equals("fpsmouse"))
                return false;

            return inner.setPosition(key, cursorPos);
        }

        public bool connect() => inner.connect();
        public bool disconnect() => inner.disconnect();
        public bool reset() => inner.reset();
        public bool startUpdate() => inner.startUpdate();
        public bool endUpdate() => inner.endUpdate();
    }
}
