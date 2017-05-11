using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.SysManager.Impl
{
    public class SysRightService : ISysRightService
    {
        private Repository<SysRight> _sysRightDal;
        private UnitOfWork _unitOfWork;

        public SysRightService(Repository<SysRight> sysRightDal, UnitOfWork unitOfWork)
        {
            _sysRightDal = sysRightDal;
            _unitOfWork = unitOfWork;
        }

        public IList<SysRight> SelectAllSysRight()
        {
            return _sysRightDal.GetAll().Entities.ToList();
        }

        public IList<SysRight> SelectUndelSysRight()
        {
            return _sysRightDal.GetAll().Entities.Where(d => d.right_deleted.Value == false).ToList();
        }

        public SysRight GetSysRight(int id)
        {
            var sysRight = _sysRightDal.Single(d => d.id == id);
            if (sysRight.HasValue)
            {
                return sysRight.Entity;
            }
            else
            {
                return null;
            }
        }

        public SysRight AddSysRight(SysRight newSysRight)
        {
            SysRight sysRight = null;
            try
            {
                _sysRightDal.Add(newSysRight);
                sysRight = newSysRight;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加权限信息失败！", ex);
            }
            return sysRight;
        }

        public SysRight UpdateSysRight(SysRight updSysRight)
        {
            SysRight sysRight = null;
            try
            {
                _sysRightDal.Update(updSysRight);
                sysRight = updSysRight;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改权限信息失败！", ex);
            }
            return sysRight;
        }

        public SysRight DeleteSysRight(SysRight delSysRight)
        {
            SysRight sysRight = null;
            try
            {
                delSysRight.right_deleted = true;
                _unitOfWork.AddAction(delSysRight, DataActions.Update);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除权限信息失败！", ex);
            }
            return sysRight;
        }

        public void DeleteSysRightList(List<SysRight> lstDelSysRight)
        {
            try
            {
                foreach (SysRight delSysRight in lstDelSysRight)
                {
                    if (delSysRight.right_deleted == true)
                    {
                        continue;
                    }
                    delSysRight.right_deleted = true;
                    _unitOfWork.AddAction(delSysRight, DataActions.Update);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除物料清单主档信息失败！", ex);
            }
        }

        public bool CheckRightCodeExist(string rightCode)
        {
            var sysRight = _sysRightDal.Single(d => d.right_code == rightCode);
            if (sysRight.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckRightNameExist(string rightName)
        {
            var sysRight = _sysRightDal.Single(d => d.right_name == rightName);
            if (sysRight.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
