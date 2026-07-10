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
    public partial class KeymapOutputItem : UserControl
    {
        private KeymapOutput output;

        public Action<Adorner> OnDragStart;
        public Action<Adorner> OnDragStop;
        private TestAdorner adorner;
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        public KeymapOutputItem(KeymapOutput output)
        {
            InitializeComponent();
            this.output = output;
            this.tbName.Text = output.Name;

            this.border.Background = new SolidColorBrush(KeymapColors.GetColor(output.Type));
            //this.adorner.Visibility = Visibility.Hidden;

        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a border_MouseMove.
        /// </summary>
        private void border_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element != null && e.LeftButton == MouseButtonState.Pressed)
            {
                dragMe();
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a border_TouchMove.
        /// </summary>
        private void border_TouchMove(object sender, TouchEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element != null)
            {
                dragMe();
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a dragMe.
        /// </summary>
        private void dragMe()
        {
            this.adorner = new TestAdorner(this.border);
            this.adorner.IsHitTestVisible = false;
            if (OnDragStart != null)
            {
                OnDragStart(this.adorner);
            }
            DataObject data = new DataObject();
            data.SetData("KeymapOutput", this.output);
            data.SetData("KeymapOutputItem", this);
            DragDrop.DoDragDrop(this.border,
                                 data,
                                 DragDropEffects.Copy | DragDropEffects.Move);
            if (OnDragStop != null)
            {
                OnDragStop(this.adorner);
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a OnGiveFeedback.
        /// </summary>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            if (this.adorner != null)
            {
                Point curpos = MouseSimulator.GetCursorPosition();
                Point point = this.border.PointFromScreen(curpos);
                this.adorner.SetPosition(point.X, point.Y);
            }

            Cursor cursor = ((TextBlock)this.Resources["CursorClosedHand"]).Cursor;

            Mouse.SetCursor(cursor);
            e.Handled = true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a DropAccepted.
        /// </summary>
        public void DropAccepted(UIElement sender)
        {
            Point pos1 = sender.PointToScreen(new Point(0, 0));
            Point pos = this.border.PointFromScreen(pos1);

            this.adorner.SetLockedPosition(pos.X, pos.Y);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a DropRejected.
        /// </summary>
        public void DropRejected()
        {
            //this.border.Background = new SolidColorBrush(Colors.Red);
        }

        public void DropLost()
        {
            this.adorner.UnlockPosition();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a DropDone.
        /// </summary>
        public void DropDone()
        {
            this.adorner.UnlockPosition();
        }

        private void border_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }

    }
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a TestAdorner.
    /// </summary>
    public class TestAdorner : Adorner
    {
        private Rect adornedElementRect;
        private double offsetx, offsety;

        private double x, y;
        private bool locked;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a TestAdorner.
        /// </summary>

        public TestAdorner(UIElement adornedElement)
        : base(adornedElement) 
      {
          //adornedElementRect = new Rect(adornedElement.DesiredSize);
          adornedElementRect = new Rect(0,0,142,29);
          Point curpos = MouseSimulator.GetCursorPosition();
          Point point = adornedElement.PointFromScreen(curpos);
          offsetx = point.X;
          offsety = point.Y;
      }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>

        public void SetPosition(double x, double y)
        {
            if (!locked)
            {
                this.x = x - offsetx;
                this.y = y - offsety;
                this.InvalidateVisual();
            }
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public void SetLockedPosition(double x, double y)
        {
            this.SetPosition(x + offsetx, y + offsety);
            this.locked = true;
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public void UnlockPosition()
        {
            this.locked = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            adornedElementRect.X = this.x;
            adornedElementRect.Y = this.y;
            VisualBrush vb = new VisualBrush(this.AdornedElement);
            drawingContext.DrawRectangle(vb, null, adornedElementRect);
        }
    }
}
