using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.OPCManager;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.OPCManager.Impl
{
    public class OPCService : IOPCService
    {
        private Repository<OPCServer> _opcServerDal;
        private Repository<OPCGroup> _opcGroupDal;
        private Repository<OPCTag> _opcTagDal;
        private UnitOfWork _unitOfWork;

        public OPCService(Repository<OPCServer> opcServerDal,Repository<OPCGroup> opcGroupDal,Repository<OPCTag> opcTagDal,UnitOfWork unitOfWork)
        {
            _opcServerDal = opcServerDal;
            _opcGroupDal = opcGroupDal;
            _opcTagDal = opcTagDal;
            _unitOfWork = unitOfWork;
        }

        #region opcserver
        public IEnumerable<OPCServer> GetAllOPCServer()
        {
            return _opcServerDal.GetAll().Entities;
        }

        public OPCServer GetOPCServer(string serverIP, string serverName)
        {
            return _opcServerDal.Single(os => os.opc_server_ip == serverIP && os.opc_server_name == serverName).Entity;
        }

        public OPCServer AddOPCServer(OPCServer newOPCServer)
        {
            try
            {
                _opcServerDal.Add(newOPCServer); 
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加opcserver出错", ex.InnerException);
            }
            return newOPCServer;
        }

        public OPCServer UpdateOPCServer(OPCServer newOPCServer)
        {
            try
            {
                _opcServerDal.Update(newOPCServer);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更新opcserver出错", ex.InnerException);
            }
            return newOPCServer;
        }
        #endregion

        #region opcgroup
        public OPCGroup GetOPCGroup(string groupCode)
        {
            return _opcGroupDal.Single(og => og.opc_group_code == groupCode).Entity;
        }

        public OPCGroup AddOPCGroup(OPCGroup newOPCGroup)
        {
            try
            {
                _opcGroupDal.Add(newOPCGroup);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加opcgroup出错", ex.InnerException);
            }
            return newOPCGroup;
        }

        public OPCGroup UpdateOPCGroup(OPCGroup newOPCGroup)
        {
            try
            {
                _opcGroupDal.Update(newOPCGroup);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更新opcgroup出错", ex.InnerException);
            }
            return newOPCGroup;
        }
        #endregion

        #region opctag
        public OPCTag GetOPCTag(string tagCode)
        {
            return _opcTagDal.Single(ot => ot.tag_code == tagCode).Entity;
        }

        public OPCTag AddOPCTag(OPCTag newOPCTag)
        {
            try
            {
                _opcTagDal.Add(newOPCTag);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加OPCTag出错", ex.InnerException);
            }
            return newOPCTag;
        }

        public OPCTag UpdateOPCTag(OPCTag newOPCTag)
        {
            try
            {
                _opcTagDal.Update(newOPCTag);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更新OPCTag出错", ex.InnerException);
            }
            return newOPCTag;
        }
        #endregion
    }
}
