using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Interpretor_NDC
{
    // State tables load
    class ScreenKeyDataLoad : CustomisationDataCommands
    {
        public string[] ScreenNumber = null;
        public string[] ScreenData = null;
        public string MessageAuthenticationCode = "";

        public ScreenKeyDataLoad(string str)
            : base(str)
        {
            Name = "Screen/Key Load";

            int sep = Utils.StrIndexOf((char)28, str, 3);
            // nurara Screen-urile transmise
            int nrScreen = Utils.StrCount((char)28, str, sep-1);

            if (str[str.Length - 1] == (char)28)
                nrScreen--;

            // nu exista Screen-uri
            if (nrScreen < 0)
            {
                return;
            }
            else
            {
                ScreenNumber = new string[nrScreen];
                ScreenData = new string[nrScreen];
                // load state
                for (int i = 0; i < ScreenData.Length; i++)
                {
                    ScreenNumber[i] = str.Substring(sep + 1, ScreenKey.NrOfCharScreenNumber);
                    int lengthState = str.IndexOf((char)28, sep + 1) - sep - 1 - ScreenKey.NrOfCharScreenNumber;
                    if (lengthState < 0)
                        ScreenData[i] = str.Substring(sep + 1 + ScreenKey.NrOfCharScreenNumber);
                    else
                        ScreenData[i] = str.Substring(sep + 1 + ScreenKey.NrOfCharScreenNumber, lengthState);
                    sep = str.IndexOf((char)28, sep + 1);
                }
            }
            //FS
            // Optional
            if (str.Length - 1 < sep + 1) // afara din string
                return;

            //MessageAuthenticationCode = str.Substring(sep + 1, 8);
            //Trailer = str.Substring(sep + 1 + 8);
        }

        public static void SaveToFile(string strDownloadLine, string pathToSave, bool append_file)
        {
            ScreenKeyDataLoad temp = new ScreenKeyDataLoad( strDownloadLine );
            if( append_file == true )
            {
                FileInfo stateDB = new FileInfo(pathToSave);
                if (!stateDB.Exists)
                    return;
                StreamWriter append = stateDB.AppendText();

                for (int i = 0; i < temp.ScreenData.Length; i++)
                {
                    append.Write(temp.ScreenNumber[i]);
                    append.WriteLine(temp.ScreenData[i]);
                }
                append.Close();
            }
            else
            {
                FileInfo stateDB = new FileInfo(pathToSave);
                StreamWriter append = stateDB.CreateText();

                for (int i = 0; i < temp.ScreenData.Length; i++)
                {
                    append.Write(temp.ScreenNumber[i]);
                    append.WriteLine(temp.ScreenData[i]);
                }
                append.Close();
            }
        }
    }

    struct Voice
    {
        char id;
        string MessageNumber;
        char NumberOfConcatenatedMessages;

        public Voice(char _id, string _MessageNumber, char _NumberOfConcatenatedMessages)
        {
            id = _id;
            MessageNumber = _MessageNumber;
            NumberOfConcatenatedMessages = _NumberOfConcatenatedMessages;
        }
    };

    struct Photo
    {
        public string path;
        public int posX;
        public int posY;
        public Photo( string _path, int _posX, int _posY )
        {
            path = _path;
            posX = _posX;
            posY = _posY;
        }
    };

    class ScreenKey
    {
        public string numar = null;
        public string text = null;
        public int idle = -1;
        // list
        public List<ScreenKey> ScreenIdle = new List<ScreenKey>();
        public List<ScreenKey> SubScreen = new List<ScreenKey>();
        public List<Photo> GraphicPictures = new List<Photo>();
        public List<Photo> Logo = new List<Photo>();
        public List<Photo> Picture = new List<Photo>();
        public List<Photo> DisplayImageFileCommand = new List<Photo>();
        public List<Voice> VoiceList = new List<Voice>();

        public static List<ScreenKey> ScreenKeys = new List<ScreenKey>();
        public static byte NrOfCharScreenNumber = 3;

        public ScreenKey()
        {
            numar = "";
            text = "";
        }

        public ScreenKey(string _numar, string _text)
        {
            numar = _numar;
            text = _text;
        }

        public static ScreenKey GetScreenKey( string _numar )
        {
            for(int i = 0; i < ScreenKeys.Count; i++)
                if( ScreenKeys[i].numar == _numar )
                    return ScreenKeys[i];
            
            return null;
        }

        public static void LoadScreenKeys(string path)
        {

        }
    };

    class BoxScreenControl // o casuta din ecran
    {
        public Point P_sus;
        public Point P_jos;
        public char character;
        public string font;
        public float size;
        public Color colorBCK;
        public Brush brText;

        public static string[] NDCFONTS = { "NDCAlphanumeric1", "NDCAlphanumeric2", "NDCStandardGraphicsSet1", "NDCStandardGraphicsSet2", "NDCStandardGraphicsSet3", "NDCAlphanumeric1", "NDCAlphanumeric2", "NDCArabic", "NDCChinese1", "NDCChinese2", "NDCCustomerGraphics2" };
        public static float[] SIZEFONTS = {17.00f, 34.00f};
        
        public BoxScreenControl( Point _P_sus, Point _P_jos, char _character)
        {
            P_sus = new Point(_P_sus.X, _P_sus.Y);
            P_jos = new Point(_P_jos.X, _P_jos.Y);
            character = _character;
            colorBCK = Color.AliceBlue;
            brText = Brushes.YellowGreen;
            font = NDCFONTS[0];
            size = SIZEFONTS[0];
        }

        public BoxScreenControl( Point _P_sus, Point _P_jos )
        {
            P_sus = new Point(_P_sus.X, _P_sus.Y);
            P_jos = new Point(_P_jos.X, _P_jos.Y);
            character = (char)0;
            colorBCK = Color.AliceBlue;
            brText = Brushes.YellowGreen;
            font = NDCFONTS[0];
            size = SIZEFONTS[0];
        }

        public void DrawText(object sender, PaintEventArgs e)
        {
            if (character != (char)0 )
            {
                Rectangle tempRec = new Rectangle(P_sus.X, P_sus.Y, P_jos.X - P_sus.X, P_jos.Y - P_sus.Y);
                Brush brGradient = new LinearGradientBrush(tempRec, colorBCK, colorBCK, 45, false);
                e.Graphics.FillRectangle(brGradient, tempRec);

                e.Graphics.DrawString(character.ToString(), new Font(font, size), brText, P_sus.X, P_sus.Y);                
            }
        }
    };

    class ScreenControl : Panel
    {
        public BoxScreenControl[,] ScreenBox = new BoxScreenControl[32,16];
        public ScreenKey SK_Control = new ScreenKey();
        private char[] axes = { '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?' };
        
        public static bool ShowGrid = false;
        //public 
        public ScreenControl(bool visible)
        {
            this.BackColor = System.Drawing.Color.Navy;
            this.Location = new System.Drawing.Point(90, 133);
            this.Name = "panelVirtual";
            this.Size = new System.Drawing.Size(640, 480);
            this.Visible = visible;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawScreenControl);

            ScreenKey SK_Control = new ScreenKey();
            
            // init Box
            Point P1 = new Point(0, 0);
            Point P2 = new Point(20, 30);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    ScreenBox[j, i] = new BoxScreenControl(P1, P2);
                    P1.X += 20;
                    P2.X += 20;
                }
                P1.X = 0;
                P2.X = 20;
                P1.Y += 30;
                P2.Y += 30;
            }
        }

        //public 
        public ScreenControl(int PosX, int PosY)
        {
            this.BackColor = System.Drawing.Color.Navy;
            this.Location = new System.Drawing.Point(PosX, PosY);
            this.Name = "panelVirtual";
            this.Size = new System.Drawing.Size(640, 480);
            this.Visible = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawScreenControl);

            ScreenKey SK_Control = new ScreenKey();

            // init Box
            Point P1 = new Point(0, 0);
            Point P2 = new Point(20, 30);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    ScreenBox[j, i] = new BoxScreenControl(P1, P2);
                    P1.X += 20;
                    P2.X += 20;
                }
                P1.X = 0;
                P2.X = 20;
                P1.Y += 30;
                P2.Y += 30;
            }
        }

        public int GetPos(char ch)
        {
            for (int i = 0; i < axes.Length; i++)
                if (axes[i] == ch)
                    return i;
            return -1;
        }

        public ScreenKey LoadCharScreen( string strScreen, bool showIdleScreen, string numar ) // incarca aria screen-ului cu caractere
        {          
            SK_Control = new ScreenKey();
            SK_Control.numar = numar;
            SK_Control.text = strScreen;

            int ii = 0;
            int jj = 0;
            int offset = 0; // caractere pe lungime dubla

            string font = BoxScreenControl.NDCFONTS[0];
            float size = BoxScreenControl.SIZEFONTS[0];
            Color tempColorBCK = Color.Navy;
            Brush tempBrText = Brushes.Yellow;
            Brush lastTempBrText = Brushes.Yellow;

            // initial value
            int leftMargin = 0;

            ii = 0;
            jj = 0;
            //ClearScreenBox(0, 0);

            for(int i = 0; i < strScreen.Length; i++)
            {
                char ch = strScreen[i];
                switch(ch)
                {
                    case (char)0x0C:    // FF
                        ii = 0;
                        jj = 0;
                        ClearScreenBox(0,0);
                        tempColorBCK = Color.Navy;
                        tempBrText = Brushes.Yellow;
                        font = BoxScreenControl.NDCFONTS[0];
                        size = BoxScreenControl.SIZEFONTS[0];
                        offset = 0;
                        break;
                    case (char)0x0E:    // SO
                        if (strScreen.Length <= i+3)
                            return null;
                        ScreenKey temp = ScreenKey.GetScreenKey(strScreen.Substring(i + 1, ScreenKey.NrOfCharScreenNumber));

                        if (temp != null)
                        {
                            i += ScreenKey.NrOfCharScreenNumber;
                            // temp este un idle screen
                            if (strScreen.Length > i + 6 && strScreen[i + 1] == (char)0x1B && strScreen[i + 2] == '[' && strScreen[i + 6] == 'z')
                            {
                                temp.idle = Convert.ToInt16(strScreen.Substring(i + 3, 3));
                                SK_Control.ScreenIdle.Add(temp);
                                i += 6;
                            }
                            else
                            {
                                i -= ScreenKey.NrOfCharScreenNumber;
                                strScreen = strScreen.Remove(i, ScreenKey.NrOfCharScreenNumber+1);
                                strScreen = strScreen.Insert(i, temp.text);
                                SK_Control.SubScreen.Add(temp);                                
                                i--;
                            }
                        }
                        //‘G00’ – standard Insert Card picture
                        //‘G01’ – standard Press Key picture
                        //‘G03’ – standard Make A Deposit picture
                        //‘G04’ – standard Take Receipt picture
                        //‘G05’ – standard Take Money picture
                        //‘G06’ – standard Take Statement picture.   
                        else if (strScreen[i + 1] == 'G')// G – Graphic Pictures
                        {
                            string strPath = @"C:\NTGlobal\Final\XFS\Dll\" + strScreen.Substring(i + 1, 3) + ".pcx";
                            SK_Control.GraphicPictures.Add(new Photo(strPath, ii, jj));
                            i += 3;
                        }                    
                        break;
                    case (char)0x0F:    // SI
                        if( i + 2 < strScreen.Length )
                        {
                            int tempii = GetPos(strScreen[i + 1]);
                            if (tempii < 0)
                                break;
                            int tempjj = GetPos(strScreen[i + 2]);
                            if (tempjj < 0)
                                break;
                            ii = tempii;
                            jj = tempjj;
                            i = i + 2;
                        }
                        break;
                    case (char)0x09:    // HT
                        // Causes the name encoded on Track 1 of the card to be displayed, starting at the current cursor position (32 characters maximum). Both ISO and VISA display formats are supported.
                        // tab?
                        break;
                    case (char)0x11:    // DC1
                        break;
                    case (char)0x12:    // DC2
                        break;
                    case (char)0x1B:    // ESC
                        // Digital Audio Control Sequence
                        #region Digital Audio Control Sequence
                        if ( strScreen.Length > i + 8 && strScreen[i + 1] == 'P' && strScreen[i + 2] == '0' )
                        {
                            string terminatorVoice = "" + ((char)0x1B).ToString() + ('\\').ToString();
                            int stopVoice = strScreen.IndexOf(terminatorVoice, i+1);
                            char id = strScreen[i + 2];
                            string messageNumber = strScreen.Substring(i + 3, stopVoice - i - 5);
                            char nr = strScreen[stopVoice-1];
                            SK_Control.VoiceList.Add(new Voice(id, messageNumber, nr));
                            i = stopVoice + 1;
                        }
                        #endregion
                        #region Screen blinking and colour control
                        // Screen blinking and colour control
                        else if (strScreen.Length > i + 7 && strScreen[i + 4] == ';' && strScreen[i + 1] == '[')
                        {
                            string color = strScreen.Substring(i + 2, 2);
                            // background color
                            if (color == "30" || color == "C0")
                                tempColorBCK = Color.Black;
                            else if (color == "31" || color == "C1")
                                tempColorBCK = Color.Red;
                            else if (color == "C2" || color == "32")
                                tempColorBCK = Color.Green;
                            else if (color == "C3" || color == "33")
                                tempColorBCK = Color.Yellow;
                            else if (color == "34" || color == "C4")
                                tempColorBCK = Color.Blue;
                            else if (color == "35" || color == "C5")
                                tempColorBCK = Color.Magenta;
                            else if (color == "36" || color == "C6")
                                tempColorBCK = Color.Cyan;
                            else if (color == "C7" || color == "37")
                                tempColorBCK = Color.White;
                            else if (color == "80")
                                tempColorBCK = Color.Transparent;
                            // text color
                            else if (color == "20" || color == "BO")
                                tempBrText = Brushes.Black;
                            else if (color == "21" || color == "B1")
                                tempBrText = Brushes.Red;
                            else if (color == "B2" || color == "22")
                                tempBrText = Brushes.Green;
                            else if (color == "B3" || color == "23")
                                tempBrText = Brushes.Yellow;
                            else if (color == "24" || color == "B4")
                                tempBrText = Brushes.Blue;
                            else if (color == "25" || color == "B5")
                                tempBrText = Brushes.Magenta;
                            else if (color == "26" || color == "B6")
                                tempBrText = Brushes.Cyan;
                            else if (color == "B7" || color == "27")
                                tempBrText = Brushes.White;
                            // blink
                            else if (color == "10")
                            {
                                lastTempBrText = tempBrText;
                                tempBrText = Brushes.Violet;
                            }
                            else if (color == "11")
                                tempBrText = lastTempBrText;
                            else if (color == "00")
                                tempBrText = lastTempBrText;

                            i += 7;
                        }
                        // Screen blinking and colour control
                        else if (strScreen.Length > i + 5 && strScreen[i + 4] == 'm' && strScreen[i + 1] == '[')
                        {
                            string color = strScreen.Substring(i + 2, 2);
                            // background color
                            if (color == "30" || color == "C0")
                                tempColorBCK = Color.Black;
                            else if (color == "31" || color == "C1")
                                tempColorBCK = Color.Red;
                            else if (color == "C2" || color == "32")
                                tempColorBCK = Color.Green;
                            else if (color == "C3" || color == "33")
                                tempColorBCK = Color.Yellow;
                            else if (color == "34" || color == "C4")
                                tempColorBCK = Color.Blue;
                            else if (color == "35" || color == "C5")
                                tempColorBCK = Color.Magenta;
                            else if (color == "36" || color == "C6")
                                tempColorBCK = Color.Cyan;
                            else if (color == "C7" || color == "37")
                                tempColorBCK = Color.White;
                            else if (color == "80")
                                tempColorBCK = Color.Transparent;
                            // text color
                            else if (color == "20" || color == "BO")
                                tempBrText = Brushes.Black;
                            else if (color == "21" || color == "B1")
                                tempBrText = Brushes.Red;
                            else if (color == "B2" || color == "22")
                                tempBrText = Brushes.Green;
                            else if (color == "B3" || color == "23")
                                tempBrText = Brushes.Yellow;
                            else if (color == "24" || color == "B4")
                                tempBrText = Brushes.Blue;
                            else if (color == "25" || color == "B5")
                                tempBrText = Brushes.Magenta;
                            else if (color == "26" || color == "B6")
                                tempBrText = Brushes.Cyan;
                            else if (color == "B7" || color == "27")
                                tempBrText = Brushes.White;
                            // blink
                            else if (color == "10")
                            {
                                lastTempBrText = tempBrText;
                                tempBrText = Brushes.Violet;
                            }
                            else if (color == "11")
                                tempBrText = lastTempBrText;
                            else if (color == "00")
                                tempBrText = lastTempBrText;

                            i += 4;
                        }
                        #endregion
                        // Changing display in idle
                        else if (strScreen.Length > i + 6 && strScreen[i + 5] == 'z' && strScreen[i + 1] == '[')
                        {
                            int delayTime = Convert.ToInt16(strScreen.Substring(i + 2, 3));
                            i += 5;
                        }
                        #region Character set 1
                        // select character set - 1
                        else if (strScreen.Length > i + 2 && strScreen[i + 1] == '(')
                        {
                            switch(strScreen[i+2])
                            {
                                case '1':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[0];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '2':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[1];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '3':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[2];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '4':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[3];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '5':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[4];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '>':
                                    offset = 1;
                                    size = BoxScreenControl.SIZEFONTS[1];
                                    font = BoxScreenControl.NDCFONTS[5];
                                    break;
                                case '?':
                                    offset = 1;
                                    size = BoxScreenControl.SIZEFONTS[1];
                                    font = BoxScreenControl.NDCFONTS[6];
                                    break;
                                case '7':
                                    offset = 0;
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    font = BoxScreenControl.NDCFONTS[7];
                                    break;
                                case ':':
                                    offset = 0;
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    font = BoxScreenControl.NDCFONTS[8];
                                    break;
                                case ';':
                                    offset = 0;
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    font = BoxScreenControl.NDCFONTS[9];
                                    break;
                                case 'B':
                                    offset = 1;
                                    size = BoxScreenControl.SIZEFONTS[1];
                                    font = BoxScreenControl.NDCFONTS[10];
                                    break;
                            }
                            i = i + 2;
                        }
                        #endregion
                        #region Character set 2
                        // select character set - 2
                        else if (strScreen.Length > i + 2 && strScreen[i + 1] == ')')
                        {
                            switch (strScreen[i + 2])
                            {
                                case '1':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[0];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '2':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[1];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '3':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[2];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '4':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[3];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '5':
                                    offset = 0;
                                    font = BoxScreenControl.NDCFONTS[4];
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    break;
                                case '>':
                                    offset = 1;
                                    size = BoxScreenControl.SIZEFONTS[1];
                                    font = BoxScreenControl.NDCFONTS[5];
                                    break;
                                case '?':
                                    offset = 1;
                                    size = BoxScreenControl.SIZEFONTS[1];
                                    font = BoxScreenControl.NDCFONTS[6];
                                    break;
                                case '7':
                                    offset = 0;
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    font = BoxScreenControl.NDCFONTS[7];
                                    break;
                                case ':':
                                    offset = 0;
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    font = BoxScreenControl.NDCFONTS[8];
                                    break;
                                case ';':
                                    offset = 0;
                                    size = BoxScreenControl.SIZEFONTS[0];
                                    font = BoxScreenControl.NDCFONTS[9];
                                    break;
                                case 'B':
                                    offset = 1;
                                    size = BoxScreenControl.SIZEFONTS[1];
                                    font = BoxScreenControl.NDCFONTS[10];
                                    break;
                            }
                            i = i + 2;
                        }
                        #endregion
                        // Set Left Margin Position
                        else if (strScreen.Length > i + 5 && strScreen[i + 1] == '[' && strScreen[i + 5] == 'P')
                        {
                            leftMargin = Convert.ToInt32(strScreen.Substring(i+2, 2));
                            i = i + 5;
                        }
                        // select logo
                        else if (strScreen.Length > i + 5 && strScreen[i + 1] == 'P' && strScreen[i + 2] == '1')
                        {
                            string strPath = @"C:\NTGlobal\Final\XFS\Dll\pic00" + strScreen.Substring(i + 3, 2) + ".pcx";
                            SK_Control.Logo.Add(new Photo(strPath, ii, jj));
                            i += 5;
                        }
                        // Picture Control Command
                        else if (strScreen.Length > i + 5 && strScreen[i + 1] == 'P' && strScreen[i + 2] == '2')
                        {
                            string terminatorPicture = "" + ((char)0x1B).ToString() + ('\\').ToString();
                            int stopPicture = strScreen.IndexOf(terminatorPicture, i + 1);
                            string strPath = @"C:\NTGlobal\Final\XFS\Dll\pic00" + strScreen.Substring(i + 3, stopPicture - i - 3) + ".pcx";
                            SK_Control.Picture.Add(new Photo(strPath, ii, jj));
                            i = stopPicture + 1;
                        }
                        //
                        else if (strScreen.Length > i + 2 && strScreen[i + 1] == 'P' && strScreen[i + 2] == 'i') // not suported
                        {
                            string terminator = "" + ((char)0x1B).ToString() + ('\\').ToString();
                            int stop = strScreen.IndexOf(terminator, i + 1);
                            i = stop + 1;// +1;
                        }
                        // Display Image File Command
                        else if (strScreen.Length > i + 5 && strScreen[i + 1] == 'P' && strScreen[i + 2] == 'E')
                        {
                            string terminatorDisplay = "" + ((char)0x1B).ToString() + ('\\').ToString();
                            int stopDisplay = strScreen.IndexOf(terminatorDisplay, i + 1);
                            string strPath = @"C:\NTGlobal\Final\XFS\Dll" + strScreen.Substring(i + 3, stopDisplay - i - 3);// .avi?
                            SK_Control.DisplayImageFileCommand.Add(new Photo(strPath, ii, jj));
                            i = stopDisplay + 1;
                        }
                        // Display Lifted Cheque Image Command
                        else if (strScreen.Length > i + 5 && strScreen[i + 1] == 'P' && strScreen[i + 2] == 'E')
                        {
                            // not implement
                            string terminatorDisplay = "" + ((char)0x1B).ToString() + ('\\').ToString();
                            int stopDisplay = strScreen.IndexOf(terminatorDisplay, i + 1);
                            string strPath = @"C:\NTGlobal\Final\XFS\Dll\" + strScreen.Substring(i + 3, stopDisplay - i - 3);// .avi?
                            SK_Control.DisplayImageFileCommand.Add(new Photo(strPath, ii, jj));
                            i = stopDisplay + 1;
                        }
                        break;
                    case (char)0x0D:    // CR
                        ii++;
                        jj = leftMargin;
                        break;
                    case (char)0x0B:    // VT
                        break;
                    default:
                        {
                            if (ii >= 16)
                                break; 

                            ScreenBox[jj, ii].character = ch;
                            ScreenBox[jj, ii].colorBCK = tempColorBCK;
                            ScreenBox[jj, ii].brText = tempBrText;
                            ScreenBox[jj, ii].font = font;
                            ScreenBox[jj, ii].size = size;

                            jj = jj + offset + 1;
                            if( jj >= 32 )
                            {
                                jj = 0;
                                ii = ii + offset + 1;
                            }
                            break;
                        }
                }
                
            }

            if( !showIdleScreen )
                return SK_Control;

            ScreenKey tempSK = SK_Control;
            for (int i = 0; i < SK_Control.ScreenIdle.Count; i++)
            {
                LoadCharScreen(SK_Control.ScreenIdle[i].text, false, "");
                Refresh();
                System.Threading.Thread.Sleep(tempSK.ScreenIdle[i].idle/*30*/ * 10);
                SK_Control = tempSK;
            }
          
            return SK_Control;
        }

        public void ClearScreenBox(int posi, int posj)
        {
            for (int i = posi; i < 16; i++)
                for (int j = posj; j < 32; j++)
                    ScreenBox[j, i].character = (char)0;
        }

        public void DrawScreenControl(object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;            
                        
            for (int i = 0; i < SK_Control.GraphicPictures.Count; i++)
            {
                FileInfo temp = new FileInfo(SK_Control.GraphicPictures[i].path);
                if (temp.Exists)
                {
                    Bitmap b = new Bitmap(SK_Control.GraphicPictures[i].path.Replace(".pcx", ".jpg"));
                    G.DrawImage(b, SK_Control.GraphicPictures[i].posY * 20, SK_Control.GraphicPictures[i].posX * 30);
                }
                else
                    e.Graphics.DrawString(SK_Control.GraphicPictures[i].path, new Font("Arial", 7), Brushes.Turquoise, SK_Control.GraphicPictures[i].posY * 20, SK_Control.GraphicPictures[i].posX * 30);
            }

            for (int i = 0; i < SK_Control.Logo.Count; i++)
            {
                FileInfo temp = new FileInfo(SK_Control.Logo[i].path);
                if (temp.Exists)
                {
                    Bitmap b = new Bitmap(SK_Control.Logo[i].path.Replace(".pcx", ".jpg"));
                    G.DrawImage(b, SK_Control.Logo[i].posY * 20, SK_Control.Logo[i].posX * 30);
                }
                else
                    e.Graphics.DrawString(SK_Control.Logo[i].path, new Font("Arial", 7), Brushes.Turquoise, SK_Control.Logo[i].posY * 20, SK_Control.Logo[i].posX * 30);
            }

            for (int i = 0; i < SK_Control.Picture.Count; i++)
            {
                FileInfo temp = new FileInfo(SK_Control.Picture[i].path);
                if (temp.Exists)
                {
                    Bitmap b = new Bitmap(SK_Control.Picture[i].path.Replace(".pcx", ".jpg"));
                    G.DrawImage(b, SK_Control.Picture[i].posY * 20, SK_Control.Picture[i].posX * 30);
                }
                else
                    e.Graphics.DrawString(SK_Control.Picture[i].path, new Font("Arial", 7), Brushes.Turquoise, SK_Control.Picture[i].posY * 20, SK_Control.Picture[i].posX * 30);
            }

            for (int i = 0; i < SK_Control.DisplayImageFileCommand.Count; i++)
            {
                FileInfo temp = new FileInfo(SK_Control.DisplayImageFileCommand[i].path);
                if (temp.Exists)
                {
                    Bitmap b = new Bitmap(SK_Control.DisplayImageFileCommand[i].path.Replace(".pcx", ".jpg"));
                    G.DrawImage(b, SK_Control.DisplayImageFileCommand[i].posY * 20, SK_Control.DisplayImageFileCommand[i].posX * 30);
                }
                else
                    e.Graphics.DrawString(SK_Control.DisplayImageFileCommand[i].path, new Font("Arial", 7), Brushes.Turquoise, SK_Control.DisplayImageFileCommand[i].posY * 20, SK_Control.DisplayImageFileCommand[i].posX * 30);
            }

            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 32; j++)
                    ScreenBox[j, i].DrawText(sender, e);

            if (ShowGrid == true)
            {
                for (int i = 0; i < 32; i++)
                    G.DrawLine(new Pen(Color.Black), 20 * i, 0, 20 * i, 480);
                for (int i = 0; i < 16; i++)
                    G.DrawLine(new Pen(Color.Black), 0, 30 * i, 640, 30 * i);
            }
        }
        
    };
}
