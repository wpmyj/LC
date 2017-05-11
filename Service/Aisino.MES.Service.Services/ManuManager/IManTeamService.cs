using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IManTeamService
    {

        #region 班组部分
        /// 
        /// <param name="manTeamCode"></param>
        ManTeam GetManTeam(string manTeamCode);  

        IEnumerable<ManTeam> GetAllManTeam();
                
        /// 
        /// <param name="newManTeam"></param>
        ManTeam CreateManTeam(ManTeam newManTeam);

        /// 
        /// <param name="updateManTeam"></param>
        ManTeam UpdateManTeam(ManTeam updateManTeam);

        /// 
        /// <param name="delManTeam"></param>
        ManTeam DeleteManTeam(ManTeam delManTeam);
        
        /// 
        /// <param name="lstDelManTeam"></param>
        void DeleteManTeamList(List<ManTeam> lstDelManTeam);

        /// 
        /// <param name="manTeamCode"></param>
        bool CheckManTeamCodeExist(string manTeamCode);

        /// 
        /// <param name="manTeamName"></param>
        bool CheckManTeamNameExist(string manTeamName);

        #endregion

        #region 班组成员
        /// 
        /// <param name="manTeamUserId"></param>
        ManTeamUser GetManTeamUser(int manTeamUserId);

        IEnumerable<ManTeamUser> GetAllManTeamUser();

        IEnumerable<OrganizationEmployee> GetOneTeamUser(string teamCode);

        IEnumerable<OrganizationEmployee> GetFreedomUser();
      
        /// 
        /// <param name="teamCode"></param>
        void UpdateTeamUserList(string teamCode, IList<ManTeamUser> teamUserList);

        #endregion

        #region 生产日历
        IEnumerable<ManRestDay> SelectAllManRestDay(DateTime YearMonth);
        IEnumerable<ManRestDay> SelectAllManRestDay(int year);
        ManRestDay AddManRestDay(ManRestDay newManRestDay);
        void UpdateManRestDay(ManRestDay updManRestDay);
        void DelManRestDay(ManRestDay delManRestDay);

        bool CheckManRestDayCodeExist(string code);
        bool CheckManRestDayNameExist(string name); 
        #endregion

    }
}
