using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    class DbController
    {
        private frmMain frm;

        public DbController()
        {

        }

        public DbController(frmMain frmIn)
        {
            frm = frmIn;
        }

        #region Get DB Context

        public Bol_Model_DBEntities GetDbContext()
        {
            return new Bol_Model_DBEntities();
        }

        #endregion

        #region Insert Statements

        public int InsertGame(bool blnTournament, int intNumPlayers)
        {
            int intRecordsWritten;

            Game gme = new Game
            {
                Tournament = blnTournament,
                NumPlayers = intNumPlayers,
                DateTimeStart = DateTime.Now
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.Games.Add(gme);
                    intRecordsWritten = ctx.SaveChanges();

                    if (intRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertGame. " + intRecordsWritten + " Records Written");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertGame. " + ex.Message);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertGame. " + ex.Message);
                }
            }

            return gme.Id;
        }

        public long InsertHand(int intGameId, int intAnte)
        {
            int intRecordsWritten;

            Hand hnd = new Hand
            {
                GameId = intGameId,
                Ante = intAnte,
                DateTimeStart = DateTime.Now
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.Hands.Add(hnd);
                    intRecordsWritten = ctx.SaveChanges();

                    if (intRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertHand. " + intRecordsWritten + " Records Written");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertHand. " + ex.Message);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertHand. " + ex.Message);
                }
            }

            return hnd.Id;
        }

        public int InsertPlayer(string strName)
        {
            int intRecordsWritten;

            Player plr = new Player
            {
                Name = strName
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.Players.Add(plr);
                    intRecordsWritten = ctx.SaveChanges();

                    if (intRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertPlayer. " + intRecordsWritten + " Records Written");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertPlayer. " + ex.Message);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertPlayer. " + ex.Message);
                }
            }
                
            return plr.Id;
        }

        public long InsertHandPlayer(long lngHandId, int intPlayerId, int intChipCountStart,
            int intBlind, short shtHoldCard1, short shtHoldCard2)
        {
            int intRecordsWritten;

            HandPlayer hpl = new HandPlayer
            {
                HandId = lngHandId,
                PlayerId = intPlayerId,
                ChipCountStart = intChipCountStart,
                Blind = intBlind,
                HoldCard1 = shtHoldCard1,
                HoldCard2 = shtHoldCard2
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.HandPlayers.Add(hpl);
                    intRecordsWritten = ctx.SaveChanges();

                    if (intRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertHandPlayer. " + intRecordsWritten + " Records Written");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertHandPlayer. " + ex.Message);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertHandPlayer. " + ex.Message);
                }
            }

            return hpl.Id;
        }

        public long InsertBoardAction(long lngHandId, short shtBoardCard, int intHandActionNumber)
        {
            int intRecordsWritten;

            BoardAction bda = new BoardAction
            {
                HandId = lngHandId,
                BoardCard = shtBoardCard,
                HandActionNumber = intHandActionNumber
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.BoardActions.Add(bda);
                    intRecordsWritten = ctx.SaveChanges();

                    if (intRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertBoardAction. " + intRecordsWritten + " Records Written");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertBoardAction. " + ex.Message);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertBoardAction. " + ex.Message);
                }
            }   

            return bda.Id;
        }

        public long InsertPlayerAction(long lngHandPlayerId, int intChipCountChange,
            int intHandActionNumber)
        {
            int intRecordsWritten;

            PlayerAction pla = new PlayerAction
            {
                HandPlayerId = lngHandPlayerId,
                ChipCountChange = intChipCountChange,
                HandActionNumber = intHandActionNumber
            };

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                try
                {
                    ctx.PlayerActions.Add(pla);
                    intRecordsWritten = ctx.SaveChanges();

                    if (intRecordsWritten != 1)
                    {
                        // ctx.SaveChanges() returns the number of records written to the DB. This method only writes one record at a time so if
                        // the number of records written is != 1 there was a problem
                        WriteErrorMessage("Wrong number of records written inside InsertPlayerAction. " + intRecordsWritten + " Records Written");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    WriteErrorMessage("DbUpdateConcurrencyException inside InsertPlayerAction. " + ex.Message);
                }
                catch (Exception ex)
                {
                    WriteErrorMessage("Some unexpected exception occured inside InsertPlayerAction. " + ex.Message);
                }
            }
                
            return pla.Id;
        }

        #endregion

        #region Queries

        public int QueryPlayerExists(string strName)
        {
            Player plr;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                plr = ctx.Players.FirstOrDefault(p => p.Name.ToUpper() == strName.ToUpper());
            }

            // if plr.Id > 0 the player exists, else the player does not exist
            if (plr != null)
            {
                return plr.Id;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Queries the DB for hands that this player was involved in.
        /// </summary>
        /// <param name="intPlayerId">The DB Player ID of the player of interest</param>
        /// <param name="intMaxNumRecords">The maximum number of records to pull from the DB</param>
        /// <returns></returns>
        public List<DbHandInfo> QueryPlayerHandInfo(int intPlayerId, int intMaxNumRecords)
        {
            List<DbHandInfo> lstHandInfo;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                lstHandInfo = QueryPlayerHandInfo_query(intPlayerId, intMaxNumRecords, ctx);
            }

            // System.Diagnostics.Debug.WriteLine("This many hands (" + intPlayerId.ToString() + ") --> " + lstHandInfo.Count);
            return lstHandInfo;
        }

        /// <summary>
        /// Queries the DB for hands that this player was involved in.
        /// </summary>
        /// <param name="intPlayerId">The DB Player ID of the player of interest</param>
        /// <param name="intMaxNumRecords">The maximum number of records to pull from the DB</param>
        /// <param name="ctx_">Bol_Model_DBEntities context</param>
        /// <returns></returns>
        public List<DbHandInfo> QueryPlayerHandInfo_query(int intPlayerId_, int intMaxNumRecords_, Bol_Model_DBEntities ctx_)
        {
            List<DbHandInfo> lstHandInfo_;

            lstHandInfo_ = ctx_.HandPlayers
                    .Where(hp => hp.PlayerId == intPlayerId_)
                    .OrderByDescending(hp => hp.Id)
                    .Take(intMaxNumRecords_)
                    .Select(hp => new DbHandInfo
                    {
                        lngHandId = hp.HandId,
                        intGameId = hp.Hand.GameId,
                        intAnte = hp.Hand.Ante,
                        lngHandPlayerId = hp.Id
                    }).ToList();

            // System.Diagnostics.Debug.WriteLine("This many hands (" + intPlayerId.ToString() + ") --> " + lstHandInfo.Count);
            return lstHandInfo_;
        }

        public List<DbPlayerHandInfoAll> QueryPlayerHandsInfoAll(List<DbHandInfo> lstHandInfo)
        {
            // Create the list variable that will contain all the data
            List<DbPlayerHandInfoAll> lstPlayerHandsInfoAll = new List<DbPlayerHandInfoAll>();

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                lstPlayerHandsInfoAll = QueryPlayerHandsInfoAll_query(lstHandInfo, ctx);
            }

            return lstPlayerHandsInfoAll;
        }

        public List<DbPlayerHandInfoAll> QueryPlayerHandsInfoAll_query(List<DbHandInfo> lstHandInfo_, Bol_Model_DBEntities ctx_)
        {
            // Create the list variable that will contain all the data
            List<DbPlayerHandInfoAll> lstPlayerHandsInfoAll_ = new List<DbPlayerHandInfoAll>();

            // Add all the current HandInfo (lstHandInfo [HandIds and HandPlayerIds]) to the lstPlayerHandsInfoAll
            foreach (DbHandInfo hi in lstHandInfo_)
            {
                lstPlayerHandsInfoAll_.Add(new DbPlayerHandInfoAll(hi));
            }

            // Creat a list variable of each type so that it can be inserted into the list variable that will contain all 
            // the data (lstPlayerHandsInfoAll)
            List<DbHandPlayerInfo> lstHandPlayerInfo_;
            List<DbPlayerActionInfo> lstPlayerActionInfo_;
            List<DbBoardActionInfo> lstBoardActionInfo_;

            foreach (DbPlayerHandInfoAll ia in lstPlayerHandsInfoAll_)
            {
                // Get handPlayers info
                lstHandPlayerInfo_ = ctx_.HandPlayers
                    .Where(hp => hp.HandId == ia.HandInfo.lngHandId)
                    .OrderBy(hp => hp.Id)
                    .Select(hp => new DbHandPlayerInfo
                    {
                        lngHandPlayerId = hp.Id,
                        intChipCountStart = hp.ChipCountStart,
                        intBlind = hp.Blind
                    }).ToList();

                // Get playerActions info
                lstPlayerActionInfo_ = ctx_.PlayerActions
                    .Where(pa => pa.HandPlayer.HandId == ia.HandInfo.lngHandId)
                    .OrderBy(pa => pa.Id)
                    .Select(pa => new DbPlayerActionInfo
                    {
                        lngHandPlayerId = pa.HandPlayerId,
                        intChipCountChange = pa.ChipCountChange,
                        intHandActionNumber = pa.HandActionNumber
                    }).ToList();

                // Get boardActions info
                lstBoardActionInfo_ = ctx_.BoardActions
                    .Where(ba => ba.HandId == ia.HandInfo.lngHandId)
                    .OrderBy(ba => ba.Id)
                    .Select(ba => new DbBoardActionInfo
                    {
                        intHandActionNumber = ba.HandActionNumber
                    }).ToList();

                // Save the newly found handPlayers info, playerActions info and boardActions info to the current
                // element of the list variable that will contain all the data (lstPlayerHandsInfoAll)
                ia.lstHandPlayerInfo = lstHandPlayerInfo_;
                ia.lstPlayerActionInfo = lstPlayerActionInfo_;
                ia.lstBoardActionInfo = lstBoardActionInfo_;
            }

            return lstPlayerHandsInfoAll_;
        }

        #endregion

        #region Updates

        public bool UpdateGme_NumPlayers(int intNumPlayers, int intGmeId)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Game gme = ctx.Games.Find(intGmeId);
                gme.NumPlayers = intNumPlayers;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdateHand_Ante(long lngHandId, int intAnte)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Hand hnd = ctx.Hands.Find(lngHandId);
                hnd.Ante = intAnte;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdatePlayer_Name(string strName, int intId)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Player plr = ctx.Players.Find(intId);
                plr.Name = strName;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdateHandPlayer_Player(long lngHandPlayerId, int intPlayerId)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(lngHandPlayerId);
                hpl.PlayerId = intPlayerId;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdateHandPlayer_Blind(long lngHandPlayerId, int intBlind)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(lngHandPlayerId);
                hpl.Blind = intBlind;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdateHandPlayer_HcBoth(long lngHandPlayerId, short shtHoldCard1, short shtHoldCard2)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(lngHandPlayerId);
                hpl.HoldCard1 = shtHoldCard1;
                hpl.HoldCard2 = shtHoldCard2;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdateHandPlayer_Hc1(long lngHandPlayerId, short shtHoldCard1)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(lngHandPlayerId);
                hpl.HoldCard1 = shtHoldCard1;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool UpdateHandPlayer_Hc2(long lngHandPlayerId, short shtHoldCard2)
        {
            bool blnSuccess = false;
            int intEntitiesWritten;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                HandPlayer hpl = ctx.HandPlayers.Find(lngHandPlayerId);
                hpl.HoldCard2 = shtHoldCard2;
                intEntitiesWritten = ctx.SaveChanges();
            }

            if (intEntitiesWritten == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        #endregion

        #region Deletes

        public bool DeleteGme(int intGmeId)
        {
            bool blnSuccess = false;
            int intEntitiesDeleted;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                Game gme = ctx.Games.Find(intGmeId);
                ctx.Games.Remove(gme);
                intEntitiesDeleted = ctx.SaveChanges();
            }

            if (intEntitiesDeleted == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        public bool DeleteBoardAction(long lngBrdActionId)
        {
            bool blnSuccess = false;
            int intEntitiesDeleted;

            using (Bol_Model_DBEntities ctx = new Bol_Model_DBEntities())
            {
                BoardAction bda = ctx.BoardActions.Find(lngBrdActionId);
                ctx.BoardActions.Remove(bda);
                intEntitiesDeleted = ctx.SaveChanges();
            }

            if (intEntitiesDeleted == 1)
            {
                blnSuccess = true;
            }

            return blnSuccess;
        }

        #endregion

        #region Clear all database data

        public bool ClearAllDbData()
        {
            bool blnSucess = false;

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

                blnSucess = true;
            }

            return blnSucess;
        }

        #endregion

        #region Classes

        public class DbHandInfo
        {
            public long lngHandId;
            public int intGameId;
            public int intAnte;
            public long lngHandPlayerId;
        }

        public class DbHandPlayerInfo
        {
            public long lngHandPlayerId;
            public int intChipCountStart;
            public int intBlind;
        }

        public class DbPlayerActionInfo
        {
            public long lngHandPlayerId;
            public int intChipCountChange;
            public int intHandActionNumber;
        }

        public class DbBoardActionInfo
        {
            public int intHandActionNumber;
        }

        public class DbPlayerHandInfoAll
        {
            public DbHandInfo HandInfo;
            public List<DbHandPlayerInfo> lstHandPlayerInfo;
            public List<DbPlayerActionInfo> lstPlayerActionInfo;
            public List<DbBoardActionInfo> lstBoardActionInfo;

            public DbPlayerHandInfoAll(DbHandInfo HandInfoIn)
            {
                HandInfo = HandInfoIn;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Writes an error message on frmMain in the txtErrorMessages textbox
        /// </summary>
        /// <param name="strMessage"></param>
        private void WriteErrorMessage(string strMessage)
        {
            if (frm != null)
            {
                if (frm.txtErrorMessages.Text.Length != 0)
                {
                    frm.txtErrorMessages.AppendText("\r\n");
                }
                frm.txtErrorMessages.AppendText(strMessage);
            }
        }

        #endregion

    }
}
