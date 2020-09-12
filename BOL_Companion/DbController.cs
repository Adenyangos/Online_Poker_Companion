using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BOL_Companion.DbDataStructures;

namespace BOL_Companion
{
    /// <summary>
    /// A class that handles interactions with the database.
    /// </summary>
    class DbController
    {
        public DbController()
        {

        }

        #region Get DB Context

        /// <summary>
        /// Get a database context for interacting with the database.
        /// </summary>
        /// <returns>A database context for interacting with the database</returns>
        public Bol_Model_DBEntities GetDbContext()
        {
            return new Bol_Model_DBEntities();
        }

        #endregion

        #region Insert Statements

        /// <summary>
        /// Insert a row representing a game into the database.
        /// </summary>
        /// <param name="isTournament">Is this game a tournament</param>
        /// <param name="numPlayers">The number of players in this game</param>
        /// <param name="errorMessagesTextBox">The textbox for logging erros</param>
        /// <returns>The game Id</returns>
        public int InsertGame(bool isTournament, int numPlayers, TextBox errorMessagesTextBox)
        {
            // Create a game object to insert into the database
            Game game = new Game
            {
                Tournament = isTournament,
                NumPlayers = numPlayers,
                DateTimeStart = DateTime.Now
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.Games.Add(game);
                    int numRecordsWritten = ctx.SaveChanges();

                    // Check for errors
                    if (numRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertGame. " + numRecordsWritten + " Records Written", errorMessagesTextBox);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertGame. " + ex.Message, errorMessagesTextBox);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertGame. " + ex.Message, errorMessagesTextBox);
                }
            }

            return game.Id;
        }

        /// <summary>
        /// Insert a row representing a hand into the database.
        /// </summary>
        /// <param name="gameId">The game Id</param>
        /// <param name="ante">The ante for the hand</param>
        /// <param name="errorMessagesTextBox">The textbox for logging erros</param>
        /// <returns>The hand Id</returns>
        public long InsertHand(int gameId, int ante, TextBox errorMessagesTextBox)
        {
            // Create a hand object to insert into the database
            Hand hand = new Hand
            {
                GameId = gameId,
                Ante = ante,
                DateTimeStart = DateTime.Now
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.Hands.Add(hand);
                    int numRecordsWritten = ctx.SaveChanges();

                    // Check for errors
                    if (numRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertHand. " + numRecordsWritten + " Records Written", errorMessagesTextBox);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertHand. " + ex.Message, errorMessagesTextBox);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertHand. " + ex.Message, errorMessagesTextBox);
                }
            }

            return hand.Id;
        }

        /// <summary>
        /// Insert a row representing a player into the database.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="errorMessagesTextBox">The textbox for logging erros</param>
        /// <returns>The player Id</returns>
        public int InsertPlayer(string name, TextBox errorMessagesTextBox)
        {
            // Create a player object to insert into the database
            Player player = new Player
            {
                Name = name
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.Players.Add(player);
                    int numRecordsWritten = ctx.SaveChanges();

                    // Check for errors
                    if (numRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertPlayer. " + numRecordsWritten + " Records Written", errorMessagesTextBox);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertPlayer. " + ex.Message, errorMessagesTextBox);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertPlayer. " + ex.Message, errorMessagesTextBox);
                }
            }
                
            return player.Id;
        }

        /// <summary>
        /// Insert a row representing a handPlayer (a specific player in a specific hand) into the database.
        /// </summary>
        /// <param name="handId">The hand Id</param>
        /// <param name="playerId">The player Id</param>
        /// <param name="chipCountStart">The player's chip count when the hand started (before antes and blinds)</param>
        /// <param name="blind">The blind the player paid for this hand</param>
        /// <param name="holdCard1">The player's first hold card</param>
        /// <param name="holdCard2">The player's second hold card</param>
        /// <param name="errorMessagesTextBox">The textbox for logging erros</param>
        /// <returns>The handPlayer Id</returns>
        public long InsertHandPlayer(long handId, int playerId, int chipCountStart,
            int blind, short holdCard1, short holdCard2, TextBox errorMessagesTextBox)
        {
            // Create a handPlayer object to insert into the database
            HandPlayer handPlayer = new HandPlayer
            {
                HandId = handId,
                PlayerId = playerId,
                ChipCountStart = chipCountStart,
                Blind = blind,
                HoldCard1 = holdCard1,
                HoldCard2 = holdCard2
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.HandPlayers.Add(handPlayer);
                    int numRecordsWritten = ctx.SaveChanges();

                    // Check for errors
                    if (numRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertHandPlayer. " + numRecordsWritten + " Records Written", errorMessagesTextBox);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertHandPlayer. " + ex.Message, errorMessagesTextBox);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertHandPlayer. " + ex.Message, errorMessagesTextBox);
                }
            }

            return handPlayer.Id;
        }

        /// <summary>
        /// Insert a row representing a board action (a card coming on board [flop, turn, river]) into the database.
        /// </summary>
        /// <param name="handId">The hand Id</param>
        /// <param name="boardCard">The board card</param>
        /// <param name="handActionNumber">The hand action number</param>
        /// <param name="errorMessagesTextBox">The textbox for logging erros</param>
        /// <returns>The board action Id</returns>
        public long InsertBoardAction(long handId, short boardCard, int handActionNumber, TextBox errorMessagesTextBox)
        {
            // Create a board action object to insert into the database
            BoardAction boardAction = new BoardAction
            {
                HandId = handId,
                BoardCard = boardCard,
                HandActionNumber = handActionNumber
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.BoardActions.Add(boardAction);
                    int numRecordsWritten = ctx.SaveChanges();

                    // Check for errors
                    if (numRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertBoardAction. " + numRecordsWritten + " Records Written", errorMessagesTextBox);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertBoardAction. " + ex.Message, errorMessagesTextBox);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertBoardAction. " + ex.Message, errorMessagesTextBox);
                }
            }   

            return boardAction.Id;
        }

        /// <summary>
        /// Insert a row representing a player action (bet, call or fold) into the database.
        /// </summary>
        /// <param name="handPlayerId">The handPlayer Id</param>
        /// <param name="chipCountChange">The player's chip count change (positive numbers are bets, negative numbers are pot winnings, 0's are folds)</param>
        /// <param name="handActionNumber">The hand action number</param>
        /// <param name="errorMessagesTextBox">The textbox for logging erros</param>
        /// <returns>The player action Id</returns>
        public long InsertPlayerAction(long handPlayerId, int chipCountChange,
            int handActionNumber, TextBox errorMessagesTextBox)
        {
            // Create a player action object to insert into the database
            PlayerAction playerAction = new PlayerAction
            {
                HandPlayerId = handPlayerId,
                ChipCountChange = chipCountChange,
                HandActionNumber = handActionNumber
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.PlayerActions.Add(playerAction);
                    int numRecordsWritten = ctx.SaveChanges();

                    // Check for errors
                    if (numRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertPlayerAction. " + numRecordsWritten + " Records Written", errorMessagesTextBox);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertPlayerAction. " + ex.Message, errorMessagesTextBox);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertPlayerAction. " + ex.Message, errorMessagesTextBox);
                }
            }
                
            return playerAction.Id;
        }

        #endregion

        #region Queries

        /// <summary>
        /// Query for the existance of a player in the database.
        /// </summary>
        /// <param name="playerName">The player's name</param>
        /// <returns>The player's Id if the player exists or -1 if the player does not exist</returns>
        public int QueryPlayerExists(string playerName)
        {
            Player player;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                player = ctx.Players.FirstOrDefault(p => p.Name.ToUpper() == playerName.ToUpper());
            }

            // if plr.Id > 0 the player exists, else the player does not exist
            if (player != null)
            {
                return player.Id;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Queries the database for hands that the player was involved in.
        /// </summary>
        /// <param name="playerId">The player's Id</param>
        /// <param name="maxNumRecords">The maximum number of records to pull from the database</param>
        /// <returns>A list of DbHandInfo objects. One DbHandInfo for each hand the player was involved in</returns>
        public List<DbHandInfo> QueryPlayerHandsInfo(int playerId, int maxNumRecords)
        {
            List<DbHandInfo> handInfoList;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                handInfoList = QueryPlayerHandsInfo(playerId, maxNumRecords, ctx);
            }

            return handInfoList;
        }

        /// <summary>
        /// Queries the database for hands that the player was involved in.
        /// </summary>
        /// <param name="playerId_">The player's Id</param>
        /// <param name="maxNumRecords_">The maximum number of records to pull from the database</param>
        /// <param name="ctx_">Database context</param>
        /// <returns>A list of DbHandInfo objects. One DbHandInfo for each hand the player was involved in</returns>
        public List<DbHandInfo> QueryPlayerHandsInfo(int playerId_, int maxNumRecords_, Bol_Model_DBEntities ctx_)
        {
            List<DbHandInfo> handInfoList = ctx_.HandPlayers
                    .Where(hp => hp.PlayerId == playerId_)
                    .OrderByDescending(hp => hp.Id)
                    .Take(maxNumRecords_)
                    .Select(hp => new DbHandInfo
                    {
                        HandId = hp.HandId,
                        GameId = hp.Hand.GameId,
                        Ante = hp.Hand.Ante,
                        HandPlayerId = hp.Id
                    }).ToList();

            return handInfoList;
        }

        /// <summary>
        /// Queries the database for a list of the complete set of information about a single player for a list of hands.
        /// </summary>
        /// <param name="handInfoList">A list of DbHandInfo objects representing a list of hands</param>
        /// <returns>A list of the complete set of information about a single player for a list of hands</returns>
        public List<DbPlayerHandInfoAll> QueryPlayerHandsInfoAll(List<DbHandInfo> handInfoList)
        {
            List<DbPlayerHandInfoAll> playerHandsInfoAllList = new List<DbPlayerHandInfoAll>();

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                playerHandsInfoAllList = QueryPlayerHandsInfoAll_query(handInfoList, ctx);
            }

            return playerHandsInfoAllList;
        }

        /// <summary>
        /// Queries the database for a list of the complete set of information about a single player for a list of hands.
        /// </summary>
        /// <param name="handInfoList_">A list of DbHandInfo objects representing a list of hands</param>
        /// <param name="ctx_">Database context</param>
        /// <returns>A list of the complete set of information about a single player for a list of hands</returns>
        public List<DbPlayerHandInfoAll> QueryPlayerHandsInfoAll_query(List<DbHandInfo> handInfoList_, Bol_Model_DBEntities ctx_)
        {
            List<DbPlayerHandInfoAll> playerHandsInfoAllList_ = new List<DbPlayerHandInfoAll>();

            // Add all the current HandInfo (handInfoList_ [HandIds and HandPlayerIds]) to the playerHandsInfoAllList_
            foreach (DbHandInfo hi in handInfoList_)
            {
                playerHandsInfoAllList_.Add(new DbPlayerHandInfoAll(hi));
            }

            // Create a list variable of each type so that it can be inserted into the list variable that will contain all 
            // the data (playerHandsInfoAllList_)
            List<DbHandPlayerInfo> handPlayerInfoList_;
            List<DbPlayerActionInfo> playerActionInfoList_;
            List<DbBoardActionInfo> boardActionInfoList_;

            foreach (DbPlayerHandInfoAll ia in playerHandsInfoAllList_)
            {
                // Get handPlayers info
                handPlayerInfoList_ = ctx_.HandPlayers
                    .Where(hp => hp.HandId == ia.HandInfo.HandId)
                    .OrderBy(hp => hp.Id)
                    .Select(hp => new DbHandPlayerInfo
                    {
                        HandPlayerId = hp.Id,
                        ChipCountStart = hp.ChipCountStart,
                        Blind = hp.Blind
                    }).ToList();

                // Get playerActions info
                playerActionInfoList_ = ctx_.PlayerActions
                    .Where(pa => pa.HandPlayer.HandId == ia.HandInfo.HandId)
                    .OrderBy(pa => pa.Id)
                    .Select(pa => new DbPlayerActionInfo
                    {
                        HandPlayerId = pa.HandPlayerId,
                        ChipCountChange = pa.ChipCountChange,
                        HandActionNumber = pa.HandActionNumber
                    }).ToList();

                // Get boardActions info
                boardActionInfoList_ = ctx_.BoardActions
                    .Where(ba => ba.HandId == ia.HandInfo.HandId)
                    .OrderBy(ba => ba.Id)
                    .Select(ba => new DbBoardActionInfo
                    {
                        HandActionNumber = ba.HandActionNumber
                    }).ToList();

                // Save the newly found handPlayers info, playerActions info and boardActions info to the current
                // element of the list variable that will contain all the data (lstPlayerHandsInfoAll)
                ia.HandPlayerInfoList = handPlayerInfoList_;
                ia.PlayerActionInfoList = playerActionInfoList_;
                ia.BoardActionInfoList = boardActionInfoList_;
            }

            return playerHandsInfoAllList_;
        }

        #endregion

        #region Updates
        
        /// <summary>
        /// Update the number of players in a game in the database.
        /// </summary>
        /// <param name="numPlayers">The number of players in the game</param>
        /// <param name="gameId">The game Id</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateGame_NumPlayers(int numPlayers, int gameId)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Game game = ctx.Games.Find(gameId);
                game.NumPlayers = numPlayers;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update the ante for a hand in the database.
        /// </summary>
        /// <param name="handId">The hand Id</param>
        /// <param name="ante">The ante for the hand</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateHand_Ante(long handId, int ante)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Hand hand = ctx.Hands.Find(handId);
                hand.Ante = ante;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update a player's name in the database.
        /// </summary>
        /// <param name="name">The player's name</param>
        /// <param name="id">The player's Id</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdatePlayer_Name(string name, int id)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Player plr = ctx.Players.Find(id);
                plr.Name = name;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update the player Id that is associated with a handPlayer Id.
        /// </summary>
        /// <param name="handPlayerId">The handPlayer Id</param>
        /// <param name="playerId">The new player Id</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateHandPlayer_Player(long handPlayerId, int playerId)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(handPlayerId);
                hpl.PlayerId = playerId;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update a handPlayer blind bet.
        /// </summary>
        /// <param name="handPlayerId">the handPlayer Id</param>
        /// <param name="blind">The new blind bet</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateHandPlayer_Blind(long handPlayerId, int blind)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(handPlayerId);
                hpl.Blind = blind;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update both of a handPlayer's hold cards
        /// </summary>
        /// <param name="handPlayerId">The handPlayer Id</param>
        /// <param name="holdCard1">The first hold card</param>
        /// <param name="holdCard2">The second hold card</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateHandPlayer_HcBoth(long handPlayerId, short holdCard1, short holdCard2)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(handPlayerId);
                hpl.HoldCard1 = holdCard1;
                hpl.HoldCard2 = holdCard2;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update a handPlayer's first hold card.
        /// </summary>
        /// <param name="handPlayerId">The handPlayer Id</param>
        /// <param name="holdCard1">The first hold card</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateHandPlayer_Hc1(long handPlayerId, short holdCard1)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(handPlayerId);
                hpl.HoldCard1 = holdCard1;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        /// <summary>
        /// Update a handPlayer's second hold card.
        /// </summary>
        /// <param name="handPlayerId">The handPlayer Id</param>
        /// <param name="holdCard2">The second hold card</param>
        /// <returns>True if the update was successful</returns>
        public bool UpdateHandPlayer_Hc2(long handPlayerId, short holdCard2)
        {
            int entitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(handPlayerId);
                hpl.HoldCard2 = holdCard2;
                entitiesWritten = ctx.SaveChanges();
            }

            return IsSuccessfulUpdate(entitiesWritten);
        }

        #endregion

        #region Deletes

        /// <summary>
        /// Delete a row representing a poker game from the database.
        /// </summary>
        /// <param name="gameId">The game Id</param>
        /// <returns>True if the delete was successful</returns>
        public bool DeleteGame(int gameId)
        {
            int entitiesDeleted;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Game gme = ctx.Games.Find(gameId);
                ctx.Games.Remove(gme);
                entitiesDeleted = ctx.SaveChanges();
            }

            return IsSuccessfulDelete(entitiesDeleted);
        }

        /// <summary>
        /// Delete a row representing a board action from the database.
        /// </summary>
        /// <param name="boardActionId">The board action Id</param>
        /// <returns>True if the delete was successful</returns>
        public bool DeleteBoardAction(long boardActionId)
        {
            int entitiesDeleted;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                BoardAction bda = ctx.BoardActions.Find(boardActionId);
                ctx.BoardActions.Remove(bda);
                entitiesDeleted = ctx.SaveChanges();
            }

            return IsSuccessfulDelete(entitiesDeleted);
        }

        #endregion

        #region Clear all database data

        /// <summary>
        /// Clear all the data in the database.
        /// </summary>
        /// <returns>True if the database data was cleared successfully</returns>
        public bool ClearAllDbData()
        {
            bool isSuccess = false;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                ctx.Database.ExecuteSqlCommand("delete from BoardAction");
                ctx.Database.ExecuteSqlCommand("dbcc CHECKIDENT (BoardAction, RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("delete from PlayerAction");
                ctx.Database.ExecuteSqlCommand("dbcc CHECKIDENT (PlayerAction, RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("delete from HandPlayer");
                ctx.Database.ExecuteSqlCommand("dbcc CHECKIDENT (HandPlayer, RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("delete from Hand");
                ctx.Database.ExecuteSqlCommand("dbcc CHECKIDENT (Hand, RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("delete from Player");
                ctx.Database.ExecuteSqlCommand("dbcc CHECKIDENT (Player, RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("delete from Game");
                ctx.Database.ExecuteSqlCommand("dbcc CHECKIDENT (Game, RESEED, 0)");

                isSuccess = true;
            }

            return isSuccess;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Write an error message in the errorMessages textbox on the main form.
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="errorMessages_">The error messages textbox</param>
        private void WriteErrorMessage(string message, TextBox errorMessages_)
        {
            if (errorMessages_ != null)
            {
                // If the errorMessages textbox is not empty skip a line
                if (errorMessages_.Text.Length != 0)
                {
                    errorMessages_.AppendText("\r\n");
                }

                errorMessages_.AppendText(message);
            }
        }

        /// <summary>
        /// Determine if a database update operation was successful.
        /// </summary>
        /// <param name="numEntitiesWritten">The number of entities successfully written when the database changes were saved</param>
        /// <returns>True if the update was successful</returns>
        private bool IsSuccessfulUpdate(int numEntitiesWritten)
        {
            return IsSuccessfulDatabaseOperation(numEntitiesWritten);
        }

        /// <summary>
        /// Determine if a database delete operation was successful
        /// </summary>
        /// <param name="numEntitiesDeleted">The number of entities successfully deleted when the database changes were saved</param>
        /// <returns>True if the delete was successful</returns>
        private bool IsSuccessfulDelete(int numEntitiesDeleted)
        {
            return IsSuccessfulDatabaseOperation(numEntitiesDeleted);
        }

        /// <summary>
        /// Determine if a general database operation was successful
        /// </summary>
        /// <param name="numEntitiesAffected">The number of entities successfully affected when the database changes were saved</param>
        /// <returns>True if the operation was successful</returns>
        private bool IsSuccessfulDatabaseOperation(int numEntitiesAffected)
        {
            if (numEntitiesAffected == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
