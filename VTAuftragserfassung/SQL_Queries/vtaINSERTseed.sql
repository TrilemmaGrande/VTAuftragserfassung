USE mechanik_test;

BEGIN TRANSACTION;

INSERT INTO vta_Gesellschafter (Firmenname) VALUES
('ABC Company'),
('XYZ Corporation'),
('Company X'),
('Company Y'),
('Company Z'),
('Company W'),
('Company V');

INSERT INTO vta_Vertriebsmitarbeiter (MitarbeiterId, Vorname, Nachname, Firma) VALUES
('id1','Max', 'Mustermann', 'SalesCo'),
('id2','Tim', 'MusterZwei', 'Vertriebsoffensive'),
('id3','Anna', 'Musterfrau', 'SalesTech'),
('id4','Jürgen', 'Mustermann', 'Vertriebsdefensive');

INSERT INTO vta_Auth (FK_Vertriebsmitarbeiter, HashedAuth) VALUES
(1, ''),
(2, 'AQAAAAIAAYagAAAAEMOQ5q1sxEzI7aZC3qVao3Cvn6kxkoSSbJz3h2SP8sHg3xC7X4iuUR9ijYf1MoSAyg=='),
(3, ''),
(4, '');

INSERT INTO vta_Kunde (FK_Gesellschafter, Kundennummer, Vorname, Nachname, EMail, IstWerkstatt, IstHandel, Firma, Telefon, Strasse, Postleitzahl, Ort) VALUES
(1, 'K1134', 'John', 'Doe', 'generic@provider.com', 1, 0, 'Doe Enterprises', '123456789', 'Main Street', '12345', 'Cityville'),
(2, 'K1434','Jane', 'Smith',  'generic@provider.com', 0, 1, 'Smith & Co.', '987654321', 'Oak Avenue', '67890', 'Towntown');
DECLARE @i INT = 0;
WHILE @i < 8800
BEGIN
  INSERT INTO vta_Kunde (FK_Gesellschafter, Kundennummer, Vorname, Nachname, EMail, IstWerkstatt, IstHandel, Firma, Telefon, Strasse, Postleitzahl, Ort)
  VALUES
  (3, 'C146' + CAST(@i AS NVARCHAR), 'EinVorname', 'EinNachname' + CAST(@i AS NVARCHAR), 'generic@provider.com', 1, 0, 'Werkstatt Ltd.', '123456789', 'Street ' + CAST(@i AS NVARCHAR), '1234' + CAST(@i AS NVARCHAR), 'City ' + CAST(@i AS NVARCHAR)),
  (4, 'D136' + CAST(@i AS NVARCHAR),'NochEinVorname', 'NochEinNachname' + CAST(@i AS NVARCHAR), 'generic@provider.com', 0, 1, 'Handel GmbH', '987654321', 'Avenue ' + CAST(@i AS NVARCHAR), '5678' + CAST(@i AS NVARCHAR), 'Town ' + CAST(@i AS NVARCHAR));

  SET @i = @i + 1;
END

INSERT INTO vta_Auftrag (FK_Kunde, FK_Vertriebsmitarbeiter, SummeAuftrag, Auftragsstatus, HatZugabe, Hinweis, ErstelltAm, LetzteStatusAenderung) VALUES
(1, 1, 50.99, 1, 1, 'Expressversand gewünscht', GETDATE(), GETDATE()),
(2, 2, 35.50, 2, 0, 'Standardversand', GETDATE(), GETDATE());
DECLARE @j INT = 0;
WHILE @j < 100
BEGIN
  INSERT INTO vta_Auftrag (FK_Kunde, FK_Vertriebsmitarbeiter, SummeAuftrag, Auftragsstatus, HatZugabe, Hinweis, ErstelltAm, LetzteStatusAenderung)
  VALUES
  (3, 1, 50.99 + @j, 1, 1, 'Expressversand gewünscht', GETDATE(), GETDATE()),
  (4, 2, 35.50 + @j, 2, 0, 'Standardversand', GETDATE(), GETDATE()),
   (5, 3, 50.99 + @j, 1, 1, 'Versand per Brieftaube', GETDATE(), GETDATE()),
    (6, 4, 50.99 + @j, 1, 1, 'Bester Kunde bekommt Sonderbehandlung', GETDATE(), GETDATE());

  SET @j = @j + 1;
END

INSERT INTO vta_Artikel (Artikelnummer, Bezeichnung1, Bezeichnung2, Preis, Verpackungseinheit) VALUES
('A001', 'Schraubendreher', 'Professionell', 12.99, 'Stück'),
('A002', 'Schrauben-Set', 'Verschiedene Größen', 24.99, 'Set');
DECLARE @k INT = 0;
WHILE @k < 9900 
BEGIN
  INSERT INTO vta_Artikel (Artikelnummer, Bezeichnung1, Bezeichnung2, Preis, Verpackungseinheit)
  VALUES
  ('B' + RIGHT('0000' + CAST(@k AS NVARCHAR), 4), 'Article' + CAST(@k AS NVARCHAR), 'Description ' + CAST(@k AS NVARCHAR), 10.00 + @k, 'Unit');

  SET @k = @k + 1.2;
END

INSERT INTO vta_Position (FK_Artikel, FK_Auftrag, PositionsNummer, Menge, SummePosition) VALUES
(1, 1, 1, 3, 38.97),
(2, 2, 2, 1, 24.99),
(1, 3, 1, 3, 38.97),
(1, 4, 1, 3, 38.97);
DECLARE @m INT = 3;
WHILE @m < 100
BEGIN
  INSERT INTO vta_Position (FK_Artikel, FK_Auftrag, PositionsNummer, Menge, SummePosition)
  VALUES
  ((@m % 90) + 1, (@m % 100) + 1, (@m % 90) + 1, 2, (10.00 + (@m % 90) + (@m % 100)));

  SET @m = @m + 1;
END

COMMIT;