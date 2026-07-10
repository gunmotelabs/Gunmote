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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WiiTUIO.Provider;

namespace WiiTUIO
{
    /// <summary>
    /// Interaction logic for LayoutSelectionRow.xaml
    /// </summary>
    public partial class KeymapOutputRow : UserControl
    {
        public Action<Adorner> OnDragStart;
        public Action<Adorner> OnDragStop;

        public KeymapOutputRow(KeymapOutput output)
        {
            InitializeComponent();

            KeymapOutputItem item = new KeymapOutputItem(output);
            item.OnDragStart += item_OnDragStart;
            item.OnDragStop += item_OnDragStop;

            this.mainGrid.Children.Add(item);

        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a item_OnDragStop.
        /// </summary>

        private void item_OnDragStop(Adorner adorner)
        {
            if (OnDragStop != null)
            {
                OnDragStop(adorner);
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a item_OnDragStart.
        /// </summary>
        private void item_OnDragStart(Adorner adorner)
        {
            if (OnDragStart != null)
            {
                OnDragStart(adorner);
            }
        }
    }
}
