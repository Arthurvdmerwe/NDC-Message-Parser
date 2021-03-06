using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Microsoft.Win32;


using System.Drawing.Drawing2D;

namespace Interpretor_NDC
{
    public partial class Form1 : Form
    {
        string[] textMessage = null;
        string beginMessage = "[";
        string endMessage = "]";
        string startSeparatorMessage = "{<(--";
        string stopSeparatorMessage = "--)]}";
        int skipChar = 0;

        ScreenControl SC = null;
        string pathFile = "";

        ScreenKey SK = new ScreenKey();
        StateTable st = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxInceputulMesajului.Text = beginMessage;
            textBoxSfarsitulMesajului.Text = endMessage;
        }

        private void textBoxInceputulMesajului_TextChanged(object sender, EventArgs e)
        {
            beginMessage = textBoxInceputulMesajului.Text;
        }

        private void textBoxSfarsitulMesajului_TextChanged(object sender, EventArgs e)
        {
            endMessage = textBoxSfarsitulMesajului.Text;
        }

        private void textBoxSariPestePrimele_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSariPestePrimele.Text == "")
                skipChar = 0;
            skipChar = Convert.ToInt32(textBoxSariPestePrimele.Text);
        }

        private void buttonArataLog_Click(object sender, EventArgs e)
        {
            openFileDialogMessageIn.ShowDialog();

            string pathFile = openFileDialogMessageIn.FileName;

            if (pathFile == "" || pathFile == null )
                return;

            StreamReader sr = new StreamReader(pathFile);
            string line = sr.ReadLine();
            textBoxArataLog.Text = "";
            while (line != null)
            {
                // precalculeaza hederul de mesaj(data si ora sau ora si numar de mesaj sau ce mai e....)
                if (line != "")
                {
                    if (line[0] == '[')
                    {
                        int lungimeHeder = line.IndexOf('[', 1)-1;
                        if (lungimeHeder > 0)
                            textBoxSariPestePrimele.Text = lungimeHeder.ToString();
                    }
                    else
                    {
                        int lungimeHeder = line.IndexOf('[', 1)-1;
                        if (lungimeHeder > 0)
                            textBoxSariPestePrimele.Text = lungimeHeder.ToString();
                    }
                }

                if (textBoxArataLog.Text.Length >= 2000)
                {
                    textBoxArataLog.Text += "---Out of text(2000)---\r\n";
                    return;
                }
                textBoxArataLog.Text += line + "\r\n";
                line = sr.ReadLine();
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialogMessageIn.FilterIndex = 1;
            openFileDialogMessageIn.ShowDialog();

            pathFile = openFileDialogMessageIn.FileName;            

            if (pathFile == "" || pathFile == null )
                return;
            File.Copy(pathFile, ((string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\NDC Tools", "Path", "")+@"\MssageIn.log"), true);
            pathFile = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\NDC Tools", "Path", "");

            StreamReader sr = new StreamReader(pathFile + @"\MssageIn.log");

            

            if (!Directory.Exists(pathFile + @"\SplitIn"))
                Directory.CreateDirectory(pathFile + @"\SplitIn");
            FileInfo f = new FileInfo(pathFile + @"\SplitIn\MessageIn.anl");
            StreamWriter writer = f.CreateText(); 

            string line = sr.ReadLine();

            int nrMessage = 0;
            bool multiline = false;
            
            while( line!=null )
            {
                if( multiline == false && line.Length < skipChar + 1 )       // elimina eventualele spatii dintre mesaje
                    ;
                else if( multiline == true )    // multiline mesaj
                {
                    string message = line;
                    if( !message.Contains(endMessage) ) // nu am ajuns la sfarsitul mesajului
                        writer.WriteLine(message);
                    else                                // sfarsitul mesajului multiline
                    {
                        multiline = false;
                        int stopMessaj = line.IndexOf(endMessage);
                        message = line.Substring(0, stopMessaj);
                        writer.WriteLine(message);
                        writer.WriteLine(stopSeparatorMessage);
                    }
                }
                else if( line.Substring(skipChar).Contains(beginMessage) && line.Substring(skipChar).Contains(endMessage) )  // mesaj de o singura linie
                {
                    multiline = false;
                    int startMesaj = line.IndexOf(beginMessage, skipChar) + beginMessage.Length;
                    int stopMesaj = line.IndexOf(endMessage, skipChar);
                    string message = line.Substring(startMesaj, stopMesaj - startMesaj);
                    string hederMessage = line.Substring(0, startMesaj-1);
                    writer.WriteLine("{0}{1} MsIn{2}", startSeparatorMessage, hederMessage, nrMessage); nrMessage++;
                    writer.WriteLine(message);
                    writer.WriteLine(stopSeparatorMessage);
                }
                else if( line.Substring(skipChar).Contains(beginMessage) && !line.Substring(skipChar).Contains(endMessage) )    // mesaj cu mai multe linii, asta e prima linie
                {
                    multiline = true;
                    int startMesaj = line.IndexOf(beginMessage, skipChar) + beginMessage.Length;
                    string message = line.Substring(startMesaj);
                    string hederMessage = line.Substring(0, startMesaj - 1);
                    writer.WriteLine("{0}{1} MsIn{2}", startSeparatorMessage, hederMessage, nrMessage); nrMessage++;
                    writer.WriteLine(message);
                }            

                line = sr.ReadLine();
            }

            sr.Close();
            writer.Close();


            sr = new StreamReader(pathFile + @"\SplitIn\MessageIn.anl");

            line = sr.ReadLine();
            textMessage = new string[nrMessage+1];
            int i = 0;
            listBoxMessageIn.Items.Clear();
            while( i < nrMessage )
            {
                if( line.Length < startSeparatorMessage.Length || line.Length < stopSeparatorMessage.Length )
                    textMessage[i] += line;
                else if( line.Substring(0,startSeparatorMessage.Length).Contains(startSeparatorMessage) )
                {
                    string itemsStr = "Ms" + (i+1).ToString();
                    listBoxMessageIn.Items.Add(itemsStr);
                    textMessage[i] = "";
                }
                else if (line.Substring(0, startSeparatorMessage.Length).Contains(stopSeparatorMessage))
                    i++;
                else
                    textMessage[i] += line;
                line = sr.ReadLine();
            }
            sr.Close();
        }

        private void buttonSplitSelect_Click(object sender, EventArgs e)
        {
            ScreenKey.ScreenKeys.Clear();
            StateTable.ListOfStateTables.Clear();
            StateTable.ViewStateTables.Clear();
            StateTable.RefreshViewStatesTables(comboBoxViewStates);

            StreamReader sr = new StreamReader(pathFile + @"\SplitIn\MessageIn.anl");
            
            // terminal commands
            sr.BaseStream.Position = 0;
            if (checkBoxTerminalCommands.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");
                
                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Terminal Commands.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();

                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        if (line[0] == '1' && line[2] == (char)28)
                            writer.WriteLine(line);
                    }
                    
                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // screen/keyboard data 
            sr.BaseStream.Position = 0;
            if (checkBoxScreenKeyboard.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Screen Keyboard.tdb");
                StreamWriter writer = f.CreateText();
                writer.Close();

                f = new FileInfo(pathFile + @"\SplitIn\Screen Keyboard.anl");
                writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == '1')  // 3.....11
                        {
                            ScreenKeyDataLoad.SaveToFile(line, pathFile + @"\SplitIn\Screen Keyboard.tdb", true);
                            writer.WriteLine(line);
                        }
                    }

                    line = sr.ReadLine();
                }
                writer.Close();

                ScreenKey.LoadScreenKeys(pathFile + @"\SplitIn\Screen Keyboard.tdb");
            }

            // state tables
            sr.BaseStream.Position = 0;
            if (checkBoxStateTables.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\State Tables.anl");
                StreamWriter writer = f.CreateText();

                FileInfo stateDB = new FileInfo(pathFile + @"\SplitIn\State Tables.tdb");
                StreamWriter writer2 = stateDB.CreateText();
                writer2.Close();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == '2')  // 3.....12
                        {
                            writer.WriteLine(line);
                            StateTablesLoad stl = new StateTablesLoad(line);
                            stl.SaveToFile(pathFile + @"\SplitIn\State Tables.tdb");
                        }
                    }
                    line = sr.ReadLine();
                }                
                writer.Close();

                StateTable.LoadListOfStateTables(pathFile + @"\SplitIn\State Tables.tdb");
                // load list 
                comboBoxViewStateTabel.Text = "ALL";
                listBoxStateTabels.Items.Clear();
                for (int i = 0; i < StateTable.ListOfStateTables.Count; i++)
                {
                    listBoxStateTabels.Items.Add(StateTable.ListOfStateTables[i].textTable);
                }
            }

            // configuration parameters
            sr.BaseStream.Position = 0;
            if (checkBoxConfigurationParameters.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Configuration Parameters.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == '3')  // 3.....13
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // FIT data
            sr.BaseStream.Position = 0;
            if (checkBoxFIT.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\FIT.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == '5')  // 3.....15
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Configuration ID Number Load
            sr.BaseStream.Position = 0;
            if (checkBoxConfigurationIDNumber.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Configuration ID Number.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == '6')  // 3.....16
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Enhanced Configuration Parameters Load
            sr.BaseStream.Position = 0;
            if (checkBox1EnhancedConfigurationParameters.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Enhanced Configuration Parameters.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == 'A')  // 3.....16
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // MAC Field Selection Load
            sr.BaseStream.Position = 0;
            if (checkBoxMACFieldSelection.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\MAC Field Selection.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == 'B')  // 3.....1B
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Date and Time
            sr.BaseStream.Position = 0;
            if (checkBoxDateAndTime.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Date and Time.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == 'C')  // 3.....1C
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Dispenser Currency Cassette Mapping Table
            sr.BaseStream.Position = 0;
            if (checkBoxDispencerCurrencyCassetteMapp.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Dispenser Currency Cassette Mapping Table.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == 'E')  // 3.....1E
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // XML Configuration Download
            sr.BaseStream.Position = 0;
            if (checkBoxXMLConfigurationDownload.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\XML Configuration Download.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '1' && line[posSep3 + 2] == 'I')  // 3.....1I
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Interactive Transaction Response
            sr.BaseStream.Position = 0;
            if (checkBoxInteractiveTransactionResponse.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Interactive Transaction Response.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '2' && (line[posSep3 + 2] >= '0' && line[posSep3 + 2] <= '9') )  // 3.....2(0-9)
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Encryption Key Change
            sr.BaseStream.Position = 0;
            if (checkBoxEncryptionKeyChange.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Encryption Key Change.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '3' && (line[posSep3 + 2] >= '1' && line[posSep3 + 2] <= '9'))  // 3.....3(1-9)
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Extended Encryption Key Change
            sr.BaseStream.Position = 0;
            if (checkBoxExtendedEncryptionKeyChange.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Extended Encryption Key Change.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '3' && line[posSep3 + 1] == '3' && ((line[posSep3 + 2] >= '1' && line[posSep3 + 2] <= '9') || (line[posSep3 + 2] >= 'A' && line[posSep3 + 2] <= 'K')))  // 4.....2(1-9)|(A-K)
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // HostToExitMessages
            sr.BaseStream.Position = 0;
            if (checkBoxHostToExitMessages.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Host to Exit Messages.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 5)
                    {
                        int posSep3 = Utils.StrIndexOf((char)28, line, 1);
                        if (posSep3 > 0 && line[0] == '7' && line[posSep3 + 1] == '1')  // 7.....1
                            writer.WriteLine(line);
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            // Transaction Reply Command
            sr.BaseStream.Position = 0;
            if (checkBoxTransactionReplyCommand.Checked == true)
            {
                if (!Directory.Exists(pathFile + @"\SplitIn"))
                    Directory.CreateDirectory(pathFile + @"\SplitIn");

                FileInfo f = new FileInfo(pathFile + @"\SplitIn\Transaction Reply Command.anl");
                StreamWriter writer = f.CreateText();

                string line = sr.ReadLine();
                bool mutiline = false;

                while (line != null)
                {
                    if (mutiline == true)
                    {
                        if (line.Contains(stopSeparatorMessage) && line[0] == stopSeparatorMessage[0]) // sfarsitul mesajului
                        {
                            mutiline = false;
                            writer.WriteLine(stopSeparatorMessage);
                        }
                        else
                            writer.WriteLine(line);
                    }

                    if (line.Length > 5)
                    {

                        int posSep3 = Utils.StrIndexOf((char)28, line, 2);
                        if (posSep3 > 0 && line[0] == '4')  // 4.....
                        {
                            mutiline = true;
                            writer.WriteLine(startSeparatorMessage);
                            writer.WriteLine(line);
                        }
                    }

                    line = sr.ReadLine();
                }
                writer.Close();
            }

            sr.Close();
        }

        private void comboBoxViewStateTabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxStateTabels.Items.Clear();
            for (int i = 0; i < StateTable.ListOfStateTables.Count; i++)
            {
                if (comboBoxViewStateTabel.Text == "ALL")
                    listBoxStateTabels.Items.Add(StateTable.ListOfStateTables[i].textTable);
                else
                {
                    if (StateTable.ListOfStateTables[i].textTable[3] == Convert.ToChar(comboBoxViewStateTabel.Text))
                        listBoxStateTabels.Items.Add(StateTable.ListOfStateTables[i].textTable);
                }
            }
        }

        private void checkBoxHexa_CheckedChanged(object sender, EventArgs e)
        {
            string temp = textBoxMessageIn.Text;
            if (checkBoxHexa.Checked == true)
                temp = Utils.ConvertCharToHexa(temp);
            else
                temp = Utils.ConvertHexaToChar(temp);

            textBoxMessageIn.Text = temp;
        }

        private void listBoxScreenKeyboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            SC.ClearScreenBox(0, 0);
            labelFF.Visible = true;

            SC.ScreenBox[0, 0] = new BoxScreenControl(new Point(0, 0), new Point(20, 30), 'N');
            string numarScreen = listBoxScreenKeyboard.Items[listBoxScreenKeyboard.SelectedIndex].ToString();
            textBoxScreen.Text = ScreenKey.GetScreenKey(numarScreen).text;
            SK = SC.LoadCharScreen(textBoxScreen.Text, false, ScreenKey.GetScreenKey(numarScreen).numar);
            SC.Visible = true;
            SC.Refresh();
            textBoxScreenView.Text = SK.numar;

            if ( textBoxScreen.Text.Contains(((char)0x0C).ToString()) )
                labelFF.Visible = false;
            labelVoice.Text = "Voices : " + SK.VoiceList.Count.ToString();
            labelGraphic.Text = "Graphics : " + SK.GraphicPictures.Count.ToString();
            labelPicture.Text = "Pictures : " + SK.Picture.Count.ToString();
            labelLogo.Text = "Logos : " + SK.Logo.Count.ToString();
            labelDisplayImageFile.Text = "Display Image Files : " + SK.DisplayImageFileCommand.Count.ToString();
            // idle screen
            labelIdleScreen.Text = "Idle screens : " + SK.ScreenIdle.Count.ToString();
            comboBoxIdleScreen.Items.Clear();
            comboBoxIdleScreen.Text = "";
            for (int i = 0; i < SK.ScreenIdle.Count; i++)
                comboBoxIdleScreen.Items.Add( SK.ScreenIdle[i].numar );
            
            // subscreen
            labelSubscreen.Text = "Subscreens : " + SK.SubScreen.Count.ToString();
            comboBoxSubscreen.Items.Clear();
            comboBoxSubscreen.Text = "";
            for (int i = 0; i < SK.SubScreen.Count; i++)
                comboBoxSubscreen.Items.Add(SK.SubScreen[i].numar);
        }

        private void checkBoxFF_CheckedChanged(object sender, EventArgs e)
        {
            string temp = textBoxScreen.Text;
            if (checkBoxHexaScreen.Checked == true)
                temp = Utils.ConvertCharToHexa(temp);
            else
                temp = Utils.ConvertHexaToChar(temp);

            textBoxScreen.Text = temp;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Screen
            listBoxScreenKeyboard.Items.Clear();
            //ScreenKey.ScreenKeys.Clear();
            this.tabPage3.Controls.Remove(SC);
            this.tabPageState3.Controls.Remove(SC);
            if (SC != null)
                SC.Dispose();
            SC = null;

            if( tabControl1.SelectedTab == tabPage3 ) // Screen
            {
                SC = new ScreenControl(true);
                this.tabPage3.Controls.Add(SC);

                if (!File.Exists(pathFile + @"\SplitIn\Screen Keyboard.tdb"))
                    return;
                StreamReader sr = new StreamReader(pathFile + @"\SplitIn\Screen Keyboard.tdb");
                string line = sr.ReadLine();
                while (line != null)
                {
                    ScreenKey.ScreenKeys.Add(new ScreenKey(line.Substring(0, ScreenKey.NrOfCharScreenNumber), line.Substring(ScreenKey.NrOfCharScreenNumber)));
                    listBoxScreenKeyboard.Items.Add(ScreenKey.ScreenKeys[ScreenKey.ScreenKeys.Count - 1].numar);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            else if (tabControl1.SelectedTab == tabPage4 && tabControlState.SelectedTab == tabPageState3) // State 
            {
                SC = new ScreenControl(10, 50);
                this.tabPageState3.Controls.Add(SC);
                                
            }
            
        }

        private void tabControlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tabPageState3.Controls.Remove(SC);
            if (tabControl1.SelectedTab == tabPage4 && tabControlState.SelectedTab == tabPageState3) // State 
            {
                SC = new ScreenControl(10, 50);
                this.tabPageState3.Controls.Add(SC);
                                
            }
        }

        private void listBoxMessageIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxMessageIn.Text = textMessage[listBoxMessageIn.SelectedIndex];
            int i = CentralToTerminal.FindMessageType(textBoxMessageIn.Text);
            checkBoxHexa.Checked = false;
        }

        private void checkBoxShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            ScreenControl.ShowGrid = checkBoxShowGrid.Checked;
            Refresh();
        }


        // Screen
        #region Screen
        private void buttonPlayVoices_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SK.VoiceList.Count; i++)
                ;
        }
        private void buttonPlayIdleScreens_Click(object sender, EventArgs e)
        {
            if (SK.ScreenIdle.Count < 1)
                return;
            listBoxScreenKeyboard.IsAccessible = false;
            SC.ClearScreenBox(0, 0);
            for (int i = 0; i < SK.ScreenIdle.Count; i++)
            {
                SC.LoadCharScreen(SK.ScreenIdle[i].text, true, "");
                Refresh();
                System.Threading.Thread.Sleep(SK.ScreenIdle[i].idle * 10);
            }
            SC.ClearScreenBox(0, 0);
            Refresh();
            listBoxScreenKeyboard.IsAccessible = true;
        }
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            SC.ClearScreenBox(0, 0);
            SC.LoadCharScreen(Utils.ConvertHexaToChar(textBoxScreen.Text), false, "");
            SC.Refresh();
        }
        private void comboBoxIdleScreen_SelectedIndexChanged(object sender, EventArgs e)
        {
            SC.ClearScreenBox(0, 0);
            ScreenKey temp = ScreenKey.GetScreenKey(comboBoxIdleScreen.Text);
            SC.LoadCharScreen(temp.text, false, temp.numar);
            textBoxScreenView.Text = temp.numar;
            textBoxScreen.Text = temp.text;
            SC.Refresh();
        }
        private void comboBoxSubscreen_SelectedIndexChanged(object sender, EventArgs e)
        {
            SC.ClearScreenBox(0, 0);
            ScreenKey temp = ScreenKey.GetScreenKey(comboBoxSubscreen.Text);
            SC.LoadCharScreen(temp.text, false, temp.numar);
            textBoxScreenView.Text = temp.numar;
            textBoxScreen.Text = temp.text;
            SC.Refresh();
        }
        private void textBoxScreen_TextChanged(object sender, EventArgs e)
        {
            SC.ClearScreenBox(0, 0);
            SC.LoadCharScreen(Utils.ConvertHexaToChar(textBoxScreen.Text), false, "");
            SC.Refresh();
        }
        #endregion
        #region State
            #region Common
            private void listBoxStateTabels_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (listBoxStateTabels.SelectedIndex < 0)
                    return;

                st = new StateTable(listBoxStateTabels.Items[listBoxStateTabels.SelectedIndex].ToString());

                if (st == null)
                    return;

                st.particulariseState(@"C:\Config.xml");

                // load intro view states
                // remove

                // common
                // add
                //StateTable.ViewStateTables.Add(st);
                //StateTable.RefreshViewStatesTables(comboBoxViewStates);
                
                // Refresh details
                RefreshDetails(st);

                // Play
                RefreshPlay(st);
                
            }
            #endregion            

            // State -> Details
            #region Details
            private void RefreshDetails( StateTable st )
            {
                label1.Text = st.PART[0].name;
                label2.Text = st.PART[1].name;
                label3.Text = st.PART[2].name;
                label4.Text = st.PART[3].name;
                label5.Text = st.PART[4].name;
                label6.Text = st.PART[5].name;
                label7.Text = st.PART[6].name;
                label8.Text = st.PART[7].name;
                label9.Text = st.PART[8].name;
                label10.Text = st.PART[9].name;
                // ext
                if (st.PART.Length >= 20)
                {
                    label11.Text = st.PART[10].name;
                    label12.Text = st.PART[11].name;
                    label13.Text = st.PART[12].name;
                    label14.Text = st.PART[13].name;
                    label15.Text = st.PART[14].name;
                    label16.Text = st.PART[15].name;
                    label17.Text = st.PART[16].name;
                    label18.Text = st.PART[17].name;
                    label19.Text = st.PART[18].name;
                    label20.Text = st.PART[19].name;
                }

                // ext 2
                if (st.PART.Length >= 30)
                {
                    label21.Text = st.PART[20].name;
                    label22.Text = st.PART[21].name;
                    label23.Text = st.PART[22].name;
                    label24.Text = st.PART[23].name;
                    label25.Text = st.PART[24].name;
                    label26.Text = st.PART[25].name;
                    label27.Text = st.PART[26].name;
                    label28.Text = st.PART[27].name;
                    label29.Text = st.PART[28].name;
                    label30.Text = st.PART[29].name;
                }


                textBox1.Text = st.PART[0].value;
                textBox2.Text = st.PART[1].value;
                textBox3.Text = st.PART[2].value;
                textBox4.Text = st.PART[3].value;
                textBox5.Text = st.PART[4].value;
                textBox6.Text = st.PART[5].value;
                textBox7.Text = st.PART[6].value;
                textBox8.Text = st.PART[7].value;
                textBox9.Text = st.PART[8].value;
                textBox10.Text = st.PART[9].value;
                //
                if (st.PART.Length >= 20)
                {
                    textBox11.Text = st.PART[10].value;
                    textBox12.Text = st.PART[11].value;
                    textBox13.Text = st.PART[12].value;
                    textBox14.Text = st.PART[13].value;
                    textBox15.Text = st.PART[14].value;
                    textBox16.Text = st.PART[15].value;
                    textBox17.Text = st.PART[16].value;
                    textBox18.Text = st.PART[17].value;
                    textBox19.Text = st.PART[18].value;
                    textBox20.Text = st.PART[19].value;
                }
                else
                    textBox11.Text = "";
                //
                if (st.PART.Length >= 30)
                {
                    textBox21.Text = st.PART[20].value;
                    textBox22.Text = st.PART[21].value;
                    textBox23.Text = st.PART[22].value;
                    textBox24.Text = st.PART[23].value;
                    textBox25.Text = st.PART[24].value;
                    textBox26.Text = st.PART[25].value;
                    textBox27.Text = st.PART[26].value;
                    textBox28.Text = st.PART[27].value;
                    textBox29.Text = st.PART[28].value;
                    textBox30.Text = st.PART[29].value;
                }
                else
                    textBox21.Text = "";

                textBoxDescriptionPart1.Text = st.PART[0].description;
                textBoxDescriptionPart2.Text = st.PART[1].description;
                textBoxDescriptionPart3.Text = st.PART[2].description;
                textBoxDescriptionPart4.Text = st.PART[3].description;
                textBoxDescriptionPart5.Text = st.PART[4].description;
                textBoxDescriptionPart6.Text = st.PART[5].description;
                textBoxDescriptionPart7.Text = st.PART[6].description;
                textBoxDescriptionPart8.Text = st.PART[7].description;
                textBoxDescriptionPart9.Text = st.PART[8].description;
                textBoxDescriptionPart10.Text = st.PART[9].description;
                //
                if (st.PART.Length >= 20)
                {
                    textBoxDescriptionPart11.Text = st.PART[10].description;
                    textBoxDescriptionPart12.Text = st.PART[11].description;
                    textBoxDescriptionPart13.Text = st.PART[12].description;
                    textBoxDescriptionPart14.Text = st.PART[13].description;
                    textBoxDescriptionPart15.Text = st.PART[14].description;
                    textBoxDescriptionPart16.Text = st.PART[15].description;
                    textBoxDescriptionPart17.Text = st.PART[16].description;
                    textBoxDescriptionPart18.Text = st.PART[17].description;
                    textBoxDescriptionPart19.Text = st.PART[18].description;
                    textBoxDescriptionPart20.Text = st.PART[19].description;
                }

                //
                if (st.PART.Length >= 30)
                {
                    textBoxDescriptionPart21.Text = st.PART[20].description;
                    textBoxDescriptionPart22.Text = st.PART[21].description;
                    textBoxDescriptionPart23.Text = st.PART[22].description;
                    textBoxDescriptionPart24.Text = st.PART[23].description;
                    textBoxDescriptionPart25.Text = st.PART[24].description;
                    textBoxDescriptionPart26.Text = st.PART[25].description;
                    textBoxDescriptionPart27.Text = st.PART[26].description;
                    textBoxDescriptionPart28.Text = st.PART[27].description;
                    textBoxDescriptionPart29.Text = st.PART[28].description;
                    textBoxDescriptionPart30.Text = st.PART[29].description;
                }

                // conditie de deschidere
                if (textBox11.Text != "")
                    groupBox2.Visible = true;
                else
                    groupBox2.Visible = false;
                //
                if (textBox21.Text != "")
                    groupBox3.Visible = true;
                else
                    groupBox3.Visible = false;
            }
            #endregion

            // State -> Map
            #region Map

            #endregion

            // State -> Play
            #region Play
            private void RefreshPlay( StateTable st )
            {
                labelActualState.Text = "Actual state : ";
                labelDislpayScreen.Text = "Display screen : ";
                treeViewStatePart.Nodes[0].Nodes.Clear();
                treeViewStatePart.Nodes[1].Nodes.Clear();

                for (int i = 0; i < st.PART.Length; i++)
                {
                    if (st.PART[i].mask == null || SC == null)
                        break;

                    if (st.PART[i].mask == "SCREEN_DISPLAY")
                    {
                        ScreenKey temp = ScreenKey.GetScreenKey(st.PART[i].value);
                        if (temp == null)
                        {
                            labelDislpayScreen.Text += "NOT EXIST!!!";
                            //break;
                        }
                        else
                        {
                            SC.LoadCharScreen(temp.text, false, temp.numar);
                            Refresh();
                            labelDislpayScreen.Text = "Display screen : " + temp.numar;
                        }



                        treeViewStatePart.Nodes[0].Nodes.Add(st.PART[i].value + "->" + st.PART[i].name);
                        //break;
                    }
                    else if (st.PART[i].mask.Contains("SCREEN_"))
                        treeViewStatePart.Nodes[0].Nodes.Add(st.PART[i].value + "->" + st.PART[i].name);
                    else if (st.PART[i].mask.Contains("STATE_"))
                    {
                        if (!st.PART[i].mask.Contains("STATE_EXTENSION") && !st.PART[i].mask.Contains("STATE_EXTENDED"))
                            treeViewStatePart.Nodes[1].Nodes.Add(st.PART[i].value + "->" + st.PART[i].name);
                        else if (st.PART[i].mask.Contains("STATE_EXTENDED"))
                            treeViewStatePart.Nodes[1].Nodes.Add("---EXTENDED---" + st.PART[i + 1].description);
                    }
                    else if (st.PART[i].mask.Contains("NUMBER") && i > 0)
                        treeViewStatePart.Nodes[1].Nodes.Add("---EXTENDED---" + st.PART[i + 1].description);

                }
                labelActualState.Text += st.PART[0].value;
            }
            private void treeViewStatePart_AfterSelect(object sender, TreeViewEventArgs e)
            {
                labelDislpayScreen.Text = "Display screen : ";
                SC.ClearScreenBox(0, 0);
                SC.Refresh();

                if (treeViewStatePart.SelectedNode.Parent == null)
                    return;
                else if (treeViewStatePart.SelectedNode.Parent.Text == "SCREENS")
                {
                    SC.ClearScreenBox(0, 0);
                    string screenNr = treeViewStatePart.SelectedNode.Text.Substring(0, treeViewStatePart.SelectedNode.Text.IndexOf("->"));
                    ScreenKey tempSK = ScreenKey.GetScreenKey(screenNr);

                    if (tempSK == null)
                    {
                        SC.LoadCharScreen(((char)0x1B).ToString() + "(1<NOT EXIST>", false, screenNr);
                        SC.Refresh();
                        return;
                    }
                    labelDislpayScreen.Text = "Display screen : " + screenNr.ToString();
                    SC.LoadCharScreen(ScreenKey.GetScreenKey(screenNr).text, true, screenNr);
                    SC.Refresh();
                }
                else if (treeViewStatePart.SelectedNode.Parent.Text == "STATES" && treeViewStatePart.SelectedNode.Text[0] != '-')
                {
                    if (StateTable.ViewStateTables.Count == 0)
                    {
                        StateTable st = new StateTable(listBoxStateTabels.Items[listBoxStateTabels.SelectedIndex].ToString());
                        StateTable.ViewStateTables.Add(st);
                    }

                    string stateNr = treeViewStatePart.SelectedNode.Text.Substring(0, treeViewStatePart.SelectedNode.Text.IndexOf("->"));
                    StateTable temp = StateTable.FindStateTabelsInList(stateNr);
                    int i = listBoxStateTabels.FindString(temp.textTable);
                    listBoxStateTabels.SelectedIndex = i;

                    StateTable.RemoveViewStatesTables(StateTable.SelectViewStates);
                    StateTable.ViewStateTables.Add(temp);
                    StateTable.RefreshViewStatesTables(comboBoxViewStates);
                }
            }
            private void buttonClearAllViewStates_Click(object sender, EventArgs e)
            {
                StateTable.ViewStateTables.Clear();
                StateTable.RefreshViewStatesTables(comboBoxViewStates);
                StateTable.SelectViewStates = -1;
            }
            private void buttonUndoViewStates_Click(object sender, EventArgs e)
            {
                if (StateTable.SelectViewStates >= 1)
                    StateTable.SelectViewStates--;
                else if (StateTable.SelectViewStates < 0)
                    StateTable.SelectViewStates = StateTable.ViewStateTables.Count - 1;
                else
                    StateTable.SelectViewStates = 0;

                string stateNr = StateTable.ViewStateTables[StateTable.SelectViewStates].PART[0].value;
                comboBoxViewStates.Text = stateNr + StateTable.ViewStateTables[StateTable.SelectViewStates].PART[1].value;
            }
            private void buttonRedoViewStates_Click(object sender, EventArgs e)
            {
                if (StateTable.SelectViewStates < StateTable.ViewStateTables.Count - 1 && StateTable.SelectViewStates >= 0)
                    StateTable.SelectViewStates++;
                else if (StateTable.SelectViewStates < 0)
                    StateTable.SelectViewStates = 0;

                string stateNr = StateTable.ViewStateTables[StateTable.SelectViewStates].PART[0].value;
                comboBoxViewStates.Text = stateNr + StateTable.ViewStateTables[StateTable.SelectViewStates].PART[1].value;
            }
            #endregion

            // State -> Tools
            #region Tools
            private void buttonFindState_Click(object sender, EventArgs e)
            {
                listBoxFindState.Items.Clear();

                if (textBoxFindState.Text.Length < 3 || textBoxFindState.Text.Length > 3)
                {
                    listBoxFindState.Items.Add("ERROR -> lungimea trebuie sa fie fix de 3 caractere");
                    return;
                }
                List<string> temp = StateTable.FindStateInAllStates(textBoxFindState.Text, "STATE_");
                for (int i = 0; i < temp.Count; i++)
                    listBoxFindState.Items.Add(temp[i]);
            }
            
            private void comboBoxViewStates_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (comboBoxViewStates.Text == "")
                    return;
                string stateNr = comboBoxViewStates.Text.Substring(0, 3);
                StateTable temp = StateTable.FindStateTabelsInList(stateNr);
                int i = listBoxStateTabels.FindString(temp.textTable);
                listBoxStateTabels.SelectedIndex = i;
                StateTable.SelectViewStates = comboBoxViewStates.SelectedIndex;
            }
            #endregion

        #endregion

        };
   
}
