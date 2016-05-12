using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DragDropDemo.Classes
{
    public partial class AddressControl : UserControl
    {
        private DragAddress dragAddress;
        private DragControl dragDelegate;
        private DragPast dragPast;
        private int sequence;
        public AddressControl(DragAddress dAddress, DragControl dragDelegate, DragPast dragPast, int seq)
        {
            InitializeComponent();
            dragAddress = dAddress;
            this.dragDelegate = dragDelegate;
            this.dragPast = dragPast;
            sequence = seq;
            SetFields();
            this.Paint += drawGroupBox;
        }
        public int AddressID
        {
            get { return dragAddress.AddressID; }
        }
        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }
        public void SetFields()
        {
            lblName.Text = dragAddress.Name;
            lblAddress1.Text = dragAddress.AddressLine1;
            lblAddress2.Text = dragAddress.AddressLine2;
            lblPostCode.Text = dragAddress.PostCode;
        }
        private void drawGroupBox(object sender, PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            rect.Width -= 4;
            rect.Height -= 4 + (lblName.Height / 2);
            rect.Location = new Point(2, (lblName.Height / 2));
            using (var pen = new Pen(SystemColors.ActiveCaption))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void AddressControl_MouseDown(object sender, MouseEventArgs e)
        {
            dragDelegate?.Invoke(dragAddress, sequence); // delegates the drag initiation to the form
        }

        private void AddressControl_DragEnter(object sender, DragEventArgs e)
        {
            dragPast?.Invoke(sequence, e.Y); // signals that something has been dragged over this control to form
        }

        private void AddressControl_DragOver(object sender, DragEventArgs e)
        {
            dragPast?.Invoke(sequence, e.Y); // signals update to drag cursor hotspot Y position
        }
    }
}
