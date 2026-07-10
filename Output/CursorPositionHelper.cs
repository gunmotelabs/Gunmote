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
using WiiTUIO.Properties;
using WiiTUIO.Provider;
using WiiTUIO.Filters;
using Microsoft.Win32;

namespace WiiTUIO.Output
{
    /// <summary>
    /// This helper class transforms absolute position of Wii pointer to relative position. 
    /// x=0.5, y=0.5 means center of the screen.
    /// </summary>
    class CursorPositionHelper
    {
        private SmoothingBuffer smoothingBuffer;
        private SmoothingBuffer lightbarSmoothingBuffer;
        private System.Drawing.Rectangle screenBounds;
        
        /// <summary>
        
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        
        /// </summary>
        
        public CursorPositionHelper()
        {
            smoothingBuffer = new SmoothingBuffer(Settings.Default.pointer_positionSmoothing);
            lightbarSmoothingBuffer = new SmoothingBuffer(Settings.Default.pointer_positionSmoothing);
            screenBounds = DeviceUtils.DeviceUtil.GetScreen(Settings.Default.primaryMonitor).Bounds;

            Settings.Default.PropertyChanged += SettingsChanged;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void SettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "primaryMonitor")
            {
                screenBounds = DeviceUtils.DeviceUtil.GetScreen(Settings.Default.primaryMonitor).Bounds;
            }
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            screenBounds = DeviceUtils.DeviceUtil.GetScreen(Settings.Default.primaryMonitor).Bounds;
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public Point getSmoothedPosition(Point relativePosition)
        {
            Vector vec = new Vector(relativePosition.X, relativePosition.Y);
            return new Point(vec.X, vec.Y);
        }   
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public Point getRelativePosition(Point absPosition)
        {
            Vector vec = new Vector(absPosition.X, absPosition.Y);
            return new Point(vec.X / screenBounds.Width, vec.Y / screenBounds.Height);
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public Point getFPSRelativePosition(Point relativeMarginPosition)
        {
            smoothingBuffer.addValue(new Vector(relativeMarginPosition.X, relativeMarginPosition.Y));
            Vector smoothedVec = smoothingBuffer.getSmoothedValue();
            return new Point(smoothedVec.X, smoothedVec.Y);
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public Point GetLightbarRelativePosition(Point lightbarPos)
        {
            lightbarSmoothingBuffer.addValue(new Vector(lightbarPos.X, lightbarPos.Y));
            Vector smoothedVec = lightbarSmoothingBuffer.getSmoothedValue();
            return new Point(smoothedVec.X, smoothedVec.Y);
        }
    }
}
