using System;
using System.Collections.Generic;
using System.Text;

namespace Interpretor_NDC
{
    class Utils
    {
        public static int StrIndexOf(char ch, string str, int nr_skip)  // pozitia ch sarind peste primele skip aparitii
        {
            int count = 0;

            while (str.IndexOf(ch, count) >= 0)
            {
                if (nr_skip < 0)
                    return count;
                else
                {
                    count = str.IndexOf(ch, count + 1);
                    if (count == -1)
                        return -1;
                    nr_skip--;
                }
            }
            return -1;
        }

        public static int StrCount(char ch, string str, int start_index)   // returneaza numraul de aparitii a unui caracter intrun sir
        {
            int count = 0;
            int pos = start_index;

            while (str.IndexOf(ch, pos) >= 0)
            {
                pos = str.IndexOf(ch, pos + 1);                     // sare la urmatorul
                if (pos == -1)
                    return count;
                count++;
            }
            return -1;
        }

        public static string GetSubstring(string str, string begin, string end) // returneaza stringul dintre bigin si end
        {
            string temp = "";

            int start = str.IndexOf(begin);
            int stop = str.IndexOf(end);

            if (start >= 0 && stop > 0 && stop > start)
                temp = str.Substring(start, stop - start);

            return temp;
        }

        public static bool IsDigit(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return true;
            return false;
        }

        public static string ConvertCharToHexa(string temp) // 0x0 = <NUL>
        {
            //NUL 00 0
            temp = temp.Replace(((char)0).ToString(), "<NUL>");
            //SOH 01 1
            temp = temp.Replace(((char)1).ToString(), "<SOH>");
            //STX 02 2
            temp = temp.Replace(((char)2).ToString(), "<STX>");
            //ETX 03 3
            temp = temp.Replace(((char)3).ToString(), "<ETX>");
            //EOT 04 4
            temp = temp.Replace(((char)4).ToString(), "<EOT>");
            //ENQ 05 5
            temp = temp.Replace(((char)5).ToString(), "<ENQ>");
            //ACK 06 6
            temp = temp.Replace(((char)6).ToString(), "<ACK>");
            //BEL 07 7
            temp = temp.Replace(((char)7).ToString(), "<BEL>");
            //BS 08 8
            temp = temp.Replace(((char)8).ToString(), "<BS>");
            //HT 09 9
            temp = temp.Replace(((char)9).ToString(), "<HT>");
            //LF A 10
            temp = temp.Replace(((char)10).ToString(), "<LF>");
            //VT 0B 11
            temp = temp.Replace(((char)11).ToString(), "<VT>");
            //FF 0C 12
            temp = temp.Replace(((char)12).ToString(), "<FF>");
            //CR 0D 13
            temp = temp.Replace(((char)13).ToString(), "<CR>");
            //SO 0E 14
            temp = temp.Replace(((char)14).ToString(), "<SO>");
            //SI 0F 15
            temp = temp.Replace(((char)15).ToString(), "<SI>");
            //DLE 10 16
            temp = temp.Replace(((char)16).ToString(), "<DLE>");
            //DC1 11 17
            temp = temp.Replace(((char)17).ToString(), "<DC1>");
            //DC2 12 18
            temp = temp.Replace(((char)18).ToString(), "<DC2>");
            //DC3 13 19
            temp = temp.Replace(((char)19).ToString(), "<DC3>");
            //DC3 14 20
            temp = temp.Replace(((char)20).ToString(), "<DC4>");
            //NAK 15 21
            temp = temp.Replace(((char)15).ToString(), "<NAK>");
            //SYN 16 22 
            temp = temp.Replace(((char)22).ToString(), "<SYN>");
            //ETB 17 23
            temp = temp.Replace(((char)23).ToString(), "<ETB>");
            //CAM 18 24
            temp = temp.Replace(((char)24).ToString(), "<CAN>");
            //EM 19 25
            temp = temp.Replace(((char)25).ToString(), "<EM>");
            //SUB 1A 26
            temp = temp.Replace(((char)26).ToString(), "<SUB>");
            //ESC 1B 27
            temp = temp.Replace(((char)27).ToString(), "<ESC>");
            //FS 1C 28
            temp = temp.Replace(((char)28).ToString(), "<FS>");
            //GS 1D 29
            temp = temp.Replace(((char)29).ToString(), "<GS>");
            //RS 1E 30
            temp = temp.Replace(((char)30).ToString(), "<RS>");
            //US 1F 31
            temp = temp.Replace(((char)31).ToString(), "<RS>");
            //SP 20 32
            temp = temp.Replace(((char)32).ToString(), "<SP>");

            return temp;
        }

        public static string ConvertHexaToChar(string temp) // <NUL> = 0x0
        {
            //NUL 00 0
            temp = temp.Replace("<NUL>", ((char)0).ToString());
            //SOH 01 1
            temp = temp.Replace("<SOH>", ((char)1).ToString());
            //STX 02 2
            temp = temp.Replace("<STX>", ((char)2).ToString());
            //ETX 03 3
            temp = temp.Replace("<ETX>", ((char)3).ToString());
            //EOT 04 4
            temp = temp.Replace("<EOT>", ((char)4).ToString());
            //ENQ 05 5
            temp = temp.Replace("<ENQ>", ((char)5).ToString());
            //ACK 06 6
            temp = temp.Replace("<ACK>", ((char)6).ToString());
            //BEL 07 7
            temp = temp.Replace("<BEL>", ((char)7).ToString());
            //BS 08 8
            temp = temp.Replace("<BS>", ((char)8).ToString());
            //HT 09 9
            temp = temp.Replace("<HT>", ((char)9).ToString());
            //LF 0A 10
            temp = temp.Replace("<LF>", ((char)10).ToString());
            //VT 0B 11
            temp = temp.Replace("<VT>", ((char)11).ToString());
            //FF 0C 12
            temp = temp.Replace("<FF>", ((char)12).ToString());
            //CR 0D 13
            temp = temp.Replace("<CR>", ((char)13).ToString());
            //SO 0E 14
            temp = temp.Replace("<SO>", ((char)14).ToString());
            //SI 0F 15
            temp = temp.Replace("<SI>", ((char)15).ToString());
            //DLE 10 16
            temp = temp.Replace("<DLE>", ((char)16).ToString());
            //DC1 11 17
            temp = temp.Replace("<DC1>", ((char)17).ToString());
            //DC2 12 18
            temp = temp.Replace("<DC2>", ((char)18).ToString());
            //DC3 13 19
            temp = temp.Replace("<DC3>", ((char)19).ToString());
            //DC3 14 20
            temp = temp.Replace("<DC4>", ((char)20).ToString());
            //NAK 15 21 
            temp = temp.Replace("<NAK>", ((char)21).ToString());
            //SYN 16 22 
            temp = temp.Replace("<SYN>", ((char)22).ToString());
            //ETB 17 23
            temp = temp.Replace("<ETB>", ((char)23).ToString());
            //CAM 18 24
            temp = temp.Replace("<CAN>", ((char)24).ToString());
            //EM 19 25
            temp = temp.Replace("<EM>", ((char)25).ToString());
            //SUB 1A 26
            temp = temp.Replace("<SUB>", ((char)26).ToString());
            //ESC 1B 27
            temp = temp.Replace("<ESC>", ((char)27).ToString());
            //FS 1C 28
            temp = temp.Replace("<FS>", ((char)28).ToString());
            //GS 1D 29
            temp = temp.Replace("<GS>", ((char)29).ToString());
            //RS 1E 30
            temp = temp.Replace("<RS>", ((char)30).ToString());
            //US 1F 31
            temp = temp.Replace("<RS>", ((char)31).ToString());
            //SP 20 32
            temp = temp.Replace("<SP>", ((char)32).ToString());

            return temp;
        }
    }
}
