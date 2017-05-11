using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.OPCManager;

namespace Aisino.MES.Service.OPCManager
{
    public interface IOPCService
    {
        #region opcserver
        IEnumerable<OPCServer> GetAllOPCServer();
        OPCServer GetOPCServer(string serverIP, string serverName);

        OPCServer AddOPCServer(OPCServer newOPCServer);
        OPCServer UpdateOPCServer(OPCServer newOPCServer);
        #endregion

        #region opcgroup
        OPCGroup GetOPCGroup(string groupCode);

        OPCGroup AddOPCGroup(OPCGroup newOPCGroup);
        OPCGroup UpdateOPCGroup(OPCGroup newOPCGroup);
        #endregion

        #region opctag
        OPCTag GetOPCTag(string tagCode);

        OPCTag AddOPCTag(OPCTag newOPCTag);
        OPCTag UpdateOPCTag(OPCTag newOPCTag);
        #endregion
    }
}
