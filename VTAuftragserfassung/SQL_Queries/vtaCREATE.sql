USE mechanik_test;

-- GESELLSCHAFTER
CREATE TABLE vta_Gesellschafter (
    PK_Gesellschafter INT IDENTITY(1,1) PRIMARY KEY,
    Firmenname NVARCHAR(100) NOT NULL
);

-- VERTRIEBSMITARBEITER
CREATE TABLE vta_Vertriebsmitarbeiter (
    PK_Vertriebsmitarbeiter INT IDENTITY(1,1) PRIMARY KEY,
	MitarbeiterId NVARCHAR(10) UNIQUE NOT NULL,
    Vorname NVARCHAR(50) NOT NULL,
    Nachname NVARCHAR(50) NOT NULL,
    Firma NVARCHAR(100)
);

-- ARTIKEL
CREATE TABLE vta_Artikel (
    PK_Artikel INT IDENTITY(1,1) PRIMARY KEY,
    Artikelnummer NVARCHAR(100) NOT NULL,
    Bezeichnung1 NVARCHAR(100) NOT NULL,
    Bezeichnung2 NVARCHAR(100),
    Preis FLOAT NOT NULL,
    Verpackungseinheit NVARCHAR(50)
);

-- AUTHENTICATION -> VERTRIEBSMITARBEITER
CREATE TABLE vta_Auth (
    PK_Auth INT IDENTITY(1,1) PRIMARY KEY,
    FK_Vertriebsmitarbeiter INT NOT NULL,
    FOREIGN KEY (FK_Vertriebsmitarbeiter) REFERENCES vta_Vertriebsmitarbeiter(PK_Vertriebsmitarbeiter) ON DELETE CASCADE,
    HashedAuth NVARCHAR(200) NOT NULL
);

-- KUNDEN -> GESELLSCHAFTER
CREATE TABLE vta_Kunde (
    PK_Kunde INT IDENTITY(1,1) PRIMARY KEY,
    FK_Gesellschafter INT NOT NULL,
    FOREIGN KEY (FK_Gesellschafter) REFERENCES vta_Gesellschafter(PK_Gesellschafter),   
	Kundennummer NVARCHAR(50) NOT NULL UNIQUE,
    Vorname NVARCHAR(50) NOT NULL,
    Nachname NVARCHAR(50) NOT NULL,
	EMail NVARCHAR(50),
    IstWerkstatt INT,
    IstHandel INT,
    Firma NVARCHAR(100),
    Telefon NVARCHAR(50),
    Strasse NVARCHAR(50),
    Postleitzahl NVARCHAR(10),
    Ort NVARCHAR(50)
);

-- AUFTRÄGE -> KUNDEN & VERTRIEBSMITARBEITER
CREATE TABLE vta_Auftrag (
    PK_Auftrag INT IDENTITY(1,1) PRIMARY KEY,
    FK_Kunde INT NOT NULL,
    FOREIGN KEY (FK_Kunde) REFERENCES vta_Kunde(PK_Kunde),
    FK_Vertriebsmitarbeiter INT NOT NULL,
    FOREIGN KEY (FK_Vertriebsmitarbeiter) REFERENCES vta_Vertriebsmitarbeiter(PK_Vertriebsmitarbeiter),
    SummeAuftrag FLOAT,
    Auftragsstatus INT NOT NULL,
    HatZugabe INT,
    Hinweis NVARCHAR(500),
    ErstelltAm DATETIME NOT NULL,
    LetzteStatusAenderung DATETIME NOT NULL
);

-- POSITIONEN -> ARTIKEL & AUFTRÄGE
CREATE TABLE vta_Position (
    PK_Position INT IDENTITY(1,1) PRIMARY KEY,
    FK_Artikel INT NOT NULL,
    FOREIGN KEY (FK_Artikel) REFERENCES vta_Artikel(PK_Artikel),
    FK_Auftrag INT NOT NULL,
    FOREIGN KEY (FK_Auftrag) REFERENCES vta_Auftrag(PK_Auftrag) ON DELETE CASCADE,
	PositionsNummer INT,
    Menge INT,
    SummePosition FLOAT
);


