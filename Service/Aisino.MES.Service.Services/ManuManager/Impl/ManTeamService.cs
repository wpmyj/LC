using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager.Impl;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class ManTeamService : IManTeamService
    {
        #region 属性
        private Repository<ManTeam> _manTeamDal;
        private Repository<ManTeamUser> _manTeamUserDal;
        private Repository<OrganizationEmployee> _sysUserDal;
        private Repository<ManRestDay> _manRestDayDal;
        private UnitOfWork _unitOfWork;


        public ManTeamService(Repository<ManTeam> manTeamDal,
                                Repository<ManTeamUser> manTeamUserDal,
                                Repository<OrganizationEmployee> sysUserDal,
                                Repository<ManRestDay> manRestDayDal,
                                UnitOfWork unitOfWork)
        {
            _manTeamDal = manTeamDal;
            _manTeamUserDal = manTeamUserDal;
            _sysUserDal = sysUserDal;
            _manRestDayDal = manRestDayDal;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region 班组部分
        public IEnumerable<ManTeam> GetAllManTeam()
        {
            return _manTeamDal.GetAll().Entities.ToList();
        }

        public ManTeam GetManTeam(string manTeamCode)
        {
            var equipmentType = _manTeamDal.Single(s => s.man_team_code == manTeamCode);
            if (equipmentType.HasValue)
            {
                return equipmentType.Entity;
            }
            else
            {
                return null;
            }
        }

        /// 
        /// <param name="manTeamCode"></param>
        public bool CheckManTeamCodeExist(string manTeamCode)
        {
            var manTeam = _manTeamDal.Single(s => s.man_team_code == manTeamCode);
            if (manTeam.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// 
        /// <param name="manTeamName"></param>
        public bool CheckManTeamNameExist(string manTeamName)
        {
            var manTeam = _manTeamDal.Single(s => s.man_team_name == manTeamName);
            if (manTeam.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// 
        /// <param name="newManTeam"></param>
        public ManTeam CreateManTeam(ManTeam newManTeam)
        {
            ManTeam returnManTeam = null;
            try
            {
                //保存
                //_unitOfWork.AddAction(newManTeam, DataActions.Add);
                //_unitOfWork.Save();

                _manTeamDal.Add(newManTeam);

                returnManTeam = newManTeam;

            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnManTeam;
        }


        /// 
        /// <param name="updateManTeam"></param>
        public ManTeam UpdateManTeam(ManTeam updateManTeam)
        {
            ManTeam returnManTeam = null;
            try
            {
                _manTeamDal.Update(updateManTeam);
                returnManTeam = updateManTeam;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnManTeam;

        }


        /// 
        /// <param name="delManTeam"></param>
        public ManTeam DeleteManTeam(ManTeam delManTeam)
        {
            ManTeam returnManTeam = null;
            try
            {

                //删除班组下的所有成员
                foreach (ManTeamUser manTeamUser in delManTeam.ManTeamUsers)
                {
                    _unitOfWork.AddAction(manTeamUser, DataActions.Delete);
                }

                _unitOfWork.AddAction(delManTeam, DataActions.Delete);

                _unitOfWork.Save();


                returnManTeam = delManTeam;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnManTeam;
        }

        /// <summary>
        /// 批量删除班组
        /// </summary>
        /// <param name="lstDelManTeam"></param>
        public void DeleteManTeamList(List<ManTeam> lstDelManTeam)
        {
            try
            {
                foreach (ManTeam delManTeam in lstDelManTeam)
                {
                    //删除班组下的所有成员
                    foreach (ManTeamUser manTeamUser in delManTeam.ManTeamUsers)
                    {
                        _unitOfWork.AddAction(manTeamUser, DataActions.Delete);
                    }

                    _unitOfWork.AddAction(delManTeam, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除班组信息失败！", ex);
            }
        }
        #endregion

        #region 班组成员

        public IEnumerable<ManTeamUser> GetAllManTeamUser()
        {
            return _manTeamUserDal.GetAll().Entities.ToList();
        }

        public IEnumerable<OrganizationEmployee> GetOneTeamUser(string manTeamCode)
        {
            return _manTeamUserDal.Find(m => m.man_team_code == manTeamCode).Entities.Select(w => w.OrganizationEmployee);
             
        }

        public IEnumerable<OrganizationEmployee> GetFreedomUser()
        {
            IEnumerable<ManTeamUser> AllManTeamUser = GetAllManTeamUser();
            IEnumerable<OrganizationEmployee> allTeamUser = AllManTeamUser.Select(mtu => mtu.OrganizationEmployee);
            return _sysUserDal.GetAllEntity().Except(allTeamUser);
        }

        public ManTeamUser GetManTeamUser(int manTeamUserId)
        {

            var equipment = _manTeamUserDal.Single(s => s.user_id == manTeamUserId);
            if (equipment.HasValue)
            {
                return equipment.Entity;
            }
            else
            {
                return null;
            }
        }

        public void UpdateTeamUserList(string teamCode, IList<ManTeamUser> teamUserList)
        {
            try
            {
                IList<ManTeamUser> oldManTeamUser = _manTeamUserDal.Find(s => s.man_team_code == teamCode).Entities.ToList();
                //原先没有的，现在要添加
                if (oldManTeamUser != null && oldManTeamUser.Count > 0)
                {
                    foreach (ManTeamUser teamUser in teamUserList)
                    {
                        if (!oldManTeamUser.Any(old => old.user_id == teamUser.user_id && old.man_team_code == teamUser.man_team_code))
                        {
                            _unitOfWork.AddAction(teamUser, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (ManTeamUser teamUser in teamUserList)
                    {
                        _unitOfWork.AddAction(teamUser, DataActions.Add);
                    }
                }

                //原先有的，现在没了，要删除
                if (oldManTeamUser != null)
                {
                    foreach (ManTeamUser teamUser in oldManTeamUser.Where(old => !teamUserList.Any(list => list.man_team_code == old.man_team_code && list.user_id == old.user_id)))
                    {
                        _unitOfWork.AddAction(teamUser, DataActions.Delete);
                    }
                }

                _unitOfWork.Save();

            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 日历

        public IEnumerable<ManRestDay> SelectAllManRestDay(DateTime YearMonth)
        {
            //return _manRestDayDal.Find(date => date.man_restday_date.Value.Year == YearMonth.Year
                                            //&& date.man_restday_date.Value.Month == YearMonth.Month).Entities;

            //显示前一个月，当前月，后一个月 
            return _manRestDayDal.Find(date => date.man_restday_date.Value.Year == YearMonth.Year
                                    && (date.man_restday_date.Value.Month >= YearMonth.Month - 1 && date.man_restday_date.Value.Month <= YearMonth.Month + 1 )).Entities;
        
        
        }

        public IEnumerable<ManRestDay> SelectAllManRestDay(int year)
        {
            return _manRestDayDal.Find(date => date.man_restday_date.Value.Year == year).Entities;
        }

        public ManRestDay AddManRestDay(ManRestDay newManRestDay)
        {
            try
            {
                _manRestDayDal.Add(newManRestDay);
                return newManRestDay;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void UpdateManRestDay(ManRestDay updManRestDay)
        {
            try
            {
                _manRestDayDal.Update(updManRestDay);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelManRestDay(ManRestDay delManRestDay)
        {
            try
            {
                _manRestDayDal.Delete(delManRestDay);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public bool CheckManRestDayCodeExist(string code)
        {
            var manRestDay = _manRestDayDal.Single(m => m.man_restday_code == code);
            if (manRestDay.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckManRestDayNameExist(string name)
        {
            var manRestDay = _manRestDayDal.Single(m => m.man_restday_name == name);
            if (manRestDay.HasValue)
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
