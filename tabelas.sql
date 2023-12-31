CREATE TABLE IF NOT EXISTS Posicoes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    IdPosicaoRisco INTEGER NOT NULL,
    IdRastreador INTEGER NOT NULL,
    IdVeiculo INTEGER NOT NULL,
    EventoDatahora DATETIME NOT NULL,
    CpfMotorista TEXT,
    Placa TEXT,
    GPS_Latitude DECIMAL,
    GPS_Longitude DECIMAL,
    GPS_Direcao INTEGER,
    Hodometro INTEGER,
    Ignicao INTEGER
);

CREATE INDEX IF NOT EXISTS Posicoes_IdPosicaoRisco ON Posicoes (IdPosicaoRisco);

CREATE TABLE IF NOT EXISTS  Mensagens (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  IdMensagemRecebidaRisco INTEGER NOT NULL,
  IdEquipamento INTEGER NOT NULL,
  IdVeiculo INTEGER NOT NULL,
  DataCriacaoNoEquipamento DATETIME NOT NULL,
  Placa TEXT,
  TipoMacro INTEGER NOT NULL,
  Texto TEXT
);
CREATE INDEX IF NOT EXISTS Mensagens_IdMensagemRecebidaRisco ON Mensagens (IdMensagemRecebidaRisco);