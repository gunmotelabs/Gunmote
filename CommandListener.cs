// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using System;
using System.Windows;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;

namespace WiiTUIO
{
    /// <summary>
    /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
    /// </summary>
    internal class CommandListener
    {
        public Action<string> OnKeymapRequested;

        private static CommandListener defaultInstance;

        public static CommandListener Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new CommandListener();
                }
                return defaultInstance;
            }
        }

        private CommandListener()
        {
            Task.Run(() => ListenForCommands());
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private async Task ListenForCommands()
        {
            while (true)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream("Gunmote"))
                {
                    await server.WaitForConnectionAsync();

                    using (StreamReader reader = new StreamReader(server))
                    {
                        string pipeMessage = await reader.ReadLineAsync();
                        if (pipeMessage != null)
                        {
                            string[] splitMessage = pipeMessage.Split(Convert.ToChar(31));
                            string command = splitMessage[0];
                            string value = splitMessage.Length > 1 ? splitMessage[1] : null;
                            Console.WriteLine($"Received command: {command} with value {value}");
                            HandleCommand(command, value);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void HandleCommand(string command, string value)
        {
            if (command == "exit")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
            else if (command == "keymap")
            {
                Console.WriteLine("Received keymap: " + value);
                OnKeymapRequested?.Invoke(value);
            }
        }
    }
}
