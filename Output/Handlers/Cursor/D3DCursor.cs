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
using System.Windows;
using System.Windows.Media;

namespace WiiTUIO.Output
{
    /// <summary>
    /// Gestiona el cursor Direct3D usado para mostrar el puntero de Gunmote con menor latencia visual.
    /// </summary>
    public class D3DCursor
    {
        public int X, Y, ID;
        public bool Hidden, Pressed;
        public Color Color;
        /// <summary>
        /// Gestiona el cursor Direct3D usado para mostrar el puntero de Gunmote con menor latencia visual.
        /// </summary>
        public D3DCursor(int id, Color color)
        {
            Color = color;
            ID = id;
            X = 0;
            Y = 0;
            Hidden = false;
            Pressed = false;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Hide.
        /// </summary>
        public void Hide()
        {
            this.Hidden = true;
        }

        public void Show()
        {
            this.Hidden = false;
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        public void SetColor(Color color)
        {
            this.Color = color;
        }

        public void SetPosition(Point point)
        {
            this.X = (int)point.X;
            this.Y = (int)point.Y;
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        public void SetReleased()
        {
            this.Pressed = false;
        }

        public void SetPressed()
        {
            this.Pressed = true;
        }
    }
}
