delete from BoardAction
dbcc CHECKIDENT (BoardAction, RESEED, 0)

delete from PlayerAction
dbcc CHECKIDENT (PlayerAction, RESEED, 0)

delete from HandPlayer
dbcc CHECKIDENT (HandPlayer, RESEED, 0)

delete from Hand
dbcc CHECKIDENT (Hand, RESEED, 0)

delete from Player
dbcc CHECKIDENT (Player, RESEED, 0)

delete from Game
dbcc CHECKIDENT (Game, RESEED, 0)