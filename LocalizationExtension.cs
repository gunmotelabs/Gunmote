// -----------------------------------------------------------------------------
// Nuevo desarrollo y modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using System;
using System.Reflection;
using System.Resources;
using System.Windows.Markup; // Necesario para MarkupExtension
using System.Globalization; // Necesario para CultureInfo

namespace WiiTUIO// ¡IMPORTANTE! Asegúrate de que este sea el Namespace de tu proyecto
{
    /// <summary>
    /// Extensión de marcado usada para resolver textos localizados en la interfaz.
    /// </summary>
    public class LocalizationExtension : MarkupExtension
    {
        public string Name { get; set; }

        public LocalizationExtension(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a ProvideValue.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Ajusta el Namespace.NombreDelArchivoDeRecursos según dónde estén tus archivos .resx
            // Si tus archivos .resx están en la carpeta 'Properties', usa "DriverInstall.Properties.Resources"
            // Si están directamente en la raíz del proyecto, usa "WiiTUIO.Resources"
            ResourceManager resourceManager = new ResourceManager("WiiTUIO.Resources.Resources", Assembly.GetExecutingAssembly());

            // Obtiene la cadena del recurso para la cultura actual de la UI
            string localizedString = resourceManager.GetString(Name, CultureInfo.CurrentUICulture);

            // Si no se encuentra la cadena, devuelve la clave para facilitar la depuración
            if (localizedString == null)
            {
                return $"!{Name}!";
            }

            return localizedString;
        }
    }
}