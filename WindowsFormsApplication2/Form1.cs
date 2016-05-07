using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class DragAndDrop : Form
    {
        public DragAndDrop()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                bool errors = false;
                String tempFilename = file, outfile;
                string password = @"verd5ui8cs54sntf";

                /* ---------- Custom Path ---------- */

                if (checkBox5.Checked)
                {
                    this.folderBrowserDialog1.ShowNewFolderButton = true;
                    this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.Desktop;

                    DialogResult result = this.folderBrowserDialog1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        MessageBox.Show("Repertory selected");
                    }

                    else
                    {
                        MessageBox.Show("No custom directory selected !", "Error");
                        errors = true;
                    }
                }

                /* ---------- Ask Confirmation ---------- */

                if (checkBox2.Checked)
                {
                    if (MessageBox.Show("This action is definitive. Do you want to continue ?", "User Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        errors = true;
                    }
                }

                if (!errors)
                {

                    Crypto fileBuffer = new Crypto();

                    if (fileBuffer.IsEncrypted(tempFilename))
                    {
                        if (fileBuffer.IsEncryptedFileName(tempFilename))
                        {
                            outfile = Path.GetDirectoryName(tempFilename) + @"\" + fileBuffer.DecryptTitle(Path.GetFileNameWithoutExtension(tempFilename), password);
                        }

                        else
                        {
                            outfile = Path.GetDirectoryName(tempFilename) + @"\" + Path.GetFileNameWithoutExtension(tempFilename);
                        }

                        fileBuffer.DecryptFile(tempFilename, outfile, password);
                    }

                    else
                    {
                        if (checkBox4.Checked)
                        {
                            outfile = Path.GetDirectoryName(tempFilename) + @"\" + fileBuffer.EncryptTitle(Path.GetFileName(tempFilename), password) + ".cryptn";
                        }
                        
                        else
                        {
                            outfile = tempFilename + ".crypt";
                        }

                        fileBuffer.EncryptFile(tempFilename, outfile, password);
                    }

                    /* ---------- Keep The Original ---------- */

                    if (!checkBox1.Checked && !errors)
                    {
                        File.Delete(tempFilename);
                    }
                }

            }
        }


        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
            //    textBox1.Enabled = true;
            }

            else
            {
              //  textBox1.Enabled = false;
            }
        }

    }
}
