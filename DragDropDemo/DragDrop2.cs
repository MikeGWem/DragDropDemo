using DragDropDemo.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DragDropDemo
{
    public partial class DragDrop2 : Form
    {
        // Sample address data courtesy of https://www.briandunning.com/sample-data/
        #region Private Declarations
        private DragAddress newAddress = null;
        private int addressIds, dragWentPast, dragItemSequence, panelTop;
        private Cursor copyCursor, noneCursor, moveCursor;
        private DataFormats.Format dragAddress = DataFormats.GetFormat("DragAddress");
        private DataFormats.Format addressControl = DataFormats.GetFormat("AddressControl");
        private List<DragAddress> preLoaded = new List<DragAddress>();
        private const int scrollAt = 25;
        #endregion
        public DragDrop2()
        {
            InitializeComponent();
        }
        #region Methods called as delegates
        public void DoControlDrag(DragAddress theAddress, int controlSequence)
        {
            dragItemSequence = controlSequence;
            DataObject myDataObject = new DataObject(addressControl.Name, theAddress);
            DoDragDrop(myDataObject, DragDropEffects.Copy | DragDropEffects.Scroll | DragDropEffects.Move);
        }
        public void DraggedPast(int pastAddress, int yPos)
        {
            dragWentPast = pastAddress;
            maybeScrollpanel(yPos);
        }
        #endregion
        private void DragDrop2_Load(object sender, EventArgs e)
        {
            newAddress = new DragAddress(++addressIds);
            moveCursor = CursorUtil.CreateCursor((Bitmap)imageList1.Images[0], 0, 0);
            copyCursor = CursorUtil.CreateCursor((Bitmap)imageList1.Images[1], 29, 3);
            noneCursor = CursorUtil.CreateCursor((Bitmap)imageList1.Images[2], 0, 0);
            this.GiveFeedback += giveFeedback;
            loadAddresses();
            btnUpdate.Location = btnFill.Location;
            calculatePanelYLocation();
        }
        #region Private Methods
        private void giveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = copyCursor;
            }
            else if (e.Effect == DragDropEffects.None)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = noneCursor;
            }else if (e.Effect == DragDropEffects.Move)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = moveCursor;
            }
        }
        /// <summary>
        /// loadAddresses() reads an embedded text file (CSV) usng the VisualBasic TextFieldParser class
        /// and loads the address data into a List<> of DragAddress
        /// </summary>
        private void loadAddresses()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader sReader = new StreamReader(assembly.GetManifestResourceStream("DragDropDemo.address50.csv"));
            TextFieldParser tfParser = new TextFieldParser(sReader);
            tfParser.TextFieldType = FieldType.Delimited;
            tfParser.SetDelimiters(",");
            while (!tfParser.EndOfData)
            {
                string[] curRow = tfParser.ReadFields();
                preLoaded.Add(new DragAddress(curRow[0], curRow[1], curRow[2], curRow[3]));
            }
            tfParser.Close();
            tfParser.Dispose();
            sReader.Close();
        }
        private void maybeScrollpanel(int dragY)
        {
            if ((dragY - panelTop) <= flowLayoutPanel1.VerticalScroll.Value && flowLayoutPanel1.VerticalScroll.Value > 0)
            {
                flowLayoutPanel1.VerticalScroll.Value -= (flowLayoutPanel1.VerticalScroll.Value > scrollAt) ? scrollAt : flowLayoutPanel1.VerticalScroll.Value;
            }
            else if (dragY - panelTop >= flowLayoutPanel1.Height - scrollAt && flowLayoutPanel1.VerticalScroll.Maximum > 0)
            {
                flowLayoutPanel1.VerticalScroll.Value += (flowLayoutPanel1.VerticalScroll.Maximum - flowLayoutPanel1.VerticalScroll.Value > scrollAt) ? scrollAt : flowLayoutPanel1.VerticalScroll.Maximum - flowLayoutPanel1.VerticalScroll.Value;
            }
        }
        private void reorderAddresses()
        {
            foreach (AddressControl ac in flowLayoutPanel1.Controls.OfType<AddressControl>().OrderBy(a => a.Sequence))
            {
                flowLayoutPanel1.Controls.SetChildIndex(ac, ac.Sequence); // probly could have just used the ChildIndex all along??
            }
        }
        private void resetPnlEdit()
        {
            newAddress = new DragAddress(++addressIds);
            tbName.Text = "";
            tbAddress1.Text = "";
            tbAddress2.Text = "";
            tbPostCode.Text = "";
        }
        /// <summary>
        /// called on Form Load and LocationChanged to calculate the Y position of the
        /// address storage panel in screen co-ordinates
        /// </summary>
        private void calculatePanelYLocation()
        {
            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
            panelTop = this.Location.Y + (screenRectangle.Top - this.Top);

        }
        #region Form Events
        private void DragDrop2_FormClosing(object sender, FormClosingEventArgs e)
        {
            moveCursor.Dispose();
            copyCursor.Dispose();
            noneCursor.Dispose();
        }
        private void DragDrop2_LocationChanged(object sender, EventArgs e)
        {
            calculatePanelYLocation();
        }
        #endregion
        #region Control Events
        private void tbName_KeyUp(object sender, KeyEventArgs e)
        {
            newAddress.Name = tbName.Text;
        }
        private void tbAddress1_KeyUp(object sender, KeyEventArgs e)
        {
            newAddress.AddressLine1 = tbAddress1.Text;
        }
        private void tbAddress2_KeyUp(object sender, KeyEventArgs e)
        {
            newAddress.AddressLine2 = tbAddress2.Text;
        }
        private void tbPostCode_KeyUp(object sender, KeyEventArgs e)
        {
            newAddress.PostCode = tbPostCode.Text;
        }

        private void pnlEdit_MouseDown(object sender, MouseEventArgs e)
        {
            DataObject myDataObject = new DataObject(dragAddress.Name, newAddress);
            DoDragDrop(myDataObject, DragDropEffects.Copy | DragDropEffects.Scroll);
        }
        private void pnlEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(addressControl.Name))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Reads the values from a random DraggAddress and then removes that class from the List<>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFill_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int address = rnd.Next(0, preLoaded.Count - 1);
            DragAddress copyAddress = preLoaded[address];
            newAddress.Name = copyAddress.Name;
            newAddress.AddressLine1 = copyAddress.AddressLine1;
            newAddress.AddressLine2 = copyAddress.AddressLine2;
            newAddress.PostCode = copyAddress.PostCode;
            preLoaded.RemoveAt(address);
            tbName.Text = newAddress.Name;
            tbAddress1.Text = newAddress.AddressLine1;
            tbAddress2.Text = newAddress.AddressLine2;
            tbPostCode.Text = newAddress.PostCode;
            btnFill.Enabled = preLoaded.Count > 0;
        }

        private void pnlEdit_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(addressControl.Name))
            {
                newAddress = (DragAddress)e.Data.GetData(addressControl.Name);
                btnFill.Visible = false;
                btnUpdate.Visible = true;
                tbName.Text = newAddress.Name;
                tbAddress1.Text = newAddress.AddressLine1;
                tbAddress2.Text = newAddress.AddressLine2;
                tbPostCode.Text = newAddress.PostCode;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (AddressControl ac in flowLayoutPanel1.Controls.OfType<AddressControl>())
            {
                if (ac.AddressID == newAddress.AddressID)
                {
                    ac.SetFields();
                    break;
                }
            }
            resetPnlEdit();
            btnUpdate.Visible = false;
            btnFill.Visible = true;
        }

        private void flowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(dragAddress.Name))
            {
                e.Effect = DragDropEffects.Copy;
            } else if (e.Data.GetDataPresent(addressControl.Name))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void flowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(dragAddress.Name))
            {
                //handles drop of new address onto the panel
                AddressControl newControl = new AddressControl((DragAddress)e.Data.GetData(dragAddress.Name), DoControlDrag, DraggedPast, flowLayoutPanel1.Controls.Count);
                flowLayoutPanel1.Controls.Add(newControl);
                resetPnlEdit();
            } else if (e.Data.GetDataPresent(addressControl.Name))
            {
                // handles drop of an existing AddressControl into (presumably) a new position
                if (dragWentPast < dragItemSequence)
                {
                    foreach(AddressControl ac in flowLayoutPanel1.Controls.OfType<AddressControl>().OrderBy(a => a.Sequence))
                    {
                        if(ac.Sequence == dragItemSequence)
                        {
                            ac.Sequence = dragWentPast;
                        } else if(ac.Sequence >= dragWentPast)
                        {
                            ac.Sequence++;
                        }
                    }
                } else if (dragWentPast > dragItemSequence)
                {
                    int newSeq = 1;
                    foreach (AddressControl ac in flowLayoutPanel1.Controls.OfType<AddressControl>().OrderBy(a => a.Sequence))
                    {
                        if(ac.Sequence == dragItemSequence)
                        {
                            ac.Sequence = dragWentPast + 1;
                        } else if (ac.Sequence >= dragWentPast + 1)
                        {
                            ac.Sequence++;
                        } else
                        {
                            ac.Sequence = newSeq;
                            newSeq++;
                        }
                    }
                }
                reorderAddresses();
            }
        }
        private void flowLayoutPanel1_DragOver(object sender, DragEventArgs e)
        {
            maybeScrollpanel(e.Y);
        }

        #endregion
        #endregion
    }
    /// <summary>
    /// Simple class to hold demo address data
    /// </summary>
    public class DragAddress
    {
        private int addressID;
        public int AddressID
        {
            get { return addressID; }
        }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }
        public DragAddress(int newID)
        {
            addressID = newID;
        }
        public DragAddress(string name, string addL1, string addL2, string pCode)
        {
            Name = name;
            AddressLine1 = addL1;
            AddressLine2 = addL2;
            PostCode = pCode;
        }
    }
    // delegates used to communicate between the UserControls (AddressControl) and the Form
    /// <summary>
    /// DragPast delegate is used to communicate a drag over an AddressControl
    /// addressID is used as a (crude) mechanism to indicate the location of a drop when a control is moved
    /// yPos is passed to allow the from to scroll the FlowLayoutPanel if the drag is nearing the top or bottom
    /// </summary>
    /// <param name="addressID"></param>
    /// <param name="yPos"></param>
    public delegate void DragPast(int addressID, int yPos);
    /// <summary>
    /// DragControl delegate signals which AddressControl wants to initiate a Drag with
    /// the DoDragDrop() being offloaded to the Form
    /// </summary>
    /// <param name="thisAddress"></param>
    /// <param name="sequence"></param>
    public delegate void DragControl(DragAddress thisAddress, int sequence);
}
