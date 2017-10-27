using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Collections.Concurrent;

namespace Sharp_Editor
{
    public partial class Form1 : Form
    {
        string path = "noPath";
        string encytpedpath = "";
        Task[] tasks = new Task[1];
   
        int xx = 0;
        int currenttabno=0;

        public Form1()
        {
            InitializeComponent();
            newTabCreate();                      //Create a new tab when the application starts or the form is created
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";    //when the save dialog box opens it automatically has the txt file as default type to save 
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            TabPage tp = tabControl1.SelectedTab;                  //takes the tab currently selected by the user
            if (!(tp.Text).Contains("Encrypted"))                 //checks whether tabs name has the keyword encrypted
            {
                if ((tp.Text).Equals("untitled *"))               //checks whether tabs name has the word untitled
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog1.FileName, myrtb().Text);    //save everything in the rich text box to the location given by the user
                        path = saveFileDialog1.FileName;
                        tp.Tag = path;                                              //save the path of the tab in the tag field of that tab so that it can be called from anywhere

                        String[] substrings = path.Split('\\');                    //split it using \
                        int noofStrings = substrings.Length;
                        tp.Text = substrings[noofStrings - 1];                     //last word in the array wil be the name of the file so set it as the tab's name
                        File.WriteAllLines(path, myrtb().Lines);

                    }
                }
                else
                {
                    try
                    {
                        path = (string)tp.Tag;                                   //save the path of the tab in the tag field of that tab so that it can be called from anywhere
                        File.WriteAllText(path, myrtb().Text);
                        File.WriteAllLines(path, myrtb().Lines);

                    }
                    catch (ArgumentException ee)
                    {
                        Console.WriteLine(ee);
                    }
                }
            }
            else
            {
                MessageBox.Show("Saving an encrypted file as normal file will result in loss of Encryption !"); //display message
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myrtb().Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool vall = myrtb().CanRedo;
            myrtb().Redo();
        }

        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string styleoffont = myrtb().SelectionFont.Style.ToString();           //take the current font style

            if (styleoffont.Equals("Bold"))                                       //check whether its already bold
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
            }
            else
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Bold);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myrtb().Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myrtb().Paste();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myrtb().Cut();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myrtb().SelectAll();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myrtb().Text = "";
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string styleoffont = myrtb().SelectionFont.Style.ToString();              //take the current font style

            if (styleoffont.Equals("Italic"))                                        //check whether its already italics
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
            }
            else
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Italic);
            }
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string styleoffont = myrtb().SelectionFont.Style.ToString();             //take the current font style

            if (styleoffont.Equals("Underline"))                                    //check whether its already underline
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
            }
            else
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Underline);
            }
        }

        private void regularToolStripMenuItem_Click(object sender, EventArgs e)
        {

            myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
        }

        private void fontDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            myrtb().Font = fontDialog1.Font;
        }

        private void colourDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            myrtb().ForeColor = colorDialog1.Color;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"C:\";

            openFileDialog1.Title = "Open text Files";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";              //set the file type to open to text
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            TabPage tp = tabControl1.SelectedTab;                           //returns the current tab
            try {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    if (openFileDialog1.FileName.Contains("Encrypted"))               //checks whether the opening file is an encrypted file
                    {
                        MessageBox.Show("You are trying to open an encrypted file");
                        string val = sr.ReadToEnd();                                   //reads the whole file
                        byte[] encodedDataAsBytes = Convert.FromBase64String(val);     //decrpyting 
                        string decryptedValue = ASCIIEncoding.ASCII.GetString(encodedDataAsBytes); //get the normal string format from ASCII
                        myrtb().Text = decryptedValue;                                             //insert the normal string value to the rich text box
                        path = openFileDialog1.FileName;
                        tp.Tag = path;
                        String[] substrings = path.Split('\\');                                 
                        int noofStrings = substrings.Length;
                        tp.Text = substrings[noofStrings - 1];                                     //set the tabs name to the last string in the array
                        sr.Close();
                    }
                    else
                    {
                        myrtb().Text = sr.ReadToEnd();                         //reads the whole file
                        path = openFileDialog1.FileName;
                        tp.Tag = path;                                         //save the path of the tab in the tag field of that tab so that it can be called from anywhere

                        String[] substrings = path.Split('\\');
                        int noofStrings = substrings.Length;
                        tp.Text = substrings[noofStrings - 1];               //set the tabs name to the last string in the array
                        File.ReadAllLines(path);

                        sr.Close();
                    }
                }
            }
            catch(FormatException ee)
            {
                Console.WriteLine(ee);
            }
        }



        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                newTabCreate();
            }
            catch (ArgumentOutOfRangeException ee)
            {
                Console.WriteLine(ee);
            }
            catch (ArgumentException ee)
            {
                Console.WriteLine(ee);
            }

        }

        private void MainmenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public void newTabCreate()
        {
            TabPage tp = new TabPage();                         //create a tab page

            RichTextBox rtb = new RichTextBox();               //create a rich text box
            tp.AutoScroll = true;
            tabControl1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;      //anchoring the tab control so it expands and shrinks properly    
            rtb.WordWrap = false;
            Label mainlabel = new Label();
            Label lb1 = new Label();
            Label lb2 = new Label();
            Label lb3 = new Label();
            Label lb4 = new Label();
            rtb.Dock = DockStyle.Fill;
            mainlabel.Dock = DockStyle.Bottom;
            lb1.Dock = DockStyle.Left;
            lb2.Dock = DockStyle.Left;
            lb3.Dock = DockStyle.Left;
            lb4.Dock = DockStyle.Left;
            mainlabel.Text = "Total Counts ";
            lb1.Text = "Word Count is : 0";
            lb2.Text = "  Line Count is : 0";
            lb3.Text = "Letter Count is : 0";
            lb4.Text = "Total Letters : 0";
                              
            mainlabel.Controls.Add(lb3);                       //adding the labels to the main label
            mainlabel.Controls.Add(lb2);
            mainlabel.Controls.Add(lb1);
            mainlabel.Controls.Add(lb4);
            //        tp.Controls.Add(lb1);
            //        tp.Controls.Add(lb2);
            //        tp.Controls.Add(lb3);
            tp.Controls.Add(mainlabel);                          //add the main label to the tab page
            tp.Controls.Add(rtb);                                //add the rich text box to the tab page
            tp.Text = "untitled *";
            tabControl1.TabPages.Add(tp);
            tabControl1.SelectedTab =tp;
            rtb.Select();
            rtb.TextChanged += rtb_TextChanged;
        }

        private void rtb_TextChanged(object sender, EventArgs e)
        {

            TabPage tp = tabControl1.SelectedTab;                         //selects the current tab
            Label mainlabel = tp.Controls[0] as Label;                    //main label of the selected tab

            int numberoflines = myrtb().Lines.Count();                     //number of lines in the rich text box

            string lastWord = myrtb().Text.Split(' ').Last();             //takes the last word from the rich text box
            lastWord = lastWord.Replace("\n", string.Empty);              //if there is a line break, replace it with empty string

            int countWords = lastWord.Count(x => { return !Char.IsWhiteSpace(x); }); //number of letters in the word
            mainlabel.Controls[0].Text = "Letter Count is : " + countWords;
            mainlabel.Controls[1].Text = "  Line Count is : " + numberoflines;
            mainlabel.Controls[2].Text 
                = "Word Count is : " + Regex.Matches(((RichTextBox)sender).Text, @"[\S]+").Count.ToString(); // 'S' counts the non white characters
           

            if (xx != countWords)
            {
                  xx = countWords;
                  tasks[0] = Task.Run(() => spellCheck());         //runs the spell check method as another task

                  string text = myrtb().Text;
                  var t1autoSave = new Task(() => autosave(tp, text));  //run the task as seperate task from the main thread
                  t1autoSave.Start();                                    //start the task

                   Task.WaitAll(t1autoSave);                             //wait till task completes
        /*        string text = myrtb().Text;
                Parallel.Invoke(
                   () => spellCheck(),
                    () => autosave(tp, text)
                ); */

            }

        }




        public void closeTab()
        {
            try {
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);         //remove the tab that is currently selected
            }
            catch(ArgumentNullException ee)
            {
                Console.WriteLine(ee);
            }
        }

        public RichTextBox myrtb()
        {
            try
            {
                TabPage tp = tabControl1.SelectedTab;                    //returns the tab currently selected
                RichTextBox rtb = tp.Controls[1] as RichTextBox;         //returns the rich text box of that selected tab .. here '1' represents rich text box because it is added as the second control to the tab
                return rtb;
            }
            catch (InvalidOperationException ee)
            {
                Console.WriteLine(ee);
            }
            catch (IndexOutOfRangeException ee)
            {
                Console.WriteLine(ee);
            }
            catch (NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
            return null;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

       

        private void encryptFile()
        {

            TabPage tp = tabControl1.SelectedTab;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            try
            {

                byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(myrtb().Text); //encrypts what is in the rich text box
                string encryptedText = Convert.ToBase64String(toEncodeAsBytes);        //from binary to ASCII
                if (!(tp.Text).Contains("Encrypted"))                                 //checks whether tab's name has the keyword encrypted
                {
                    if ((tp.Text).Contains("untitled *"))                             //checks whether tab's name has the word untitled
                    {
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
                        {
                            string newFilename = saveFileDialog1.FileName;
                            String[] substrings = newFilename.Split('.');
                            newFilename = substrings[0];
                            newFilename += "Encrypted.";                             //adds the keyword 'Encrypted' to the filename
                            newFilename += substrings[1];
                            File.WriteAllText(newFilename, encryptedText);           //writes everything to the file
                            path = newFilename;
                            encytpedpath = path;
                            String[] substrings2 = path.Split('\\');
                            int noofStrings = substrings2.Length;
                            tp.Text = substrings2[noofStrings - 1];                  //sets the tab's name to the file name
                            tp.Tag = encytpedpath;


                        }
                    }
                    else
                    {
                        MessageBox.Show("Dont Encrypt an existing normal file ! Create a new one");
                    }

                }
                else
                {
                    if (tp.Tag != null)
                    {
                        encytpedpath = (string)tp.Tag;
                    }
                    File.WriteAllText(encytpedpath, encryptedText);
                }

            }

            catch (NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }


        }

        public delegate void encryptFileDelegate();
        private void encryptMethod(TabPage tp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new encryptFileDelegate(encryptFile));   
            }
            else
            {
                encryptFile();
            }
        }

        private void encryptFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tp = tabControl1.SelectedTab;
            var taskEnc = new Task(() => encryptMethod(tp));         //run as a separate task
            taskEnc.Start();
          
        }

        private void autosave(TabPage tp, string text)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (!tp.Text.Contains("Encrypted"))
            {
                if (!tp.Text.Equals("untitled *"))
                {
                    try
                    {

                        path = (string)tp.Tag;
                        File.WriteAllText(path, text);


                    }
                    catch (ArgumentException ee)
                    {
                        Console.WriteLine(ee);
                    }
                    catch (NullReferenceException ee)
                    {
                        Console.WriteLine(ee);
                    }
                    catch (IOException ee)
                    {
                        Console.WriteLine(ee);
                    }

                }
            }
            else
            {
                byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(text);           //encrypt the text before saving
                string encryptedText = Convert.ToBase64String(toEncodeAsBytes);
                path = (string)tp.Tag;
                File.WriteAllText(path, encryptedText);                                //writes the encrypted text to the file
            }

        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

           
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            try
            {
                TabControl.TabPageCollection tabcollection = tabControl1.TabPages;
                  foreach (TabPage tabpage in tabcollection)
                  {
                    if (!(tabpage.Text).Contains("Encrypted"))
                    {
                        if (!(tabpage.Text).Equals("untitled *"))
                        {
                            RichTextBox rtb = tabpage.Controls[1] as RichTextBox;           //returns the rich tex box of that selected tab
                            string text = rtb.Text;
                            var t2 = new Task(() => saveAll(tabpage, text));                //run as a separate task
                            t2.Start();                                                     //start the task
                            Task.WaitAll(t2);                                               //wait till it completes
                        }
                        else
                        {
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {

                                path = saveFileDialog1.FileName;
                                tabpage.Tag = path;                                         //set the path as the tab's tag field

                                String[] substrings = path.Split('\\');
                                int noofStrings = substrings.Length;
                                tabpage.Text = substrings[noofStrings - 1];                 //set the name of the tab as the file name
                                RichTextBox rtbnew = tabpage.Controls[1] as RichTextBox;
                                string text = rtbnew.Text;
                                File.WriteAllText(saveFileDialog1.FileName, myrtb().Text);  //write what is in the rich text box to the file
                                File.WriteAllLines(path, myrtb().Lines);

                            }
                        }
                    }
                    else
                    {
                        RichTextBox rtb = tabpage.Controls[1] as RichTextBox;
                        string text = rtb.Text;
                        byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(text);         //if it is already an encrypted file, encrypt the text 
                        string encryptedText = Convert.ToBase64String(toEncodeAsBytes);
                        var t2 = new Task(() => saveAll(tabpage, encryptedText));           //run as another task
                        t2.Start();
                        Task.WaitAll(t2);                                                   //wait till task completes
                       
                    }
             
                  }

             
                } 
            
            catch (InvalidOperationException ee)
            {
                Console.WriteLine(ee);
            } 
        }

       

        private void saveAll(TabPage tabpg, string text)
        {
            try
            {

                path = (string)tabpg.Tag;
                File.WriteAllText(path, text);
            }
            catch (ArgumentNullException aaa)
            {
                Console.WriteLine(aaa);
            }

        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl.TabPageCollection tabcollection = tabControl1.TabPages;
            /*   foreach (TabPage tabbpage in tabcollection)
               {
                   var t3 = new Task(() => closeAll(tabbpage));
                   t3.Start();
               } */
            try
            {
                Task.Factory.StartNew(() =>

                {
                    try {
                        Parallel.ForEach(Partitioner.Create(0, tabControl1.TabCount), range =>
                      {

                          for (int i = range.Item1; i < range.Item2; i++)
                          {
                              this.Invoke(new MethodInvoker(() => closeAllTabs(tabControl1.TabPages[0])));
                          }


                      });
                    }
                    catch (ArgumentOutOfRangeException ee)
                    {
                        Console.WriteLine(ee);
                    }
                });



            } 
            catch(AggregateException ee)
            {
                foreach (Exception eee in ee.InnerExceptions) {
                    Console.WriteLine(eee);
                }
            }
            catch (ArgumentOutOfRangeException ee)
            {
                Console.WriteLine(ee);
            }


            /*      for(int i=0; i< tabControl1.TabCount+1; i++)
                  {
                      closeAll(i);
                  } */


        }

    /*    public delegate void closetabsDelegate(TabPage tp);
        private void closealltabsinvoke(TabPage tp)
        {

            if (this.InvokeRequired)
            {

                this.Invoke(new closetabsDelegate(closeAllTabs));
            }
            else
            {

                closeAllTabs(tp);
            }

        }  */

        private void closeAllTabs(TabPage tp)
        {
           
            try {
                string textfortab = (string)tp.Tag;
                tabControl1.TabPages.Remove(tp);
            }
            catch(ArgumentOutOfRangeException ee)
            {
                Console.WriteLine(ee);
            }
            
        }


        public delegate void spellCheckDelegate();
        private void spellCheck()
        {

            if (this.InvokeRequired)
            {

                this.Invoke(new spellCheckDelegate(spellchecker));
            }
            else
            {

                spellchecker();
            }

        }
        private void spellchecker()
        {
            TabPage tp = tabControl1.SelectedTab;
            Label mainlabel = tp.Controls[0] as Label;
            string lastWord = myrtb().Text.Split(' ').Last();  //take the last word from the rich text box
            lastWord = lastWord.Replace("\n", string.Empty);   //if it has a line break, replace it with empt string

            string[] output = lastWord.Split(' ');

            string a = "";

            string[] allwords = myrtb().Text.Split(' ');
            int countWords = lastWord.Count(x => { return !Char.IsWhiteSpace(x); });
            int countwordstotal = 0;
            try
            {
           //     foreach (string firstletter in output)
           //     {

                    a = lastWord[0].ToString();                 //take the first letter of the last word
                    a = a.ToUpper();
            //    }
                StreamReader sr = 
           new StreamReader(@"C:\Users\HP\Documents\Visual Studio 2015\Projects\Sharp Editor\allwords" + a + ".txt");  //pass the first letter of the last word with the spell check filename
                for (int i = 0; i < allwords.Length; i++)
                {
                    countwordstotal += allwords[i].Length;                 //counts the total letters in the rich text box

                }
                mainlabel.Controls[3].Text = "Total Letters: " + countwordstotal;
                myrtb().SelectionStart = countwordstotal + allwords.Length;
                myrtb().SelectionLength = countWords;
                myrtb().SelectionColor = Color.Black;
                if (countWords >= 1)
                {
                    string curval;
                    string chk = "false";

                    while ((curval = sr.ReadLine()) != null)
                    {

                        if (countWords == curval.Length && 
                            curval.Equals(lastWord, StringComparison.InvariantCultureIgnoreCase) && curval != null)     //if the last word in the rich text box is equal to the word in the file
                        {

                            myrtb().SelectionStart = countwordstotal - countWords + allwords.Length - 1;            //start point of the last word

                            myrtb().SelectionLength = countWords;                                                 //length of selection

                            int covr = myrtb().SelectionLength;
                            myrtb().SelectionColor = Color.Black;                                          //set the colour of the selected length to black
                            string nnn = myrtb().SelectedText;
                            myrtb().SelectionStart = countwordstotal + allwords.Length;                    //put the cursor back to the last position
                            chk = "true";

                            break;

                        }
                    }

                    if (chk.Equals("false"))                   //if the last word is not in the spell check file
                    {
                        myrtb().SelectionStart = countwordstotal - countWords + allwords.Length - 1;  //start positio of the cursor

                        myrtb().SelectionLength = countWords;       //selection length
                        int covr2 = myrtb().SelectionLength;
                        myrtb().SelectionColor = Color.Red;          //change the selected area to red

                        myrtb().SelectionStart = countwordstotal + allwords.Length; //put the cursor back to the last position

                    }

                }


            }
            catch (NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
            catch (IndexOutOfRangeException ee)
            {
                Console.WriteLine(ee);

            }
            catch (FileNotFoundException ee)
            {
                Console.WriteLine(ee);
            }
            catch (ArgumentNullException ee)
            {
                Console.WriteLine(ee);
            }
            catch (ArgumentException ee)
            {
                Console.WriteLine(ee);

            }
            catch (DirectoryNotFoundException ee)
            {
                Console.WriteLine(ee);

            }


        }

      

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;

            if ((printDialog1.ShowDialog()) == DialogResult.OK)
            {
                printDocument1.Print();
            }
            
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string textt = myrtb().Text;
            e.Graphics.DrawString(textt, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25,100));   //how tt should look like in the final output
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            if ((printPreviewDialog1.ShowDialog()) == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                string styleoffont = myrtb().SelectionFont.Style.ToString();

                if (styleoffont.Equals("Bold"))
                {
                    myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
                }
                else
                {
                    myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Bold);
                }
                myrtb().Select();
            }
            catch(NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try { 
            string styleoffont = myrtb().SelectionFont.Style.ToString();

            if (styleoffont.Equals("Underline"))
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
            }
            else
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Underline);
            }
            myrtb().Select();
        }
            catch (NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void button3Italics_Click(object sender, EventArgs e)
        {
            try { 
            string styleoffont = myrtb().SelectionFont.Style.ToString();

            if (styleoffont.Equals("Italic"))
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
            }
            else
            {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Italic);
            }
            myrtb().Select();
        }
            catch (NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void button4Regular_Click(object sender, EventArgs e)
        {
            try {
                myrtb().SelectionFont = new Font(myrtb().Font, FontStyle.Regular);
                myrtb().Select();
            }
            catch (NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = @"C:\";
                saveFileDialog1.Title = "Save text Files";
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                TabPage tp = tabControl1.SelectedTab;
                if (!(tp.Text).Contains("Encrypted"))
                {
                    if ((tp.Text).Equals("untitled *"))                        //if it is a new tab
                    {
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(saveFileDialog1.FileName, myrtb().Text);
                            path = saveFileDialog1.FileName;
                            tp.Tag = path;

                            String[] substrings = path.Split('\\');        //split using \ character
                            int noofStrings = substrings.Length;
                            tp.Text = substrings[noofStrings - 1];         //set the tab name to the file name
                            File.WriteAllLines(path, myrtb().Lines);      

                        }
                    }
                    else
                    {
                        try
                        {
                            path = (string)tp.Tag;                             //get the location from the tag field
                            File.WriteAllText(path, myrtb().Text);             //write the file to the given location
                            File.WriteAllLines(path, myrtb().Lines);

                        }
                        catch (ArgumentException ee)
                        {
                            Console.WriteLine(ee);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Saving an encrypted file as normal file will result in loss of Encryption !");
                }
            }
            catch(NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = @"C:\";
                saveFileDialog1.Title = "Save text Files";
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                TabPage tp = tabControl1.SelectedTab;
                if (!(tp.Text).Contains("Encrypted"))                          //check whether it has the keyword encrypted
                {

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog1.FileName, myrtb().Text);
                        path = saveFileDialog1.FileName;
                        tp.Tag = path;

                        String[] substrings = path.Split('\\');
                        int noofStrings = substrings.Length;
                        tp.Text = substrings[noofStrings - 1];                      //set the name of the tab to the filename
                        File.WriteAllLines(path, myrtb().Lines);                    //write the file to the locaiton given

                    }
                }

                else
                {
                    MessageBox.Show("Saving an encrypted file as normal file will result in loss of Encryption !");
                }
            }
            catch(NullReferenceException ee)
            {
                Console.WriteLine(ee);
            }
            }


        
    }

   

}
        

