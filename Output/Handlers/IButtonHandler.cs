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

namespace WiiTUIO.Output.Handlers
{
    public interface IButtonHandler : IOutputHandler
    {
        //Return true if it has been handled
        bool setButtonDown(string key);
        bool setButtonUp(string key);

    }
}
