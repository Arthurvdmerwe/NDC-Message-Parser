using System;
using System.Collections.Generic;
using System.Text;

namespace Interpretor_NDC
{
    class CentralToTerminal
    {
        public char MessageClass = '0';
        public char ResponseFlag = '0';
        public string LUNO = "000";
        public string MessageSequenceNumber = "000";
        public string Name = "";
        public string MesajNDC = "";       // mesajul ndc care este procesat
        public string Trailer = "";
        // constr
        public CentralToTerminal(int neutilizat)
        {
            MessageClass = Convert.ToChar(MesajNDC[0]);
            if (MesajNDC[1] != 28)
                ResponseFlag = Convert.ToChar(MesajNDC[1]);
        }
        public CentralToTerminal(string str)
        {
            int sep1 = str.IndexOf((char)28, 0);
            int sep2 = str.IndexOf((char)28, sep1 + 1);
            int sep3 = str.IndexOf((char)28, sep2 + 1);
            MesajNDC = str;

            MessageClass = Convert.ToChar(MesajNDC[0]);
            if (MesajNDC[1] != 28)
                ResponseFlag = Convert.ToChar(MesajNDC[1]);
            // optional
            if (str[1] != 28)
                ResponseFlag = Convert.ToChar(str[1]);
            //FS
            if (sep1 + 1 < sep2)
                LUNO = str.Substring(sep1 + 1, 3);
            else
                LUNO = "";
            //FS
            if (sep2 + 1 < sep3)
                MessageSequenceNumber = str.Substring(sep2 + 1, 3);
            else
                MessageSequenceNumber = "";
            //FS        
        }
        // 0 - terminal commands
        // 1 - Screen/Keyboard Data Load
        // 2 - State Tables Load
        // 3 - Configuration Parameters Load
        // 4 - Reserved
        // 5 - FIT Data Load
        // 6 - Configu ration ID Number Load
        // 7 - Enhanced Configuration Parameters Load
        // 8 - MAC Field Selection Load
        // 9 - Date and Time Load
        //10 - Reserved
        //11 - Dispenser Currency Cassette Mapping Table
        //12 - Reserved
        //13 - Reserved
        //14 - XML Configuration Download
        //15 - Interactive Transaction Response
        //16 - Encryption Key Change
        //17 - Extended Encryption Key Change
        public static int FindMessageType(string str)
        {
            int sep1 = str.IndexOf((char)28, 0);
            int sep2 = str.IndexOf((char)28, sep1 + 1);
            int sep3 = str.IndexOf((char)28, sep2 + 1);

            if (str[0] == '1') // terminal commands
                return 0;
            else if (str[0] == '3') // Customisation Data Commands
            {
                switch(str[sep3+1])
                {
                    case '1':
                        switch (str[sep3 + 2])
                        {
                            case '1':
                                return 1;
                            case '2':
                                return 2;
                            case '3':
                                return 3;
                            case '4':
                                return 4;
                            case '5':
                                return 5;
                            case '6':
                                return 6;
                            case 'A':
                                return 7;
                            case 'B':
                                return 8;
                            case 'C':
                                return 9;
                            case 'D':
                                return 10;
                            case 'E':
                                return 11;
                            case 'F':
                                return 12;
                            case 'G':
                                return 13;
                            case 'I':
                                return 14;
                        }
                        break;
                    case '2':
                        if (str[sep3 + 2] >= '0' && str[sep3 + 2] <= '9')
                            return 15;
                        break;
                    case '3':
                        if (str[sep3 + 2] >= '1' && str[sep3 + 2] <= '9')
                            return 16;
                        break;
                    case '4':
                        if ((str[sep3 + 2] >= '1' && str[sep3 + 2] <= '9') && ((str[sep3 + 2] >= 'A' && str[sep3 + 2] <= 'K')))
                            return 17;
                        break;
                };
            }
            return -1;
        }
    };

    class TerminalComand : CentralToTerminal
    {
        public char CommandCode = '0';
        public char CommandModifier = '0';

        public TerminalComand(string str)
            : base(str)
        {
            int sep3 = Utils.StrIndexOf((char)28, str, 2);
            //FS
            CommandCode = Convert.ToChar(str[sep3 + 1]);
            if (str.Length - 1 < sep3 + 2)
                return;
            // optional
            CommandModifier = Convert.ToChar(str[sep3 + 2]);
            Trailer = str.Substring(sep3 + 3);

            Name = "Terminal Command";
        }
    };
}