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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WiiCPP; // Asegúrate de que este using está, ya que lo necesitas para 'MonitorInfo' y 'Monitors'

namespace WiiTUIO.DeviceUtils
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a DeviceUtil.
    /// </summary>
    class DeviceUtil
    {
        // --- MÉTODOS ORIGINALES (sin cambios) ---

        public static IEnumerable<HidLibrary.HidDevice> GetHidList()
        {
            return HidLibrary.HidDevices.Enumerate();
        }
        /// <summary>
        /// Devuelve un valor calculado o configurado utilizado por la lógica específica de Gunmote.
        /// </summary>
        public static IEnumerable<MonitorInfo> GetMonitorList()
        {
            return new List<MonitorInfo>(Monitors.enumerateMonitors());
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        public static Screen GetScreen(string devicePath)
        {
            MonitorInfo primaryMonitorInfo = GetMonitorList().FirstOrDefault(info => info.DevicePath == devicePath);

            if (primaryMonitorInfo != null)
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.DeviceName == primaryMonitorInfo.DeviceName)
                    {
                        return screen;
                    }
                }
            }
            return Screen.PrimaryScreen;
        }

        // --- NUEVO MÉTODO Y CÓDIGO AUXILIAR ---
        /// <summary>
        /// Obtiene el identificador de hardware Plug and Play (PNP) de un monitor específico.
        /// </summary>
        /// <param name="monitorDeviceName">El nombre técnico del monitor (ej: "\\.\DISPLAY1").</param>
        /// <returns>El ID del PNP o null si no se encuentra.</returns>
        /// <summary>
        /// Devuelve un valor calculado o configurado utilizado por la lógica específica de Gunmote.
        /// </summary>
        public static string GetMonitorPnpDeviceId(string monitorDeviceName)
        {
            var dev = new DISPLAY_DEVICE();
            dev.cb = Marshal.SizeOf(dev);

            if (!EnumDisplayDevices(monitorDeviceName, 0, ref dev, 0))
            {
                return null;
            }

            var pnpDeviceId = dev.DeviceID.Split('\\');
            if (pnpDeviceId.Length > 1)
            {
                return pnpDeviceId[1];
            }

            return null;
        }

        // --- CÓDIGO INTERNO PARA LA API DE WINDOWS (P/Invoke) ---

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }
    }
}