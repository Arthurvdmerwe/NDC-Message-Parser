using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace Interpretor_NDC
{
    // State tables load
    class StateTablesLoad : CustomisationDataCommands
    {
        public bool[] StateOk = null;
        public string[] StateTableData = null;
        public string MessageAuthenticationCode = "";

        public StateTablesLoad(string str)
            : base(str)
        {
            Name = "State Tables Load";

            int sep = Utils.StrIndexOf((char)28, str, 3);
            // nurara State-urile transmise
            int nrState = GetNoStates(str);

            // nu exista State-uri
            if (nrState < 0)
            {
                StateOk = new bool[1];
                StateTableData = new string[1];
                StateOk[0] = false;
                StateTableData[0] = "ERROR";
            }
            else
            {
                StateOk = new bool[nrState];
                StateTableData = new string[nrState];
                // load state
                for (int i = 0; i < StateTableData.Length; i++)
                {
                    int start = Utils.StrIndexOf((char)28, str, 3 + i);
                    int stop = Utils.StrIndexOf((char)28, str, 3 + i + 1);
                    if (stop < 0)
                        stop = str.Length;

                    StateTableData[i] = str.Substring(start + 1, stop - 1 - start);
                    if (StateTableData[i].Length != 28)
                        StateOk[i] = false;
                    else
                        StateOk[i] = true;

                    sep = stop;
                }
            }
            //FS
            if (sep >= str.Length - 1)
                return;

            MessageAuthenticationCode = str.Substring(sep + 1, 8);
            Trailer = str.Substring(sep + 1 + 8);
        }

        public int GetNoStates(string str) // da numarul de state-uri din str
        {
            int reIndex = Utils.StrIndexOf((char)28, str, 3);   // se sare peste antetul de inceput si se trece la primul state probabil

            if (reIndex < 0)
                return -1;  // nu exista state-uri => eroare

            // numara posibilele state-uri
            int noStates = 0;
            while (reIndex > 0)
            {
                int lastIndex = reIndex;
                noStates++;
                reIndex = Utils.StrIndexOf((char)28, str, 3 + noStates); // urmatorul separator
                // urmeaza un state?
                if (reIndex < -1) // nu mai exista separator
                {
                    // ultimul a fost state sau nu?
                    if (lastIndex + 28 == str.Length - 1) // a fost
                        break;
                    else // nu a fost
                        noStates--;
                }
                else if (reIndex == str.Length - 1) // se termina brusc cu un separator
                    break;
            }
            return noStates;
        }

        public void SaveToFile(string pathToSave)
        {
            FileInfo stateDB = new FileInfo(pathToSave);
            if (!stateDB.Exists)
                return;
            StreamWriter append = stateDB.AppendText();

            for (int i = 0; i < StateTableData.Length; i++)
                append.WriteLine(StateTableData[i]);

            append.Close();
        }       

    };

    struct PartStateTable
    {
        public string value; // cele 8 parti 8*3
        public string description;//
        public string name; //
        public string mask;
        public PartStateTable(string _value, string _description, string _name, string _mask)
        {
            value = _value;
            description = _description;
            name = _name;
            mask = _mask;
        }

    };

    // State Table
    class StateTable
    {
        public PartStateTable[] PART;// = new PartStateTable[10];
        public string textTable;

        public static List<StateTable> ListOfStateTables = new List<StateTable>();
        public static List<StateTable> ViewStateTables = new List<StateTable>();
        public static int SelectViewStates = -1;

        public static void RefreshViewStatesTables( ComboBox CB )
        {
            CB.Items.Clear();
            for(int i = 0; i < ViewStateTables.Count; i++)
                CB.Items.Add(ViewStateTables[i].PART[0].value + ViewStateTables[i].PART[1].value);
            SelectViewStates = -1;
        }

        public static void RemoveViewStatesTables(int index)
        {
            if (index < 0 || index > ViewStateTables.Count - 1)
                return;
            for (int i = index+1; i < ViewStateTables.Count; )
                ViewStateTables.RemoveAt(i);

            //SelectViewStates = index;
        }

        public StateTable FindStateTabelsInListAll(string numberState)
        {
            for (int i = 0; i < ListOfStateTables.Count; i++)
            {
                if (ListOfStateTables[i].PART[0].value == numberState)
                    return ListOfStateTables[i];
            }                
            return null;
        }

        public static StateTable FindStateTabelsInList(string numberState)
        {
            for (int i = 0; i < ListOfStateTables.Count; i++)
            {
                if (ListOfStateTables[i].PART[0].value == numberState)
                    return ListOfStateTables[i];
            }
            return null;
        }

        public static List<string> FindStateInAllStates( string number, string mask )
        {
            List<string> state = new List<string>();

            for (int i = 0; i < ListOfStateTables.Count; i++)
            {
                ListOfStateTables[i].particulariseState(@"C:\config.xml");
                for (int j = 0; j < ListOfStateTables[i].PART.Length; j++)
                {
                    if (ListOfStateTables[i].PART[j].mask != null && ListOfStateTables[i].PART[j].mask.Contains(mask) && ListOfStateTables[i].PART[j].value == number)
                        state.Add(ListOfStateTables[i].PART[0].value + ListOfStateTables[i].PART[1].value + "->" + number);
                }
            }
            return state;
        }

        public StateTable FindStateTabelsInListAll( char stateType )
        {
            for (int i = 0; i < ListOfStateTables.Count; i++)
            {
                if (ListOfStateTables[i].PART[1].value == stateType.ToString())
                    return ListOfStateTables[i];
            }
            return null;
        }

        public StateTable(string strNumarTypeCommon) // mesajul transmis cu numar tip si partea comuna
        {
            textTable = strNumarTypeCommon;
            PART = new PartStateTable[30];

            PART[0].value = strNumarTypeCommon.Substring(0, 3);
            PART[0].name = "###";
            PART[0].description = "";
            PART[1].value = strNumarTypeCommon.Substring(3, 1);
            PART[1].name = "###";
            PART[1].description = "";

            if (strNumarTypeCommon.Length < 28)
                return;


            for (int i = 2; i < 10; i++)
            {
                PART[i].value = strNumarTypeCommon.Substring(3 * (i - 2) + 4, 3);
                PART[i].name = "###";
                PART[0].description = "###";
            }
        }

        public static void LoadListOfStateTables(string path)
        {
            ListOfStateTables.Clear();

            StreamReader sr = new StreamReader(path);
            string line = sr.ReadLine();

            while (line != null)
            {
                StateTable.ListOfStateTables.Add(new StateTable(line));
                line = sr.ReadLine();
            }
        }

        public void particulariseState(string pathConfigXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathConfigXML);

            XmlNode Node = xmlDoc.DocumentElement.FirstChild;
            while( Node != null )
            {
                if (Node.Name == "State" && Node.Attributes.Count != 0 && Node.Attributes["name"].Value == textTable[3].ToString())
                {
                    UInt16 ext = Convert.ToUInt16(Node.Attributes["extended"].Value);
                    PART = new PartStateTable[10 + ext * 10];

                    XmlNode childrenNode = Node.FirstChild;
                    List<string> StrExtended_Z = new List<string>();
                    int pos = 0;

                    if (textTable.Length < 28)
                        return;
                    // base
                    int i = 0;
                    for(; i < 10; )
                    {
                        if(childrenNode.Name.Contains("Part"))
                        {
                            int len = childrenNode.Attributes["value"].Value.Length;
                            PART[i].value = textTable.Substring(pos, len);
                            pos += len;
                            PART[i].name = childrenNode.Attributes["name"].Value.Replace("\\r\\n", "\r\n");
                            PART[i].description = childrenNode.Attributes["description"].Value.Replace("\\r\\n", "\r\n");
                            PART[i].mask = childrenNode.Attributes["mask"].Value.Replace("AND", "&");
                            if (childrenNode.Attributes["mask"].Value == "STATE_EXTENSION")
                            {
                                // Z
                                if (FindStateTabelsInListAll(PART[i].value) != null && FindStateTabelsInListAll(PART[i].value).textTable[3] == 'Z')
                                    StrExtended_Z.Add(FindStateTabelsInListAll(PART[i].value).textTable);
                                else
                                    StrExtended_Z.Add("NOT EXIST");
                            }
                            if (childrenNode.Attributes["mask"].Value == "STATE_EXTENSION_OR_DATA" && Convert.ToInt16(PART[i].value) > 7)
                            {
                                // Z or DATA
                                if (FindStateTabelsInListAll(PART[i].value) != null && FindStateTabelsInListAll(PART[i].value).textTable[3] == 'Z')
                                    StrExtended_Z.Add(FindStateTabelsInListAll(PART[i].value).textTable);
                            }
                            i++;
                        }
                        childrenNode = childrenNode.NextSibling;
                    }

                    // cazuri particulare
                    if (PART[1].value == "K") // cazul K
                    {
                        StateTable tempST = FindStateTabelsInListAll('_');
                        if (tempST != null)
                        {
                            pos = 0;
                            for (; i < 20; )
                            {
                                if (childrenNode.Name.Contains("Part"))
                                {
                                    int len = childrenNode.Attributes["value"].Value.Length;
                                    PART[i].value = tempST.textTable.Substring(pos, len);
                                    pos += len;
                                    PART[i].name = childrenNode.Attributes["name"].Value.Replace("\\r\\n", "\r\n");
                                    PART[i].description = childrenNode.Attributes["description"].Value.Replace("\\r\\n", "\r\n");
                                    PART[i].mask = childrenNode.Attributes["mask"].Value;
                                    if (childrenNode.Attributes["mask"].Value == "STATE_EXTENSION")
                                    {
                                        // Z
                                        if (FindStateTabelsInListAll(PART[i].value) != null && FindStateTabelsInListAll(PART[i].value).textTable[3] == 'Z')
                                            StrExtended_Z.Add(FindStateTabelsInListAll(PART[i].value).textTable);
                                    }
                                    i++;
                                }
                                childrenNode = childrenNode.NextSibling;
                            }
                        }

                    }

                    // extended Z
                    for(int k = 0; k < StrExtended_Z.Count; k++)
                    {
                        pos = 0;
                        for (int j = 0; j < 10 ;)
                        {
                            if (StrExtended_Z[k] != "NOT EXIST")
                            {
                                int len = childrenNode.Attributes["value"].Value.Length;
                                PART[i+j].value = StrExtended_Z[k].Substring(pos, len);
                                pos += len;
                                PART[i+j].name = childrenNode.Attributes["name"].Value.Replace("\\r\\n", "\r\n");
                                PART[i+j].description = childrenNode.Attributes["description"].Value.Replace("\\r\\n", "\r\n");
                                PART[i+j].mask = childrenNode.Attributes["mask"].Value;
                                
                            }
                            j++;
                            childrenNode = childrenNode.NextSibling;
                        }
                    }                
                                
                }
                Node = Node.NextSibling;
            }
        }

        public static void SaveToXML(string strPath)
        {
            FileInfo f = new FileInfo(strPath);
            StreamWriter writer = f.CreateText();

            writer.WriteLine("<STATES>");
            for (int count = 0; count < ListOfStateTables.Count; count++)
            {
                ListOfStateTables[count].particulariseState(@"C:\Config.xml");
                // <State>
                writer.Write("\t<State number=\"{0}\" type=\"{1}\" ", ListOfStateTables[count].PART[0].value, ListOfStateTables[count].PART[1].value);
                for (int i = 0; i < ListOfStateTables[count].PART.Length && ListOfStateTables[count].PART[i].value != null; i++)
                    writer.Write("part{0}=\"{1}\" ", i + 1, ListOfStateTables[count].PART[i].value);
                writer.WriteLine(">");
                //</State>
                writer.WriteLine("\t</State>");

            }
            writer.WriteLine("</STATES>");
            writer.Close();            
        }

    };

}


