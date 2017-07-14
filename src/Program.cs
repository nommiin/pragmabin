using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace PragmaBin {
    class Program {
        public class scriptNode {
            public string reference = "";
            public string contents = "";
            public scriptNode(string reference, string content) {
                this.reference = reference;
                this.contents = content;
            }
        }

        public static string scriptReturn = null;
        public static string scriptLimit = null;
        public static string scriptJump = null;
        public static bool scriptContinue = true;
        public static BinaryReader scriptBinary;
        public static string scriptLocation = @"D:\PragmaBin\gm.prag";
        public static List<scriptNode> scriptData = new List<scriptNode>();
        public static Dictionary<string, dynamic> scriptReferences = new Dictionary<string, dynamic> {
            { "execTell", 0 }
        };

        public static List<string> scriptCondtions = new List<string> {
            "<",
            ">",
            "<=",
            ">=",
            "!=",
            "=="
        };

        static void Main(string[] args) {
            string[] scriptLines = File.ReadAllLines(scriptLocation);
            // Gather nodes
            for(int i = 0; i < scriptLines.Length; i++) { string scriptLine = scriptLines[i];
                int scriptIndex = scriptMoveTo(scriptLine, ':');
                if (scriptIndex > -1) {
                    string scriptReference = scriptLine.Substring(0, scriptIndex++ - 1).Trim(), scriptContents = scriptLine.Substring(scriptIndex, scriptLine.Length - scriptIndex).Trim(); scriptReference = scriptReference == "" ? i.ToString() : scriptReference;
                    scriptData.Add(new scriptNode(scriptReference, scriptContents));
                }
            }

            // Execute nodes
            for(int i = 0; i < scriptData.Count; i++) {
                if (scriptContinue == true) {
                    // While Loop
                    if (scriptLimit != null) {
                        if (scriptData[i].reference == scriptLimit) {
                            for (int j = 0; j < scriptData.Count; j++) {
                                if (scriptData[j].reference == scriptReturn) {
                                    i = j;
                                    scriptLimit = null;
                                }
                            }
                        }
                    }

                    // Conditional Check
                    if (scriptJump != null) {
                        for(int j = 0; j < scriptData.Count; j++) {
                            if (scriptData[j].reference == scriptJump) {
                                i = j;
                                scriptJump = null;
                            }
                        }
                    }
                    
                    // Execute Node
                    scriptNode scriptNodeRead = scriptData[i];
                    scriptExecute(scriptNodeRead.reference, scriptNodeRead.contents);
                }
            }

            Console.ReadKey();
        }

        public static int scriptMoveTo(string scriptLine, char scriptChar, int scriptIndex = 0) {
            if (scriptLine.Trim().Length == 0) { return -1; }
            while (scriptLine[scriptIndex++] != scriptChar) {
                if (scriptIndex >= scriptLine.Length) {
                    return -1;
                }
            }
            return scriptIndex;
        }

        public static dynamic scriptExecute(string scriptReference, string scriptContents) {
            int scriptIndex = scriptMoveTo(scriptContents, '(') - 1;
            if (scriptIndex > -1) {
                string scriptFunction = scriptContents.Substring(0, scriptIndex);
                string[] scriptArguments = scriptContents.Substring(scriptIndex + 1, (scriptMoveTo(scriptContents, ')') - scriptIndex) - 2).Split(',').Select(i => i.Trim()).ToArray();
                scriptConstants();
                scriptReferences[scriptReference] = scriptExecuteFunction(scriptReference, scriptFunction, scriptArguments);
            } else {
                scriptReferences[scriptReference] = scriptContents;
            }
            return scriptReferences[scriptReference];
        }

        public static dynamic scriptExecuteFunction(string scriptReference, string scriptFunction, string[] scriptArguments) {
            switch (scriptFunction) {
                // Comparing
                case "doCompare":
                    if (scriptCondtional(scriptArguments[0]) == false) {
                        scriptJump = scriptArguments[1];
                    }
                return false;

                case "doWhile":
                    if (scriptCondtional(scriptArguments[0]) == true) {
                        scriptReturn = scriptReference;
                        scriptLimit = scriptArguments[1];
                    } else {
                        scriptJump = scriptArguments[1];
                    }
                return false;

                // Reading
                case "readByte": return scriptBinary.ReadByte();
                case "readUInt16": return scriptBinary.ReadUInt16();
                case "readUInt32": return scriptBinary.ReadUInt32();
                case "readUInt64": return scriptBinary.ReadUInt64();
                case "readSByte": return scriptBinary.ReadSByte();
                case "readInt16": return scriptBinary.ReadInt16();
                case "readInt32": return scriptBinary.ReadInt32();
                case "readInt64": return scriptBinary.ReadInt64();
                case "readChar": return Encoding.ASCII.GetString(scriptBinary.ReadBytes(1));
                case "readString": return Encoding.ASCII.GetString(scriptBinary.ReadBytes(Convert.ToByte(scriptArguments[0])));
                case "readBinary":
                    if (File.Exists(scriptArguments[0]) == true) {
                        scriptBinary = new BinaryReader(File.Open(scriptArguments[0], FileMode.Open));
                        scriptReferences["execSize"] = scriptBinary.BaseStream.Length;
                    } else {
                        Console.WriteLine("Error: [{0}]", scriptArguments[0] + " does not exist.");
                        scriptContinue = false;
                    }
                return true;

                // Execution
                case "execPrint": Console.WriteLine("Message: [{0}]", scriptReferences[scriptArguments[0]]); return -1;
                case "execMessage": return MessageBox.Show(string.Join("\n", scriptArguments));

                case "execSeek":
                    if (scriptArguments.Length > 1) {
                        SeekOrigin scriptSeek = SeekOrigin.Current;
                        switch (scriptArguments[1]) {
                            case "BEGIN": scriptSeek = SeekOrigin.Begin; break;
                            case "END": scriptSeek = SeekOrigin.End; break;
                        }
                        scriptBinary.BaseStream.Seek(Convert.ToInt64(scriptReferences[scriptArguments[0]]), scriptSeek);
                    } else {
                        scriptBinary.BaseStream.Seek(Convert.ToInt64(scriptReferences[scriptArguments[0]]), SeekOrigin.Current);
                    }
                return scriptBinary.BaseStream.Position;

                case "execKill":
                    Console.WriteLine("Execution Complete: [{0}]", string.Join("\n", scriptArguments));
                    scriptContinue = false;
                return -1;

                // Unknown Function
                default: Console.WriteLine("Warning: [Unknown Function: {0} ({1})]", scriptFunction, String.Join(", ", scriptArguments)); return -1;
            }
        }

        public static bool scriptCondtional(string scriptContents) {
            for(int i = 0; i < scriptContents.Length; i++) {
                for(int j = 0; j < scriptCondtions.Count; j++) {
                    string scriptCondition = scriptCondtions[j];
                    if (scriptContents.Substring(i, scriptCondition.Length) == scriptCondition) {
                        string scriptLeft = scriptContents.Substring(0, i).Trim(), scriptRight = scriptContents.Substring(i + scriptCondition.Length).Trim();
                        switch (j) {
                            case 0: return scriptReferences[scriptLeft] < scriptReferences[scriptRight];
                            case 1: return scriptReferences[scriptLeft] > scriptReferences[scriptRight];
                            case 2: return scriptReferences[scriptLeft] <= scriptReferences[scriptRight];
                            case 3: return scriptReferences[scriptLeft] >= scriptReferences[scriptRight];
                            case 4: return scriptReferences[scriptLeft] != scriptReferences[scriptRight];
                            case 5: return scriptReferences[scriptLeft] == scriptReferences[scriptRight];
                        }
                    }
                }
            }
            return false;
        }

        public static void scriptConstants() {
            if (scriptBinary != null) {
                scriptReferences["execTell"] = scriptBinary.BaseStream.Position;
            }
        }
    }
}
