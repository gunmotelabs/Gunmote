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
using System.Windows.Media;

namespace WiiTUIO
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a KeymapColors.
    /// </summary>
    public class KeymapColors
    {

        public static Color GetColor(KeymapOutputType type)
        {
            switch (type)
            {
                case KeymapOutputType.KEYBOARD:
                    return Colors.Orange;
                case KeymapOutputType.MOUSE:
                    return Colors.OrangeRed;
                case KeymapOutputType.XINPUT:
                    return Colors.Green;
                case KeymapOutputType.WIIMOTE:
                    return Colors.DodgerBlue;
                case KeymapOutputType.CURSOR:
                    return Colors.Purple;
                case KeymapOutputType.DISABLE:
                    return Colors.Black;
                default:
                    return Colors.Black;
            }
        }

    }
}
