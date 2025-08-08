import os
import time
import tkinter
import tkinter.filedialog
import tkinter.messagebox
import tkinter.scrolledtext
import Archives
import Builder

OpenArchives = []
ArchiveIndex = -1
MapIndex = -1
NavMesh = None

def DoNothing():
	x = 0

def CheckValue(Entry, MinValue, MaxValue):
	try:
		Value = int(Entry.get().strip())
		Valid = MinValue <= Value <= MaxValue
	except ValueError:
		Valid = False
	Entry.config(fg = 'black' if Valid else 'red')
	return Valid

def ResetGUI():
	global ArchiveIndex, MapIndex, NavMesh
	ArchiveIndex = -1
	MapIndex = -1
	NavMesh = None
	ListBoxArchives.delete(first = 0, last = tkinter.END)
	ListBoxMaps.delete(first = 0, last = tkinter.END)
	EntryArchiveNameText.set("")
	EntryArchiveLocationText.set("")
	EntryArchiveFileSizeText.set("")
	EntryArchiveLastUpdateText.set("")
	EntryMapNameText.set("")
	EntryMapNamespaceText.set("")
	EntryMapVertexText.set("")
	EntryMapLinedefText.set("")
	EntryMapSidedefText.set("")
	EntryMapSectorText.set("")
	EntryMapThingText.set("")

def ListBoxArchivesSelect(Event):
	global ArchiveIndex, MapIndex, NavMesh
	MapIndex = -1
	NavMesh = None
	Selection = ListBoxArchives.curselection()
	if Selection:
		ArchiveIndex = Selection[0]
		Archive = OpenArchives[ArchiveIndex]
		ListBoxMaps.delete(first = 0, last = tkinter.END)
		for MapDefinition in Archive.MapDefinitions:
			ListBoxMaps.insert("end", MapDefinition.MapName)
		EntryArchiveNameText.set(Archive.FileName)
		EntryArchiveLocationText.set(Archive.FileSpec)
		EntryArchiveFileSizeText.set(Archive.FileSize)
		EntryArchiveLastUpdateText.set(time.ctime(Archive.LastUpdate))
		EntryMapNameText.set("")
		EntryMapNamespaceText.set("")
		EntryMapVertexText.set("")
		EntryMapLinedefText.set("")
		EntryMapSidedefText.set("")
		EntryMapSectorText.set("")
		EntryMapThingText.set("")

def ListBoxMapsSelect(Event):
	global ArchiveIndex, MapIndex, NavMesh
	NavMesh = None
	Selection = ListBoxMaps.curselection()
	if Selection:
		MapIndex = Selection[0]
		Archive = OpenArchives[ArchiveIndex]
		MapDefinition = Archive.MapDefinitions[MapIndex]
		EntryMapNameText.set(MapDefinition.MapName)
		EntryMapNamespaceText.set(MapDefinition.MapNamespaceText())
		EntryMapVertexText.set(len(MapDefinition.MapVertex))
		EntryMapLinedefText.set(len(MapDefinition.MapLinedef))
		EntryMapSidedefText.set(len(MapDefinition.MapSidedef))
		EntryMapSectorText.set(len(MapDefinition.MapSector))
		EntryMapThingText.set(len(MapDefinition.MapThing))

def OpenArchive():
	FileTypes = (
		("Text files", "*.txt"),
		("WAD files", "*.wad"),
		("PK3 files", "*.pk3")
	)
	FileName = tkinter.filedialog.askopenfilename(title = "Open archive", initialdir = ".", filetypes = FileTypes)
	if FileName:
		Archive = Archives.TArchive()
		if Archive.Load(FileName):
			OpenArchives.append(Archive)
		ResetGUI()
		for Archive in OpenArchives:
			ListBoxArchives.insert("end", Archive.FileName)

def OpenArchiveLocation():
	Selection = ListBoxArchives.curselection()
	if Selection:
		Archive = OpenArchives[Selection[0]]
		os.system("explorer.exe \"" + Archive.FileSpec.rsplit("/", 1)[0].replace("/", "\\") + "\"")

def CloseArchive():
	Selection = ListBoxArchives.curselection()
	if Selection:
		OpenArchives.pop(Selection[0])
		ResetGUI()
		for Archive in OpenArchives:
			ListBoxArchives.insert("end", Archive.FileName)

def GenerateNavMesh():
	global ArchiveIndex, MapIndex, NavMesh
	Selection = ListBoxMaps.curselection()
	if Selection:
		MapIndex = Selection[0]
		Archive = OpenArchives[ArchiveIndex]
		Map = Archive.MapDefinitions[MapIndex]
		try:
			ActorHeight = int(EntryActorHeight.get())
			ActorWidth = int(EntryActorWidth.get())
			NavMeshSettings = Builder.TNavMeshSettings(ActorHeight, ActorWidth)
			global NavMesh
			NavMesh = Builder.TNavMesh()
			NavMesh.Build(NavMeshSettings, Map)
		except ValueError as E:
			tkinter.messagebox.showinfo(title = "Hint", message = "Please check the actor height and width!")
	else:
		tkinter.messagebox.showinfo(title = "Hint", message = "No map selected!")

def SaveNavMesh():
	global ArchiveIndex, MapIndex, NavMesh
	FileName = tkinter.filedialog.asksaveasfile(title = "Save navigation mesh", initialdir = ".", defaultextension = ".txt")
	if FileName:
		FileName.write(str(NavMesh))
		FileName.close()

def FormMapDraw():
	return 0

class TFormViewMap(tkinter.Toplevel):
	def __init__(self):
		tkinter.Toplevel.__init__(self)
		self.minsize(640, 400)
		self.resizable(True, True)
		self.title("Map and navigation mesh")
		self.MapCanvas = tkinter.Canvas(self, bg = "black")
		self.MapCanvas.pack(fill = tkinter.BOTH, expand = True)
		self.bind('<Left>', self.KeyLeft)
		self.bind('<Right>', self.KeyRight)
		self.bind('<Up>', self.KeyUp)
		self.bind('<Down>', self.KeyDown)
		self.bind('<Home>', self.KeyHome)
		self.bind('<Shift-Up>', self.KeyPlus)
		self.bind('<Shift-Down>', self.KeyMinus)
		self.NavMesh = None
		self.INSIDE = 0b0000
		self.LEFT = 0b0001
		self.RIGHT = 0b0010
		self.BOTTOM = 0b0100
		self.TOP = 0b1000
		self.MinX = None
		self.MinY = None
		self.MaxX = None
		self.MaxY = None
		self.PosX = 0
		self.PosY = 0
		self.ViewMinX = 0
		self.ViewMinY = 0
		self.ViewMaxX = 0
		self.ViewMaxY = 0
		self.ZoomFactor = 3
	
	def View(self, NavMesh):
		self.NavMesh = NavMesh
		self.PosX = 32768
		self.PosY = 32768
		for MapVertex in self.NavMesh.MapDefinition.MapVertex:
			if (self.MinX == None) or (self.MinX > MapVertex.X):
				self.MinX = MapVertex.X
			if (self.MinY == None) or (self.MinY > MapVertex.Y):
				self.MinY = MapVertex.Y
			if (self.MaxX == None) or (self.MaxX < MapVertex.X):
				self.MaxX = MapVertex.X
			if (self.MaxY == None) or (self.MaxY < MapVertex.Y):
				self.MaxX = MapVertex.Y
	
	def ComputeRC(self, X, Y):
		Result = self.INSIDE
		if X < self.ViewMinX:
			Result += self.LEFT
		if X > self.ViewMaxX:
			Result += self.RIGHT
		if Y < self.ViewMinY:
			Result += self.BOTTOM
		if Y > self.ViewMaxY:
			Result += self.TOP
		return Result
			
	def ComputeVisible(self, X1, Y1, X2, Y2):
		Result = False
		RC1 = self.ComputeRC(X1, Y1)
		RC2 = self.ComputeRC(X2, Y2)
		while (True):
			if ((RC1 == 0) and (RC2 == 0)):
				# Both endpoints are visible.
				Result = True
				break
			else:
				if ((RC1 & RC2) != 0):
					# Both endpoints are not visible, in same region.
					break
				else:
					# Some segment of line lies within the screen port.
					RC = 0
					X = 0.0
					Y = 0.0
					# At least one endpoint is outside the screen port, pick it.
					if (RC1 != 0):
						RC = RC1
					else:
						RC = RC2
					# Find intersection point using formulas y = y1 + slope * (x - x1), x = x1 + (1 / slope) * (y - y1).
					if ((RC & self.TOP) != 0):
						# Point is above the screen port.
						X = X1 + (X2 - X1) * (self.ViewMaxY - Y1) / (Y2 - Y1)
						Y = self.ViewMaxY
					elif ((RC & self.BOTTOM) != 0):
						# Point is below the screen port.
						X = X1 + (X2 - X1) * (self.ViewMaxY - Y1) / (Y2 - Y1)
						Y = self.ViewMinY
					elif ((RC & self.RIGHT) != 0):
						# Point is to the right of the screen port.
						Y = Y1 + (Y2 - X1) * (self.ViewMaxX - X1) / (X2 - X1)
						X = self.ViewMaxX
					elif ((RC & self.LEFT) != 0):
						# Point is to the left of the screen port.
						Y = Y1 + (X2 - X1) * (self.ViewMinX - X1) / (X2 - X1)
						X = self.ViewMinX
					else:
						X = 0
						Y = 0
					if (RC == RC1):
						X1 = int(X)
						Y1 = int(Y)
						RC1 = self.ComputeRC(X1, Y1)
					else:
						X2 = int(X)
						Y2 = int(Y)
						RC2 = self.ComputeRC(X2, Y2)
		return Result
	
	def Clamp(self, Value, Min, Max):
		if Value < Min:
			return Min
		if Value > Max:
			return Max
		return Value
	
	def KeyUp(self, Event):
		self.PosY = self.Clamp(self.PosY - (16 << self.ZoomFactor), 0, 65536)
		self.Draw()

	def KeyDown(self, Event):
		self.PosY = self.Clamp(self.PosY + (16 << self.ZoomFactor), 0, 65536)
		self.Draw()
		
	def KeyLeft(self, Event):
		self.PosX = self.Clamp(self.PosX - (16 << self.ZoomFactor), 0, 65536)
		self.Draw()

	def KeyRight(self, Event):
		self.PosX = self.Clamp(self.PosX + (16 << self.ZoomFactor), 0, 65536)
		self.Draw()

	def KeyHome(self, Event):
		self.PosX = 32768
		self.PosY = 32768
		self.Draw()
		
	def KeyMinus(self, Event):
		self.ZoomFactor = self.Clamp(self.ZoomFactor - 1, 1, 13)
		self.Draw()

	def KeyPlus(self, Event):
		self.ZoomFactor = self.Clamp(self.ZoomFactor + 1, 1, 13)
		self.Draw()

	def Draw(self):
		self.MapCanvas.delete("all")
		self.update()
		self.ViewMinX = 0
		self.ViewMinY = 0
		self.ViewMaxX = self.ViewMinX + self.MapCanvas.winfo_width()
		self.ViewMaxY = self.ViewMinY + self.MapCanvas.winfo_height()
		for MapLinedef in self.NavMesh.MapDefinition.MapLinedef:
			V1 = self.NavMesh.MapDefinition.MapVertex[MapLinedef.V1]
			V2 = self.NavMesh.MapDefinition.MapVertex[MapLinedef.V2]
			V1X = (V1.X - self.PosX + 32768) >> self.ZoomFactor
			V1Y = ((-V1.Y) - self.PosY + 32768) >> self.ZoomFactor
			V2X = (V2.X - self.PosX + 32768) >> self.ZoomFactor
			V2Y = ((-V2.Y) - self.PosY + 32768) >> self.ZoomFactor
			if self.ComputeVisible(V1X, V1Y, V2X, V2Y):
				if MapLinedef.Blocking:
					self.MapCanvas.create_line(V1X, V1Y, V2X, V2Y, fill = "white")
				else:
					self.MapCanvas.create_line(V1X, V1Y, V2X, V2Y, fill = "gray")
		for Line in self.NavMesh.Lines:
			V1X = (Line.A.X - self.PosX + 32768) >> self.ZoomFactor
			V1Y = ((-Line.A.Y) - self.PosY + 32768) >> self.ZoomFactor
			V2X = (Line.B.X - self.PosX + 32768) >> self.ZoomFactor
			V2Y = ((-Line.B.Y) - self.PosY + 32768) >> self.ZoomFactor
			if self.ComputeVisible(V1X, V1Y, V2X, V2Y):
				if Line.Portal >= 0:
					self.MapCanvas.create_line(V1X, V1Y, V2X, V2Y, fill = "blue")
				else:
					self.MapCanvas.create_line(V1X, V1Y, V2X, V2Y, fill = "red")
		
def ShowMap():
	global NavMesh
	FormViewMap = TFormViewMap()
	FormViewMap.View(NavMesh)
	FormViewMap.Draw()
	FormViewMap.mainloop()

def ShowText(Title, Text):
	global NavMesh
	FormNavMesh = tkinter.Toplevel()
	FormNavMesh.minsize(640, 400)
	FormNavMesh.resizable(True, True)
	FormNavMesh.title(Title)
	TextNavMesh = tkinter.scrolledtext.ScrolledText(FormNavMesh)
	TextNavMesh.insert("end", chars = Text)
	TextNavMesh.config(state = tkinter.DISABLED)
	TextNavMesh.pack(fill = tkinter.BOTH, expand = True)
	FormNavMesh.mainloop()

def ShowNavMesh():
	global NavMesh
	ShowText("Navigation mesh", NavMesh.GetText())

def ShowNavMeshMessages():
	global NavMesh
	ShowText("Navigation mesh build messages", NavMesh.GetMessages())

def ShowAbout():
	FormAbout = tkinter.Toplevel()
	FormAbout.minsize(640, 200)
	FormAbout.resizable(False, False)
	FormAbout.title("About")
	LabelTitle = tkinter.Label(FormAbout, text = "ZDoom Navigation Mesh builder", font = ("Verdana", 18))
	LabelTitle.grid(row = 0, column = 0, padx = 5)
	LabelAbout0 = tkinter.Label(FormAbout, text = "This application creates NavMesh from ZDoom maps and saves them in text form")
	LabelAbout0.grid(row = 1, column = 0, padx = 5)
	LabelAbout1 = tkinter.Label(FormAbout, text = "Valid maps supported are WAD, PK3, ZIP and TXT text files (text for UDMF only)")
	LabelAbout1.grid(row = 2, column = 0, padx = 5)
	LabelAbout2 = tkinter.Label(FormAbout, text = "Source code and Wiki documents at the following link:")
	LabelAbout2.grid(row = 3, column = 0, padx = 5)
	LabelAbout3 = tkinter.Label(FormAbout, text = "https://github.com/StefanoP85/Custom-ZDoom-PathFinding")
	LabelAbout3.grid(row = 4, column = 0, padx = 5)
	FormAbout.mainloop()

if __name__ == "__main__":
	root = tkinter.Tk()
	root.option_readfile("AppGUI.cfg")
	root.minsize(640, 480)
	root.title("NZDoom NavMesh builder")
	MenuBar = tkinter.Menu(root)
	MenuFile = tkinter.Menu(MenuBar, tearoff = 0)
	MenuFile.add_command(label = "Open archive...", command = OpenArchive)
	MenuFile.add_command(label = "Open selected archive location...", command = OpenArchiveLocation)
	MenuFile.add_command(label = "Close selected archive", command = CloseArchive)
	MenuFile.add_command(label = "Save navigation mesh...", command = SaveNavMesh)
	MenuFile.add_separator()
	MenuFile.add_command(label = "Exit", command = root.quit)
	MenuBar.add_cascade(label = "File", menu = MenuFile)
	MenuView = tkinter.Menu(MenuBar, tearoff = 0)
	MenuView.add_command(label = "Selected map...", command = ShowMap)
	MenuView.add_command(label = "Navigation mesh text...", command = ShowNavMesh)
	MenuView.add_command(label = "Build messages...", command = ShowNavMeshMessages)
	MenuBar.add_cascade(label = "View", menu = MenuView)
	MenuHelp = tkinter.Menu(MenuBar, tearoff = 0)
	MenuHelp.add_command(label = "About...", command = ShowAbout)
	MenuBar.add_cascade(label = "Help", menu = MenuHelp)
	root.config(menu = MenuBar)
	root.columnconfigure(0, weight = 1)
	root.columnconfigure(1, weight = 1)
	root.columnconfigure(2, weight = 2)
	root.rowconfigure(0, weight = 1)
	FrameArchives = tkinter.Frame(root)
	FrameArchives.grid(column = 0, row = 0, sticky = "NSWE")
	LabelArchives = tkinter.Label(FrameArchives, text = "Archives")
	LabelArchives.pack()
	ListBoxArchives = tkinter.Listbox(FrameArchives, selectmode = tkinter.SINGLE)
	ListBoxArchives.pack(fill = tkinter.BOTH, expand = True)
	ListBoxArchives.bind("<<ListboxSelect>>", ListBoxArchivesSelect)
	FrameMaps = tkinter.Frame(root)
	FrameMaps.grid(column = 1, row = 0, sticky = "NSWE")
	LabelMaps = tkinter.Label(FrameMaps, text = "Maps")
	LabelMaps.pack()
	ListBoxMaps = tkinter.Listbox(FrameMaps)
	ListBoxMaps.pack(fill = tkinter.BOTH, expand = True)
	ListBoxMaps.bind("<<ListboxSelect>>", ListBoxMapsSelect)
	FrameDetails = tkinter.Frame(root)
	FrameDetails.grid(column = 2, row = 0, sticky = "NSWE")
	FrameDetailArchive = tkinter.Frame(FrameDetails, highlightbackground = "black", highlightthickness = 1)
	FrameDetailArchive.pack(fill = tkinter.BOTH, expand = True)
	LabelArchiveName = tkinter.Label(FrameDetailArchive, text = "Archive name")
	LabelArchiveName.pack(padx = 2, pady = 2)
	EntryArchiveNameText = tkinter.StringVar()
	EntryArchiveName = tkinter.Entry(FrameDetailArchive, textvariable = EntryArchiveNameText, state = "readonly")
	EntryArchiveName.pack(padx = 2, pady = 2)
	LabelArchiveLocation = tkinter.Label(FrameDetailArchive, text = "Archive location")
	LabelArchiveLocation.pack(padx = 2, pady = 2)
	EntryArchiveLocationText = tkinter.StringVar()
	EntryArchiveLocation = tkinter.Entry(FrameDetailArchive, textvariable = EntryArchiveLocationText, state = "readonly")
	EntryArchiveLocation.pack(padx = 2, pady = 2)
	LabelArchiveFileSize = tkinter.Label(FrameDetailArchive, text = "File size")
	LabelArchiveFileSize.pack(padx = 2, pady = 2)
	EntryArchiveFileSizeText = tkinter.StringVar()
	EntryArchiveFileSize = tkinter.Entry(FrameDetailArchive, textvariable = EntryArchiveFileSizeText, state = "readonly", width = 40)
	EntryArchiveFileSize.pack(padx = 2, pady = 2)
	LabelArchiveLastUpdate = tkinter.Label(FrameDetailArchive, text = "Last file update")
	LabelArchiveLastUpdate.pack(padx = 2, pady = 2)
	EntryArchiveLastUpdateText = tkinter.StringVar()
	EntryArchiveLastUpdate = tkinter.Entry(FrameDetailArchive, textvariable = EntryArchiveLastUpdateText, state = "readonly", width = 40)
	EntryArchiveLastUpdate.pack(padx = 2, pady = 2)
	FrameDetailMap = tkinter.Frame(FrameDetails, highlightbackground = "black", highlightthickness = 1)
	FrameDetailMap.pack(fill = tkinter.BOTH, expand = True)
	LabelMapName = tkinter.Label(FrameDetailMap, text = "Map name")
	LabelMapName.grid(column = 0, row = 0, columnspan = 5)
	EntryMapNameText = tkinter.StringVar()
	EntryMapName = tkinter.Entry(FrameDetailMap, textvariable = EntryMapNameText, state = "readonly", width = 40)
	EntryMapName.grid(column = 0, row = 1, columnspan = 5)
	LabelMapNamespace = tkinter.Label(FrameDetailMap, text = "Map namespace")
	LabelMapNamespace.grid(column = 0, row = 2, columnspan = 5)
	EntryMapNamespaceText = tkinter.StringVar()
	EntryMapNamespace = tkinter.Entry(FrameDetailMap, textvariable = EntryMapNamespaceText, state = "readonly", width = 40)
	EntryMapNamespace.grid(column = 0, row = 3, columnspan = 5)
	LabelMapVertex = tkinter.Label(FrameDetailMap, text = "Vertex")
	LabelMapVertex.grid(column = 0, row = 4)
	EntryMapVertexText = tkinter.StringVar()
	EntryMapVertex = tkinter.Entry(FrameDetailMap, textvariable = EntryMapVertexText, state = "readonly", width = 6)
	EntryMapVertex.grid(column = 0, row = 5)
	LabelMapLinedef = tkinter.Label(FrameDetailMap, text = "Linedef")
	LabelMapLinedef.grid(column = 1, row = 4)
	EntryMapLinedefText = tkinter.StringVar()
	EntryMapLinedef = tkinter.Entry(FrameDetailMap, textvariable = EntryMapLinedefText, state = "readonly", width = 6)
	EntryMapLinedef.grid(column = 1, row = 5)
	LabelMapSidedef = tkinter.Label(FrameDetailMap, text = "Sidedef")
	LabelMapSidedef.grid(column = 2, row = 4)
	EntryMapSidedefText = tkinter.StringVar()
	EntryMapSidedef = tkinter.Entry(FrameDetailMap, textvariable = EntryMapSidedefText, state = "readonly", width = 6)
	EntryMapSidedef.grid(column = 2, row = 5)
	LabelMapSector = tkinter.Label(FrameDetailMap, text = "Sector")
	LabelMapSector.grid(column = 3, row = 4)
	EntryMapSectorText = tkinter.StringVar()
	EntryMapSector = tkinter.Entry(FrameDetailMap, textvariable = EntryMapSectorText, state = "readonly", width = 6)
	EntryMapSector.grid(column = 3, row = 5)
	LabelMapThing = tkinter.Label(FrameDetailMap, text = "Thing")
	LabelMapThing.grid(column = 4, row = 4)
	EntryMapThingText = tkinter.StringVar()
	EntryMapThing = tkinter.Entry(FrameDetailMap, textvariable = EntryMapThingText, state = "readonly", width = 6)
	EntryMapThing.grid(column = 4, row = 5)
	FrameDetailNavMesh = tkinter.Frame(FrameDetails, highlightbackground = "black", highlightthickness = 1)
	FrameDetailNavMesh.pack(fill = tkinter.BOTH, expand = True)
	LabelActorHeight = tkinter.Label(FrameDetailNavMesh, text = "Actor height")
	LabelActorHeight.pack(padx = 2, pady = 2)
	EntryActorHeight = tkinter.Entry(FrameDetailNavMesh)
	EntryActorHeight.insert(tkinter.END, "128")
	EntryActorHeight.bind("<KeyRelease>", lambda E: CheckValue(EntryActorHeight, 1, 256))
	EntryActorHeight.pack(padx = 2, pady = 2)
	LabelActorWidth = tkinter.Label(FrameDetailNavMesh, text = "Actor width")
	LabelActorWidth.pack(padx = 2, pady = 2)
	EntryActorWidth = tkinter.Entry(FrameDetailNavMesh)
	EntryActorWidth.insert(tkinter.END, "32")
	EntryActorWidth.bind("<KeyRelease>", lambda E: CheckValue(EntryActorWidth, 1, 256))
	EntryActorWidth.pack(padx = 2, pady = 2)
	ButtonNavMesh = tkinter.Button(FrameDetailNavMesh, text = "Generate navmesh", command = GenerateNavMesh)
	ButtonNavMesh.pack(padx = 5, pady = 5)
	root.mainloop()
