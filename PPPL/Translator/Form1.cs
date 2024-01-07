using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using static System.Windows.Forms.AxHost;

namespace xtUML1
{
    public partial class Form1 : Form
    {
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private string selectedFilePath;
        private bool isJsonFileSelected = false;


        public Form1()
        {
            InitializeComponent();

            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
        }

        private void btnSelect_Click_1(object sender, EventArgs e)
        {
            // Menampilkan dialog untuk memilih file JSON
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.Title = "Select JSON File";
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {

                // ubah semua sesuai kebutuhan

                string jsonContent = File.ReadAllText(openFileDialog.FileName);

                dynamic jsonObj = JObject.Parse(jsonContent);

                selectedFilePath = openFileDialog.FileName;

                textBox1.Text = selectedFilePath;

                // tampilkan sintax error parsing di textBox4 jika tidak lolos parsing

                textBox4.Text = jsonContent; // untuk menampilkan isi file json (setelah lolos parsing)

                isJsonFileSelected = true;
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            // parsing
            // tampilkan sintax error parsing di textBox4 jika tidak lolos parsing
            // tampilkan isi file json di textBox4 jika lolos parsing
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            // translate jika lolos parsing
            // tampilkan hasil translate di textBox3
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // tulis method untuk menghapus nilai textBox1 (selected file)
            textBox3.Clear();
            textBox4.Clear();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpMessage = OpenHelp(); // tulis isi help di sini
            MessageBox.Show(helpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string OpenHelp()
        {
            StringBuilder helpMessage = new StringBuilder();

            helpMessage.AppendLine("User Guide for Generating JSON Model into Java Programming Language");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Open the desired JSON File by clicking Browse button");
            helpMessage.AppendLine("2. After the code appeared in the box, click the Generate button to translate it to Java");
            helpMessage.AppendLine("3. The Java Code will exist in the right box and click Export button to save it as Java file");

            return helpMessage.ToString();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (textBox3.TextLength > 0)
            {
                textBox3.SelectAll();
                textBox3.Copy();
                MessageBox.Show("Successfully Copied!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                MessageBox.Show("Please Translate First!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isJsonFileSelected)
            {
                MessageBox.Show("Select JSON file as an input first!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBox3.TextLength > 0)
            {
                saveFileDialog.Filter = "C++ files (*.cpp)|*.cpp|All files (*.*)|*.*"; // ubah ekstensi output save file
                saveFileDialog.Title = "Save C++ File"; // ubah C++

                DialogResult result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string cppCode = textBox3.Text;

                    File.WriteAllText(saveFileDialog.FileName, cppCode);

                    selectedFilePath = saveFileDialog.FileName;

                    MessageBox.Show("Successfully Saved!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please Translate First!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
    }
}
