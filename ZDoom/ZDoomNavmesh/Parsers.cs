/*
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains a very simple text lexer and
 * a UDMF parser, that is used in reading text maps
 */
using System;
using ZDoomNavmesh;

namespace NavmeshBuilder
{
    /// <summary>
    /// Class <c>TScanner</c> is a simple text scanner, that maintains a position, row and column of the processing chars.
    /// </summary>
    internal sealed class TScanner : Object
    {
        private readonly string TextMap;
        private int FColumn;
        private int FRow;
        private int Position;
        public int Column { get => FColumn; }
        public int Row { get => FRow; }
        public bool Finished { get => Position >= TextMap.Length; }
        public TScanner(string ATextMap)
        {
            TextMap = ATextMap ?? throw new ArgumentNullException(nameof(ATextMap));
            FColumn = 1;
            FRow = 1;
            Position = 1;
        }
        public char GetNextChar()
        {
            char Result;
            if (Position <= TextMap.Length)
            {
                Result = TextMap[Position - 1];
                Position += 1;
                if (Result == '\n')
                {
                    FColumn = 1;
                    FRow += 1;
                }
                else
                    FColumn += 1;
            }
            else
                Result = '\0';
            return Result;
        }
        public char Peek(int Offset = 0)
        {
            char Result;
            if ((Position + Offset >= 1) && (Position + Offset <= TextMap.Length))
            {
                Result = TextMap[Position + Offset - 1];
                if (Result == '\n')
                {
                    FColumn = 1;
                    FRow += 1;
                }
                else
                    FColumn += 1;
            }
            else
                Result = '\0';
            return Result;
        }
    }
    /// <summary>
    /// Class <c>TUdmfLexer</c> is a simple text lexer of the UDMF grammar.
    /// </summary>
    public sealed class TUdmfLexer : Object
    {
        private TScanner Scanner;
        public int Column { get => Scanner.Column; }
        public int Row { get => Scanner.Row; }
        public TUdmfLexer(string TextMap)
        {
            if (TextMap == null)
                throw new ArgumentNullException(nameof(TextMap));
            Scanner = new TScanner(TextMap);
        }
        private char GetNextChar()
        {
            if (Scanner.Finished)
                return '\0';
            else
                return Scanner.GetNextChar();
        }
        private bool IsSymbol(char C)
        {
            return C == ';';
        }
        private bool IsWhitespace(char C)
        {
            return (C == ' ') || (C == '\t') || (C == '\n') || (C == '\r');
        }
        private bool IsTokenStop(char C)
        {
            return (IsSymbol(C)) || (IsWhitespace(C));
        }
        private void SkipWhitespaces()
        {
            while (IsWhitespace(Scanner.Peek(0)))
                if (GetNextChar() == '\0')
                    break;
        }
        /// <summary>
        /// Function <c>GetToken</c> gives the next token in the source code.
        /// </summary>
        /// <returns>
        /// A string representing the next token in the source code, without leading or trailing whitespaces.
        /// </returns>
        public string GetToken()
        {
            bool OpenQuote1 = false;
            bool OpenQuote2 = false;
            if (Scanner.Finished)
                return null;
            string Result = "";
            do
            {
                char C = GetNextChar();
                if ((C == '/') && (!OpenQuote1) && (!OpenQuote2))
                {
                    C = GetNextChar();
                    if (C == '/')
                    {
                        do
                        {
                            C = GetNextChar();
                        } while (C != '\n');
                        SkipWhitespaces();
                    }
                    else
                        if (C == '*')
                    {
                        do
                        {
                            C = GetNextChar();
                        } while ((C != '*') || (Scanner.Peek() != '/'));
                        C = GetNextChar();
                        SkipWhitespaces();
                    }
                }
                else
                    if ((C == '\'') && (!OpenQuote1))
                    OpenQuote1 = false;
                else
                        if ((C == '"') && (!OpenQuote2))
                    OpenQuote2 = false;
                else
                            if ((C == '\\') && ((OpenQuote1) || (OpenQuote2)))
                    C = GetNextChar();
                else
                    Result += C;
                if (Scanner.Finished)
                    break;
            } while ((!IsTokenStop(Scanner.Peek(0))) || (OpenQuote1) || (OpenQuote2));
            SkipWhitespaces();
            return Result;
        }
    }
    /// <summary>
    /// Class <c>EUdmfException</c> is an Exception thrown, when there are problems in parsing the source code.
    /// </summary>
    public class EUdmfException : FormatException
    {
        public EUdmfException()
        {
        }
        public EUdmfException(string Message) : base(Message)
        {
        }
        public EUdmfException(string Message, Exception InnerException) : base(Message, InnerException)
        {
        }
    }
    /// <summary>
    /// Class <c>TUdmfParser</c> is a simple UDMF parser.
    /// </summary>
    public sealed class TUdmfParser : Object
    {
        private TUdmfLexer UdmfLexer;
        private TMapNamespace FMapNamespace;
        public TMapNamespace MapNamespace { get => FMapNamespace; }
        private bool Expects(string Value)
        {
            string Token = UdmfLexer.GetToken();
            return string.Equals(Token, Value, StringComparison.OrdinalIgnoreCase);
        }
        private string GetValueToken()
        {
            string Result;
            if (Expects("="))
            {
                Result = UdmfLexer.GetToken();
                if (!Expects(";"))
                    throw new EUdmfException(string.Format("Row %n, column %n: expected ';'", UdmfLexer.Row, UdmfLexer.Column));
            }
            else
                throw new EUdmfException(string.Format("Row %n, column %n: expected '='", UdmfLexer.Row, UdmfLexer.Column));
            return Result;
        }
        private bool GetBooleanToken()
        {
            string Token = GetValueToken();
            bool Result = Boolean.Parse(Token);
            return Result;
        }
        private short GetInt16Token()
        {
            string Token = GetValueToken();
            short Result = Convert.ToInt16(Double.Parse(Token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
            return Result;
        }
        private int GetInt32Token()
        {
            string Token = GetValueToken();
            int Result = Convert.ToInt32(Double.Parse(Token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
            return Result;
        }
        private TMapVertex GetMapVertex()
        {
            int X = 0;
            int Y = 0;
            bool ValidX = false;
            bool ValidY = false;
            string Token;
            Expects("{");
            do
            {
                Token = UdmfLexer.GetToken();
                if (string.Equals(Token, "X", StringComparison.OrdinalIgnoreCase))
                {
                    X = GetInt32Token();
                    ValidX = true;
                }
                if (string.Equals(Token, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Y = GetInt32Token();
                    ValidY = true;
                }
            } while (Token != "}");
            if ((ValidX) && (ValidY))
                return new TMapVertex
                {
                    X = X,
                    Y = Y
                };
            else
                return null;
        }
        private TMapLinedef GetMapLinedef()
        {
            int V1 = 0;
            int V2 = 0;
            bool ValidV1 = false;
            bool ValidV2 = false;
            int ID = 0;
            bool Blocking = false;
            bool BlockMonsters = false;
            bool TwoSided = false;
            int SideFront = 0;
            int SideBack = 0;
            short Special = 0;
            short Arg0 = 0;
            short Arg1 = 0;
            short Arg2 = 0;
            short Arg3 = 0;
            short Arg4 = 0;
            bool MonsterUse = false;
            bool RepeatableSpecial = false;
            bool Ignored = false;
            string Token;
            Expects("{");
            do
            {
                Token = UdmfLexer.GetToken();
                if (string.Equals(Token, "V1", StringComparison.OrdinalIgnoreCase))
                {
                    V1 = GetInt32Token();
                    ValidV1 = true;
                }
                if (string.Equals(Token, "V2", StringComparison.OrdinalIgnoreCase))
                {
                    V2 = GetInt32Token();
                    ValidV2 = true;
                }
                if (string.Equals(Token, "ID", StringComparison.OrdinalIgnoreCase))
                    ID = GetInt16Token();
                if (string.Equals(Token, "Blocking", StringComparison.OrdinalIgnoreCase))
                    Blocking = GetBooleanToken();
                if (string.Equals(Token, "BlockMonsters", StringComparison.OrdinalIgnoreCase))
                    BlockMonsters = GetBooleanToken();
                if (string.Equals(Token, "TwoSided", StringComparison.OrdinalIgnoreCase))
                    TwoSided = GetBooleanToken();
                if (string.Equals(Token, "SideFront", StringComparison.OrdinalIgnoreCase))
                    SideFront = GetInt32Token();
                if (string.Equals(Token, "SideBack", StringComparison.OrdinalIgnoreCase))
                    SideBack = GetInt32Token();
                if (string.Equals(Token, "Special", StringComparison.OrdinalIgnoreCase))
                    Special = GetInt16Token();
                if (string.Equals(Token, "Arg0", StringComparison.OrdinalIgnoreCase))
                    Arg0 = GetInt16Token();
                if (string.Equals(Token, "Arg1", StringComparison.OrdinalIgnoreCase))
                    Arg1 = GetInt16Token();
                if (string.Equals(Token, "Arg2", StringComparison.OrdinalIgnoreCase))
                    Arg2 = GetInt16Token();
                if (string.Equals(Token, "Arg3", StringComparison.OrdinalIgnoreCase))
                    Arg3 = GetInt16Token();
                if (string.Equals(Token, "Arg4", StringComparison.OrdinalIgnoreCase))
                    Arg4 = GetInt16Token();
                if (string.Equals(Token, "MonsterUse", StringComparison.OrdinalIgnoreCase))
                    MonsterUse = GetBooleanToken();
                if (string.Equals(Token, "RepeatableSpecial", StringComparison.OrdinalIgnoreCase))
                    RepeatableSpecial = GetBooleanToken();
                if (string.Equals(Token, "user_norecast", StringComparison.OrdinalIgnoreCase))
                    Ignored = GetBooleanToken();
            } while (Token != "}");
            if ((ValidV1) && (ValidV2))
                return new TMapLinedef
                {
                    V1 = V1,
                    V2 = V2,
                    ID = ID,
                    Blocking = Blocking || BlockMonsters,
                    TwoSided = TwoSided,
                    SideFront = SideFront,
                    SideBack = SideBack,
                    Special = Special,
                    Arg0 = Arg0,
                    Arg1 = Arg1,
                    Arg2 = Arg2,
                    Arg3 = Arg3,
                    Arg4 = Arg4,
                    MonsterUse = MonsterUse,
                    RepeatableSpecial = RepeatableSpecial,
                    Ignored = Ignored
                };
            else
                return null;
        }
        private TMapSidedef GetMapSidedef()
        {
            int Sector = 0;
            bool ValidSector = false;
            string Token;
            Expects("{");
            do
            {
                Token = UdmfLexer.GetToken();
                if (string.Equals(Token, "Sector", StringComparison.OrdinalIgnoreCase))
                {
                    Sector = GetInt32Token();
                    ValidSector = true;
                }
            } while (Token != "}");
            if (ValidSector)
                return new TMapSidedef
                {
                    Sector = Sector
                };
            else
                return null;
        }
        private TMapSector GetMapSector()
        {
            int HeightFloor = 0;
            int HeightCeiling = 0;
            int ID = 0;
            short Special = 0;
            bool Ignored = false;
            string Token;
            Expects("{");
            do
            {
                Token = UdmfLexer.GetToken();
                if (string.Equals(Token, "HeightFloor", StringComparison.OrdinalIgnoreCase))
                    HeightFloor = GetInt32Token();
                if (string.Equals(Token, "HeightCeiling", StringComparison.OrdinalIgnoreCase))
                    HeightCeiling = GetInt32Token();
                if (string.Equals(Token, "ID", StringComparison.OrdinalIgnoreCase))
                    ID = GetInt32Token();
                if (string.Equals(Token, "Special", StringComparison.OrdinalIgnoreCase))
                    Special = GetInt16Token();
                if (string.Equals(Token, "user_norecast", StringComparison.OrdinalIgnoreCase))
                    Ignored = GetBooleanToken();
            } while (Token != "}");
            return new TMapSector
            {
                HeightFloor = HeightFloor,
                HeightCeiling = HeightCeiling,
                ID = ID,
                Special = Special,
                Ignored = Ignored
            };
        }
        private TMapThing GetMapThing()
        {
            int X = 0;
            int Y = 0;
            int Z = 0;
            bool ValidX = false;
            bool ValidY = false;
            int ID = 0;
            short Special = 0;
            short Arg0 = 0;
            short Arg1 = 0;
            short Arg2 = 0;
            short Arg3 = 0;
            short Arg4 = 0;
            string Token;
            Expects("{");
            do
            {
                Token = UdmfLexer.GetToken();
                if (string.Equals(Token, "X", StringComparison.OrdinalIgnoreCase))
                {
                    X = GetInt32Token();
                    ValidX = true;
                }
                if (string.Equals(Token, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    X = GetInt32Token();
                    ValidY = true;
                }
                if (string.Equals(Token, "Z", StringComparison.OrdinalIgnoreCase))
                    Z = GetInt32Token();
                if (string.Equals(Token, "ID", StringComparison.OrdinalIgnoreCase))
                    ID = GetInt32Token();
                if (string.Equals(Token, "Special", StringComparison.OrdinalIgnoreCase))
                    Special = GetInt16Token();
                if (string.Equals(Token, "Arg0", StringComparison.OrdinalIgnoreCase))
                    Arg0 = GetInt16Token();
                if (string.Equals(Token, "Arg1", StringComparison.OrdinalIgnoreCase))
                    Arg1 = GetInt16Token();
                if (string.Equals(Token, "Arg2", StringComparison.OrdinalIgnoreCase))
                    Arg2 = GetInt16Token();
                if (string.Equals(Token, "Arg3", StringComparison.OrdinalIgnoreCase))
                    Arg3 = GetInt16Token();
                if (string.Equals(Token, "Arg4", StringComparison.OrdinalIgnoreCase))
                    Arg4 = GetInt16Token();

            } while (Token != "}");
            if ((ValidX) && (ValidY))
                return new TMapThing
                {
                    X = X,
                    Y = Y,
                    Z = Z,
                    ID = ID,
                    Special = Special,
                    Arg0 = Arg0,
                    Arg1 = Arg1,
                    Arg2 = Arg2,
                    Arg3 = Arg3,
                    Arg4 = Arg4
                };
            else
                return null;
        }
        public TUdmfParser(string TextMap)
        {
            if (TextMap is null)
                throw new ArgumentNullException(nameof(TextMap));
            UdmfLexer = new TUdmfLexer(TextMap);
            string Token = UdmfLexer.GetToken();
            if (string.Equals(Token, "NAMESPACE", StringComparison.OrdinalIgnoreCase))
            {
                string TextNamespace = GetValueToken();
                if (string.Equals(TextNamespace, "DOOM", StringComparison.OrdinalIgnoreCase))
                    FMapNamespace = TMapNamespace.MapNamespaceDoom;
                else
                    if (string.Equals(TextNamespace, "HEXEN", StringComparison.OrdinalIgnoreCase))
                        FMapNamespace = TMapNamespace.MapNamespaceHexen;
                    else
                        if (string.Equals(TextNamespace, "ZDOOM", StringComparison.OrdinalIgnoreCase))
                            FMapNamespace = TMapNamespace.MapNamespaceZDoom;
                        else
                            throw new EUdmfException(string.Format("Row %n, column %n: invalid namespace '%s'", UdmfLexer.Row, UdmfLexer.Column, Token));
            }
            else
                throw new EUdmfException(string.Format("Row %n, column %n: expected namespace, found '%s'", UdmfLexer.Row, UdmfLexer.Column, Token));
        }
        /// <summary>
        /// Function <c>ParseMap</c> gives the next token in the source code.
        /// </summary>
        /// <param name="MapDefinition"><c>MapDefinition</c> is the map, to fill with the TEXTMAP source code.</param>
        public void ParseMap(TMapDefinition MapDefinition)
        {
            if (MapDefinition == null)
                throw new ArgumentNullException(nameof(MapDefinition));
            string Token = UdmfLexer.GetToken();
            while (!string.IsNullOrEmpty(Token))
            {
                if (string.Equals(Token, "VERTEX", StringComparison.OrdinalIgnoreCase))
                    MapDefinition.AddMapVertex(GetMapVertex());
                else
                    if (string.Equals(Token, "LINEDEF", StringComparison.OrdinalIgnoreCase))
                        MapDefinition.AddMapLinedef(GetMapLinedef());
                    else
                        if (string.Equals(Token, "SIDEDEF", StringComparison.OrdinalIgnoreCase))
                            MapDefinition.AddMapSidedef(GetMapSidedef());
                        else
                            if (string.Equals(Token, "SECTOR", StringComparison.OrdinalIgnoreCase))
                                MapDefinition.AddMapSector(GetMapSector());
                            else
                                if (string.Equals(Token, "THING", StringComparison.OrdinalIgnoreCase))
                                    MapDefinition.AddMapThing(GetMapThing());
                                else
                                    throw new EUdmfException(string.Format("Row %n, column %n: unknown UDMF entity '%s'", UdmfLexer.Row, UdmfLexer.Column, Token));
                Token = UdmfLexer.GetToken();
            }
        }
    }
}
