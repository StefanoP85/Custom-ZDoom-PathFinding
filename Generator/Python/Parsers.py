"""
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains a very simple text lexer and
 * a UDMF parser, that is used in reading text maps
"""
from Structures import *

class TScanner:
	def __init__(self, TextMap):
		if TextMap is None:
			raise ValueError("TextMap cannot be None")
		self.TextMap = TextMap
		self.Column = 1
		self.Row = 1
		self.Position = 1

	def Finished(self):
		return self.Position >= len(self.TextMap)

	def GetNextChar(self):
		if self.Position <= len(self.TextMap):
			Result = self.TextMap[self.Position - 1]
			self.Position += 1
			if Result == '\n':
				self.Column = 1
				self.Row += 1
			else:
				self.Column += 1
		else:
			Result = '\0'
		return Result

	def Peek(self, Offset = 0):
		if (self.Position + Offset >= 1) and (self.Position + Offset <= len(self.TextMap)):
			Result = self.TextMap[self.Position + Offset - 1]
			if Result == '\n':
				self.Column = 1
				self.Row += 1
			else:
				self.Column += 1
		else:
			Result = '\0'
		return Result

# Class TUdmfLexer is a simple text lexer of the UDMF grammar.
class TUdmfLexer:
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
		return '\0' if self.Scanner.Finished() else self.Scanner.GetNextChar()

	def IsSymbol(self, C):
		return C == ';'

	def IsWhitespace(self, C):
		return C in [' ', '\t', '\n', '\r']

	def IsTokenStop(self, C):
		return self.IsSymbol(C) or self.IsWhitespace(C)

	def SkipWhitespaces(self):
		while self.IsWhitespace(self.Scanner.Peek(0)):
			if self.GetNextChar() == '\0':
				break

	def GetToken(self):
		OpenQuote1 = False
		OpenQuote2 = False
		if self.Scanner.Finished():
			return ""
		Result = ""
		while True:
			C = self.GetNextChar()
			if C == '/' and not OpenQuote1 and not OpenQuote2:
				C = self.GetNextChar()
				if C == '/':
					while C != '\n':
						C = self.GetNextChar()
					self.SkipWhitespaces()
				elif C == '*':
					while not (C == '*' and self.Scanner.peek() == '/'):
						C = self.GetNextChar()
					C = self.GetNextChar()
					self.SkipWhitespaces()
			elif C == '\'' and not OpenQuote1:
				OpenQuote1 = False
			elif C == '\"' and not OpenQuote2:
				OpenQuote2 = False
			elif C == '\\' and (OpenQuote1 or OpenQuote2):
				C = self.GetNextChar()
			else:
				Result += C
			if self.Scanner.Finished():
				break
			if self.IsTokenStop(self.Scanner.Peek(0)) and not (OpenQuote1 or OpenQuote2):
				break
		self.SkipWhitespaces()
		return Result

# Class EUdmfException is an Exception thrown, when there are problems in parsing the source code.
class EUdmfException(Exception):
	def __init__(self, message):
		super().__init__(message)

# Class TUdmfParser is a simple UDMF parser.
class TUdmfParser:
	def __init__(self, TextMap):
		if TextMap is None:
			raise ValueError("TextMap cannot be null")
		self.UdmfLexer = TUdmfLexer(TextMap)
		Token = self.UdmfLexer.GetToken()
		if Token.lower() == "namespace":
			TextNamespace = self.GetValueToken()
			if TextNamespace.lower() == "doom":
				self.MapNamespace = TMapNamespace.MapNamespaceDoom
			elif TextNamespace.lower() == "hexen":
				self.MapNamespace = TMapNamespace.MapNamespaceHexen
			elif TextNamespace.lower() == "zdoom":
				self.MapNamespace = TMapNamespace.MapNamespaceZDoom
			else:
				raise EUdmfException(f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: invalid namespace '{Token}'")
		else:
			raise EUdmfException(f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected namespace keyword, found '{Token}'")

	def Expects(self, Value):
		Token = self.UdmfLexer.GetToken()
		return Token.lower() == Value.lower()

	def GetValueToken(self):
		Result = ""
		if self.Expects("="):
			Result = self.UdmfLexer.GetToken()
			if not self.Expects(";"):
				raise EUdmfException(f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected ';'")
		else:
			raise EUdmfException(f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: expected '='")
		return Result

	def GetBooleanToken(self):
		Token = self.GetValueToken()
		return Token.lower() == "true"

	def GetIntToken(self):
		Token = self.GetValueToken()
		return int(float(Token))

	def GetFloatToken(self):
		Token = self.GetValueToken()
		return float(Token)

	def GetMapVertex(self):
		X = 0
		Y = 0
		ValidX = False
		ValidY = False
		self.Expects("{")
		while True:
			Token = self.UdmfLexer.GetToken()
			if Token.lower() == "x":
				X = self.GetIntToken()
				ValidX = True
			if Token.lower() == "y":
				Y = self.GetIntToken()
				ValidY = True
			if Token == "}":
				break
		if ValidX and ValidY:
			return TMapVertex(X, Y)
		else:
			return None

	def GetMapLinedef(self):
		V1 = 0
		V2 = 0
		ValidV1 = False
		ValidV2 = False
		ID = 0
		Blocking = False
		BlockMonsters = False
		TwoSided = False
		SideFront = 0
		SideBack = 0
		Special = 0
		Arg0 = 0
		Arg1 = 0
		Arg2 = 0
		Arg3 = 0
		Arg4 = 0
		MonsterUse = False
		RepeatableSpecial = False
		Ignored = False
		self.Expects("{")
		while True:
			Token = self.UdmfLexer.GetToken()
			if Token.lower() == "v1":
				V1 = self.GetIntToken()
				ValidV1 = True
			if Token.lower() == "v2":
				V2 = self.GetIntToken()
				ValidV2 = True
			if Token.lower() == "id":
				ID = self.GetIntToken()
			if Token.lower() == "blocking":
				Blocking = self.GetBooleanToken()
			if Token.lower() == "blockmonsters":
				BlockMonsters = self.GetBooleanToken()
			if Token.lower() == "twosided":
				TwoSided = self.GetBooleanToken()
			if Token.lower() == "sidefront":
				SideFront = self.GetIntToken()
			if Token.lower() == "sideback":
				SideBack = self.GetIntToken()
			if Token.lower() == "special":
				Special = self.GetIntToken()
			if Token.lower() == "arg0":
				Arg0 = self.GetIntToken()
			if Token.lower() == "arg1":
				Arg1 = self.GetIntToken()
			if Token.lower() == "arg2":
				Arg2 = self.GetIntToken()
			if Token.lower() == "arg3":
				Arg3 = self.GetIntToken()
			if Token.lower() == "arg4":
				Arg4 = self.GetIntToken()
			if Token.lower() == "monsteruse":
				MonsterUse = self.GetBooleanToken()
			if Token.lower() == "repeatablespecial":
				RepeatableSpecial = self.GetBooleanToken()
			if Token.lower() == "ignored":
				Ignored = self.GetBooleanToken()
			if Token == "}":
				break
		if ValidV1 and ValidV2:
			return TMapLinedef(V1, V2, ID, Blocking, TwoSided, SideFront, SideBack, Special, Arg0, Arg1, Arg2, Arg3, Arg4, MonsterUse, RepeatableSpecial, Ignored)
		else:
			return None

	def GetMapSidedef(self):
		Sector = 0
		ValidSector = False
		self.Expects("{")
		while True:
			Token = self.UdmfLexer.GetToken()
			if Token.lower() == "sector":
				Sector = self.GetIntToken()
				ValidSector = True
			if Token == "}":
				break
		if ValidSector:
			return TMapSidedef(Sector)
		else:
			return None

	def GetMapSector(self):
		HeightFloor = 0
		HeightCeiling = 0
		ID = 0
		Special = 0
		Ignored = False
		self.Expects("{")
		while True:
			Token = self.UdmfLexer.GetToken()
			if Token.lower() == "heightfloor":
				HeightFloor = self.GetIntToken()
			if Token.lower() == "heightceiling":
				HeightCeiling = self.GetIntToken()
			if Token.lower() == "id":
				ID = self.GetIntToken()
			if Token.lower() == "special":
				Special = self.GetIntToken()
			if Token.lower() == "ignored":
				Ignored = self.GetBooleanToken()
			if Token == "}":
				break
		return TMapSector(HeightFloor, HeightCeiling, ID, Special, Ignored)


	def GetMapThing(self):
		X = 0
		Y = 0
		Z = 0
		ThingType = 0
		ValidX = False
		ValidY = False
		ValidThingType = False
		ID = 0
		Special = 0
		Arg0 = 0
		Arg1 = 0
		Arg2 = 0
		Arg3 = 0
		Arg4 = 0
		self.Expects("{")
		while True:
			Token = self.UdmfLexer.GetToken()
			if Token.lower() == "x":
				X = self.GetIntToken()
				ValidX = True
			if Token.lower() == "y":
				Y = self.GetIntToken()
				ValidY = True
			if Token.lower() == "z":
				Z = self.GetIntToken()
			if Token.lower() == "thingtype":
				ThingType = self.GetIntToken()
				ValidThingType = True
			if Token.lower() == "id":
				ID = self.GetIntToken()
			if Token.lower() == "special":
				Special = self.GetIntToken()
			if Token.lower() == "arg0":
				Arg0 = self.GetIntToken()
			if Token.lower() == "arg1":
				Arg1 = self.GetIntToken()
			if Token.lower() == "arg2":
				Arg2 = self.GetIntToken()
			if Token.lower() == "arg3":
				Arg3 = self.GetIntToken()
			if Token.lower() == "arg4":
				Arg4 = self.GetIntToken()
			if Token == "}":
				break
		if ValidX and ValidY and ValidThingType:
			return TMapVertex(X, Y)
		else:
			return None

	def ParseMap(self, MapDefinition):
		if MapDefinition == None:
			raise ValueError("MapDefinition cannot be null")
		Token = self.UdmfLexer.GetToken()
		while Token:
			if Token.lower() == "vertex":
				MapDefinition.AddMapVertex(self.GetMapVertex())
			elif Token.lower() == "linedef":
				MapDefinition.AddMapLinedef(self.GetMapLinedef())
			elif Token.lower() == "sidedef":
				MapDefinition.AddMapSidedef(self.GetMapSidedef())
			elif Token.lower() == "sector":
				MapDefinition.AddMapSector(self.GetMapSector())
			elif Token.lower() == "thing":
				MapDefinition.AddMapThing(self.GetMapThing())
			else:
				raise EUdmfException(f"Row {self.UdmfLexer.Row}, column {self.UdmfLexer.Column}: unknown UDMF entity '{Token}'")
			Token = self.UdmfLexer.GetToken();
