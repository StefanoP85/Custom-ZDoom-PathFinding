"""
- Author: Pollazzon Stefano (ported)
- Project: ZDoom Navmesh builder
- This module contains a very simple text lexer and
- a UDMF parser, that is used in reading text maps
"""

from Structures import (
    TMapVertex,
    TMapLinedef,
    TMapSidedef,
    TMapSector,
    TMapThing,
    TMapNamespace,
)


class TScanner:
    """
    Simple text scanner that maintains position, row, and column.
    """

    def __init__(self, TextMap):
        if TextMap is None:
            raise ValueError("TextMap cannot be None")
        self.TextMap = TextMap
        self._column = 1
        self._row = 1
        self._position = 1  # 1-based, like the C# code

    @property
    def Column(self):
        return self._column

    @property
    def Row(self):
        return self._row

    @property
    def Finished(self):
        return self._position > len(self.TextMap)

    def GetNextChar(self):
        if self._position <= len(self.TextMap):
            result = self.TextMap[self._position - 1]
            self._position += 1
            if result == "\n":
                self._column = 1
                self._row += 1
            else:
                self._column += 1
            return result
        return "\0"

    def Peek(self, Offset=0):
        pos = self._position + Offset
        if 1 <= pos <= len(self.TextMap):
            return self.TextMap[pos - 1]
        return "\0"


class TUdmfLexer:
    """
    Simple lexer for UDMF text.
    """

    def __init__(self, TextMap):
        if TextMap is None:
            raise ValueError("TextMap cannot be null")
        self.Scanner = TScanner(TextMap)

    @property
    def Column(self):
        return self.Scanner.Column

    @property
    def Row(self):
        return self.Scanner.Row

    def GetNextChar(self):
        return "\0" if self.Scanner.Finished else self.Scanner.GetNextChar()

    def IsSymbol(self, C):
        # More robust than the original while fully compatible with your UDMF files
        return C in ";{}="

    def IsWhitespace(self, C):
        return C in (" ", "\t", "\n", "\r")

    def IsTokenStop(self, C):
        return self.IsSymbol(C) or self.IsWhitespace(C)

    def SkipWhitespaces(self):
        while True:
            c = self.Scanner.Peek(0)

            # normal whitespace
            if self.IsWhitespace(c):
                if self.GetNextChar() == "\0":
                    break
                continue

            # single-line comments // ...
            if c == "/" and self.Scanner.Peek(1) == "/":
                self.GetNextChar()
                self.GetNextChar()
                while True:
                    c = self.GetNextChar()
                    if c in ("\0", "\n"):
                        break
                continue

            # block comments /* ... */
            if c == "/" and self.Scanner.Peek(1) == "*":
                self.GetNextChar()
                self.GetNextChar()
                while True:
                    c = self.GetNextChar()
                    if c == "\0":
                        break
                    if c == "*" and self.Scanner.Peek(0) == "/":
                        self.GetNextChar()
                        break
                continue

            break

    def GetToken(self):
        self.SkipWhitespaces()

        if self.Scanner.Finished:
            return ""

        c = self.Scanner.Peek(0)

        # single-symbol token
        if self.IsSymbol(c):
            self.GetNextChar()
            self.SkipWhitespaces()
            return c

        result = []
        open_quote1 = False
        open_quote2 = False

        while True:
            c = self.GetNextChar()
            if c == "\0":
                break

            if not open_quote1 and not open_quote2 and c == "/" and self.Scanner.Peek(0) == "/":
                # consume second slash
                self.GetNextChar()
                while True:
                    c = self.GetNextChar()
                    if c in ("\0", "\n"):
                        break
                break

            if not open_quote1 and not open_quote2 and c == "/" and self.Scanner.Peek(0) == "*":
                self.GetNextChar()
                while True:
                    c = self.GetNextChar()
                    if c == "\0":
                        break
                    if c == "*" and self.Scanner.Peek(0) == "/":
                        self.GetNextChar()
                        break
                break

            if c == "'" and not open_quote2:
                open_quote1 = not open_quote1
                continue

            if c == '"' and not open_quote1:
                open_quote2 = not open_quote2
                continue

            if c == "\\" and (open_quote1 or open_quote2):
                c = self.GetNextChar()
                if c == "\0":
                    break
                result.append(c)
                continue

            if (not open_quote1 and not open_quote2):
                if self.IsSymbol(c):
                    # put back symbol by rewinding one character
                    self.Scanner._position -= 1
                    if self.Scanner._column > 1:
                        self.Scanner._column -= 1
                    break
                if self.IsWhitespace(c):
                    break

            result.append(c)

        self.SkipWhitespaces()
        return "".join(result)


class EUdmfException(Exception):
    pass


class TUdmfParser:
    def __init__(self, TextMap):
        if TextMap is None:
            raise ValueError("TextMap cannot be null")

        self.UdmfLexer = TUdmfLexer(TextMap)

        token = self.UdmfLexer.GetToken()
        if token.lower() == "namespace":
            text_namespace = self.GetValueToken()
            if text_namespace.lower() == "doom":
                self.MapNamespace = TMapNamespace.MapNamespaceDoom
            elif text_namespace.lower() == "hexen":
                self.MapNamespace = TMapNamespace.MapNamespaceHexen
            elif text_namespace.lower() == "zdoom":
                self.MapNamespace = TMapNamespace.MapNamespaceZDoom
            else:
                raise EUdmfException(
                    f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: invalid namespace '{text_namespace}'"
                )
        else:
            raise EUdmfException(
                f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected namespace, found '{token}'"
            )

    # --------------------------------------------------
    # helpers
    # --------------------------------------------------

    def Expects(self, Value):
        token = self.UdmfLexer.GetToken()
        return token.lower() == Value.lower()

    def GetValueToken(self):
        if self.Expects("="):
            result = self.UdmfLexer.GetToken()
            if not self.Expects(";"):
                raise EUdmfException(
                    f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected ';'"
                )
            return result
        raise EUdmfException(
            f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '='"
        )

    def GetBooleanToken(self):
        token = self.GetValueToken().strip().lower()
        if token == "true":
            return True
        if token == "false":
            return False
        raise EUdmfException(
            f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: invalid boolean '{token}'"
        )

    def GetInt16Token(self):
        token = self.GetValueToken()
        return int(round(float(token)))

    def GetInt32Token(self):
        token = self.GetValueToken()
        return int(round(float(token)))

    # --------------------------------------------------
    # entity readers
    # --------------------------------------------------

    def GetMapVertex(self):
        x = 0
        y = 0
        valid_x = False
        valid_y = False

        if not self.Expects("{"):
            raise EUdmfException(
                f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '{{'"
            )

        token = self.UdmfLexer.GetToken()
        while token != "}":
            if token.lower() == "x":
                x = self.GetInt32Token()
                valid_x = True
            elif token.lower() == "y":
                y = self.GetInt32Token()
                valid_y = True
            else:
                # ignore unsupported fields
                if self.UdmfLexer.Scanner.Peek(0) == "=":
                    self.GetValueToken()
            token = self.UdmfLexer.GetToken()

        if valid_x and valid_y:
            return TMapVertex(x, y)
        return None

    def GetMapLinedef(self):
        v1 = 0
        v2 = 0
        valid_v1 = False
        valid_v2 = False

        line_id = 0
        blocking = False
        block_monsters = False
        two_sided = False
        side_front = 0
        side_back = 0
        special = 0
        arg0 = 0
        arg1 = 0
        arg2 = 0
        arg3 = 0
        arg4 = 0
        monster_use = False
        repeatable_special = False
        ignored = False

        if not self.Expects("{"):
            raise EUdmfException(
                f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '{{'"
            )

        token = self.UdmfLexer.GetToken()
        while token != "}":
            t = token.lower()

            if t == "v1":
                v1 = self.GetInt32Token()
                valid_v1 = True
            elif t == "v2":
                v2 = self.GetInt32Token()
                valid_v2 = True
            elif t == "id":
                line_id = self.GetInt16Token()
            elif t == "blocking":
                blocking = self.GetBooleanToken()
            elif t == "blockmonsters":
                block_monsters = self.GetBooleanToken()
            elif t == "twosided":
                two_sided = self.GetBooleanToken()
            elif t == "sidefront":
                side_front = self.GetInt32Token()
            elif t == "sideback":
                side_back = self.GetInt32Token()
            elif t == "special":
                special = self.GetInt16Token()
            elif t == "arg0":
                arg0 = self.GetInt16Token()
            elif t == "arg1":
                arg1 = self.GetInt16Token()
            elif t == "arg2":
                arg2 = self.GetInt16Token()
            elif t == "arg3":
                arg3 = self.GetInt16Token()
            elif t == "arg4":
                arg4 = self.GetInt16Token()
            elif t == "monsteruse":
                monster_use = self.GetBooleanToken()
            elif t == "repeatablespecial":
                repeatable_special = self.GetBooleanToken()
            elif t in ("user_norecast", "user_nocast"):
                ignored = self.GetBooleanToken()
            else:
                if self.UdmfLexer.Scanner.Peek(0) == "=":
                    self.GetValueToken()

            token = self.UdmfLexer.GetToken()

        if valid_v1 and valid_v2:
            return TMapLinedef(
                v1,
                v2,
                line_id,
                blocking or block_monsters,
                two_sided,
                side_front,
                side_back,
                special,
                arg0,
                arg1,
                arg2,
                arg3,
                arg4,
                monster_use,
                repeatable_special,
                ignored,
            )
        return None

    def GetMapSidedef(self):
        sector = 0
        valid_sector = False

        if not self.Expects("{"):
            raise EUdmfException(
                f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '{{'"
            )

        token = self.UdmfLexer.GetToken()
        while token != "}":
            if token.lower() == "sector":
                sector = self.GetInt32Token()
                valid_sector = True
            else:
                if self.UdmfLexer.Scanner.Peek(0) == "=":
                    self.GetValueToken()
            token = self.UdmfLexer.GetToken()

        if valid_sector:
            return TMapSidedef(sector)
        return None

    def GetMapSector(self):
        height_floor = 0
        height_ceiling = 0
        sector_id = 0
        special = 0
        ignored = False

        if not self.Expects("{"):
            raise EUdmfException(
                f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '{{'"
            )

        token = self.UdmfLexer.GetToken()
        while token != "}":
            t = token.lower()

            if t == "heightfloor":
                height_floor = self.GetInt32Token()
            elif t == "heightceiling":
                height_ceiling = self.GetInt32Token()
            elif t == "id":
                sector_id = self.GetInt32Token()
            elif t == "special":
                special = self.GetInt16Token()
            elif t in ("user_norecast", "user_nocast"):
                ignored = self.GetBooleanToken()
            else:
                if self.UdmfLexer.Scanner.Peek(0) == "=":
                    self.GetValueToken()

            token = self.UdmfLexer.GetToken()

        return TMapSector(height_floor, height_ceiling, sector_id, special, ignored)

    def GetMapThing(self):
        x = 0
        y = 0
        z = 0
        thing_type = 0
        valid_x = False
        valid_y = False
        valid_thing_type = False

        thing_id = 0
        special = 0
        arg0 = 0
        arg1 = 0
        arg2 = 0
        arg3 = 0
        arg4 = 0

        if not self.Expects("{"):
            raise EUdmfException(
                f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '{{'"
            )

        token = self.UdmfLexer.GetToken()
        while token != "}":
            t = token.lower()

            if t == "x":
                x = self.GetInt32Token()
                valid_x = True
            elif t == "y":
                y = self.GetInt32Token()
                valid_y = True
            elif t == "type":
                thing_type = self.GetInt32Token()
                valid_thing_type = True
            elif t == "z":
                z = self.GetInt32Token()
            elif t == "id":
                thing_id = self.GetInt32Token()
            elif t == "special":
                special = self.GetInt16Token()
            elif t == "arg0":
                arg0 = self.GetInt16Token()
            elif t == "arg1":
                arg1 = self.GetInt16Token()
            elif t == "arg2":
                arg2 = self.GetInt16Token()
            elif t == "arg3":
                arg3 = self.GetInt16Token()
            elif t == "arg4":
                arg4 = self.GetInt16Token()
            else:
                if self.UdmfLexer.Scanner.Peek(0) == "=":
                    self.GetValueToken()

            token = self.UdmfLexer.GetToken()

        if valid_x and valid_y and valid_thing_type:
            return TMapThing(x, y, z, thing_type, thing_id, special, arg0, arg1, arg2, arg3, arg4)
        return None

    # --------------------------------------------------
    # whole map
    # --------------------------------------------------

    def ParseMap(self, MapDefinition):
        if MapDefinition is None:
            raise ValueError("MapDefinition cannot be None")

        token = self.UdmfLexer.GetToken()
        while token:
            t = token.lower()

            if t == "vertex":
                MapDefinition.AddMapVertex(self.GetMapVertex())
            elif t == "linedef":
                MapDefinition.AddMapLinedef(self.GetMapLinedef())
            elif t == "sidedef":
                MapDefinition.AddMapSidedef(self.GetMapSidedef())
            elif t == "sector":
                MapDefinition.AddMapSector(self.GetMapSector())
            elif t == "thing":
                MapDefinition.AddMapThing(self.GetMapThing())
            else:
                raise EUdmfException(
                    f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: unknown UDMF entity '{token}'"
                )

            token = self.UdmfLexer.GetToken()
