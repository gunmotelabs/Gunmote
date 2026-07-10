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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WiiTUIO
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a SystemProcessMonitor.
    /// </summary>
    class SystemProcessMonitor : IDisposable
    {
        /// <summary>
        /// The GetForegroundWindow function returns a handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public Action<ProcessChangedEvent> ProcessChanged;

        private uint lastProcessId = 0;

        private System.Timers.Timer pollingTimer;
        private bool inEvent;

        private static SystemProcessMonitor defaultInstance;
        public static SystemProcessMonitor Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new SystemProcessMonitor();
                }
                return defaultInstance;
            }
        }

        private SystemProcessMonitor()
        {
            pollingTimer = new System.Timers.Timer();
            pollingTimer.Interval = 500;
            pollingTimer.Elapsed += pollingTimer_Elapsed;
            this.Start();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a pollingTimer_Elapsed.
        /// </summary>
        private void pollingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            inEvent = true;

            IntPtr foregroundWindow = GetForegroundWindow();
            uint procId = 0;
            GetWindowThreadProcessId(foregroundWindow, out procId);

            if (procId != lastProcessId)
            {
                Process process = Process.GetProcessById((int)procId);
                if (ProcessChanged != null && process != null && process.Id > 0)
                {
                    this.ProcessChanged(new ProcessChangedEvent(process));
                }
                this.lastProcessId = procId;
            }

            inEvent = false;
        }
        /// <summary>
        /// Devuelve un valor calculado o configurado utilizado por la lógica específica de Gunmote.
        /// </summary>
        public Process GetLastProcess()
        {
            Process process = null;
            if (lastProcessId > 0)
            {
                process = Process.GetProcessById((int)lastProcessId);
            }

            return process;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Start.
        /// </summary>
        public void Start()
        {
            pollingTimer.Start();
        }

        public void Stop()
        {
            pollingTimer.Elapsed -= pollingTimer_Elapsed;

            while (inEvent)
            {
                Thread.SpinWait(500);
            }

            pollingTimer.Stop();
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            pollingTimer.Dispose();
        }
    }
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a ProcessChangedEvent.
    /// </summary>
    public class ProcessChangedEvent
    {
        public Process Process;

        public ProcessChangedEvent(Process process)
        {
            this.Process = process;
        }
    }
}
