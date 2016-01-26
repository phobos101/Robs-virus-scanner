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
            //The first few line ensure that the program can be reused without needing to restart the application
            listBox1.Items.Clear();
            progressBar1.Maximum = 1;
            label5.Text = ("Time Taken: ");
            
            string sigFile = openFileDialog1.FileName;  // The file specified by the user that contains the virus signature.
            if (sigFile != "openFileDialog1")           // Ensures user specifies a signature file.
            {
                string folderCheck = folderBrowserDialog1.SelectedPath;
                if (folderCheck != "")                   //Ensures a user selects a folder.
                {
                    try
                    {
                        // Many variables for this program are declared when the user begins the scanning process. 
                        string[] directory; // The folder selected by the user to scan the contents for viruses.
                        int virusCount = 0; // A counter of how many viruses have been found.
                        int fileCount = 0;  // A counter of how many files have been scanned.
                        int fileVal = 0;    // The value of an individual byte within the currently scanning file.
                        int sigVal = 0;     // The value of an individual byte within the virus signature file.

                        DateTime startTime = new DateTime();
                        TimeSpan elapsedTime = new TimeSpan();

                        timer1.Interval = (1000) * (1);
                        timer1.Enabled = true;
                        timer1.Start();
                        startTime = DateTime.Now;

                        if (checkBox1.Checked == true)
                        {
                            // If the checkbox is ticked then the user wants to scan all sub-directories in addition to the selected directory/folder.                
                            directory = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.*", SearchOption.AllDirectories);
                            progressBar1.Maximum = directory.Length;
                        }
                        else
                        {
                            // If the checkbox is left blank then only the contents of the selected director will be scanned.
                            directory = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                            progressBar1.Maximum = directory.Length;
                        }

                        foreach (string file in directory)
                        {
                            // This will loop for every file in the specified directory and sub-directories if the user ticked the box.
                            try
                            {
                                if (file == openFileDialog1.FileName)
                                {
                                    //This prevents the sigfile itself from being scanned which would cause a false positive result and could cause a program crash.
                                    progressBar1.Increment(1);
                                }
                                else
                                {
                                    FileStream streamIn = new FileStream(file, FileMode.Open);      // This will read in the bytes of the file to scan.
                                    FileStream sigStream = new FileStream(sigFile, FileMode.Open);  // This will read in the bytes of the virus signature file.
                                    sigVal = sigStream.ReadByte();                                  // The value of the first byte read by the file stream.

                                    for (int i = 0; (fileVal = streamIn.ReadByte()) != -1; i++)
                                    {
                                        // This will loop for as long as there is data to read in the current file being scanned. When the end of the file has been reached, this loop will end.

                                        if (sigVal == fileVal)
                                        {
                                            // If the first byte of the signature matches the current byte of the file being scanned then this is invoked.

                                            int z = 1;  // A counter to verify a signature match
                                            for (int y = 1; y <= sigStream.Length - 1; y++)
                                            {
                                                /* This will loop as long as the value of y is less than or equal to the length of the virus signature. Y is set to one not zero because this is only 
                                                 * invoked if the zero byte has already matched.
                                                 */
                                                fileVal = streamIn.ReadByte(); // Read the next byte of the currently scanning file.
                                                sigVal = sigStream.ReadByte(); // Read the next byte of the virus signature
                                                if (sigVal != fileVal)
                                                {
                                                    // If at any point the chain is broken and the signature does not match, then the loop ends.
                                                }
                                                else if (sigVal == fileVal)
                                                {
                                                    // When the subsequent bytes of both the virus signature and the currently scanning file also match, the variable z is increased by one.
                                                    z++;
                                                }
                                            }
                                            if (z == sigStream.Length)
                                            {
                                                /* Once the byte comparisons have completed (due to end of virus signature), if there is a match between the virus signature and the section of the 
                                                 * file that is currently scanning, then the value of Z will be that of the length of the signature file's filestream. This being the case, we know
                                                 * that a virus is on this file and the virus count is increased. Additionally, the file is added to the list box for the user to see.
                                                 */
                                                virusCount++;
                                                listBox1.Items.Add(file);
                                            }
                                        }
                                    }

                                    /* Once the file reaches the end and the loop ends, the signature files filestream must be reset in order to accurately compare it to the next file. The file count
                                     * is incremented to keep track of how many files have been scanned, and the progress bar is updated to reflect another file has been scanned.
                                     */
                                    sigStream.Flush();
                                    sigStream.Close();
                                    fileCount++;
                                    progressBar1.Increment(1);
                                }
                            }
                            catch
                            {
                                // If there is a problem scanning a file, this message will appear, allowing the user to continue the scan successfully.
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
                            // If the virus count is set to one or higher, then the user is shown a message stating how many files were scanned and how many viruses were found.
                            MessageBox.Show(fileCount + " file(s) scanned with " + virusCount + " virus(s) found");
                        }
                        else
                        {
                            /* If no viruses were found then the virusCount variable will still be set to zero. The user is shown a message stating how many files were scanned and that no viruses 
                             * were detected.
                             */
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
                // When the user selects the folder to scan, label4 will show the folder selected. This allows the user to notice if they have selected the wrong folder.
                label4.Text = ("Folder to Scan: "+folderBrowserDialog1.SelectedPath);
            }        
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Once the user has specified a virus signature file, label3 will display the file path.
                label3.Text = ("Signature File: "+openFileDialog1.FileName);
            }
        }
    }
}


    
    
    

