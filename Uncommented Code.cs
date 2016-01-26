using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            progressBar1.Maximum = 1;
            label5.Text = ("Time Taken: ");
            
            string sigFile = openFileDialog1.FileName;  
            if (sigFile != "openFileDialog1")           
            {
                string folderCheck = folderBrowserDialog1.SelectedPath;
                if (folderCheck != "")                   
                {
                    try
                    { 
                        string[] directory; 
                        int virusCount = 0; 
                        int fileCount = 0;  
                        int fileVal = 0;    
                        int sigVal = 0;     

                        DateTime startTime = new DateTime();
                        TimeSpan elapsedTime = new TimeSpan();

                        timer1.Interval = (1000) * (1);
                        timer1.Enabled = true;
                        timer1.Start();
                        startTime = DateTime.Now;

                        if (checkBox1.Checked == true)
                        {             
                            directory = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.*", SearchOption.AllDirectories);
                            progressBar1.Maximum = directory.Length;
                        }
                        else
                        {
                            directory = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                            progressBar1.Maximum = directory.Length;
                        }

                        foreach (string file in directory)
                        {
                            try
                            {
                                if (file == openFileDialog1.FileName)
                                {
                                    progressBar1.Increment(1);
                                }
                                else
                                {
                                    FileStream streamIn = new FileStream(file, FileMode.Open);      
                                    FileStream sigStream = new FileStream(sigFile, FileMode.Open);  
                                    sigVal = sigStream.ReadByte();                                 

                                    for (int i = 0; (fileVal = streamIn.ReadByte()) != -1; i++)
                                    {
                                        if (sigVal == fileVal)
                                        {
                                            int z = 1;
                                            for (int y = 1; y <= sigStream.Length - 1; y++)
                                            {
                                                fileVal = streamIn.ReadByte(); 
                                                sigVal = sigStream.ReadByte(); 
                                                if (sigVal != fileVal)
                                                {

                                                }
                                                else if (sigVal == fileVal)
                                                {
                                                    z++;
                                                }
                                            }
                                            if (z == sigStream.Length)
                                            {
                                                virusCount++;
                                                listBox1.Items.Add(file);
                                            }
                                        }
                                    }
                                    sigStream.Flush();
                                    sigStream.Close();
                                    fileCount++;
                                    progressBar1.Increment(1);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error occured scanning: '" + file + "'. Press 'OK' to skip this file...");
                                progressBar1.Increment(1);
                            }
                        }
                        timer1.Stop();
                        timer1.Enabled = false;
                        elapsedTime = DateTime.Now - startTime;
                        string time = elapsedTime.TotalSeconds.ToString();
                        label5.Text = ("Time Taken: " + time + " seconds!");

                        if (virusCount >= 1)
                        {
                            MessageBox.Show(fileCount + " file(s) scanned with " + virusCount + " virus(s) found");
                        }
                        else
                        {
                            MessageBox.Show(fileCount + " file(s) scanned with No Virus Found");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Error accesssing the specified directory or sub-directory. Please change the directory or ensure you have the proper permissions.");
                    }
                }      
                else
                {
                    MessageBox.Show("Please specify the folder to scan and try again.");
                }
            }
            else
            {
                MessageBox.Show("Please specify the signature file and try again.");
            }
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                label4.Text = ("Folder to Scan: "+folderBrowserDialog1.SelectedPath);
            }        
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label3.Text = ("Signature File: "+openFileDialog1.FileName);
            }
        }
    }
}


    
    
    

