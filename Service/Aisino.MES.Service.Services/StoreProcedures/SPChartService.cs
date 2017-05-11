using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.StoreProcedure.Procedures;
using Aisino.MES.Model.StoreProcedure;

namespace Aisino.MES.Service.StoreProcedures
{
    public class SPChartService
    {
        private Repository<ChartFoodCharacter> _getChartFoodCharacterDal;
        private Repository<ChartGoodsType> _getChartGoodsTypeDal;
        private Repository<ChartGoodsKind> _getChartGoodsKindDal;
        private Repository<ChartGoodsTrendByYear> _getChartGoodsTrendByYearDal;
        private Repository<ChartGoodsTypeTrendByYear> _getChartGoodsTypeTrendByYearDal;
        private Repository<ChartGoodsKindTrendByYear> _getChartGoodsKindTrendByYearDal;

        public SPChartService(Repository<ChartFoodCharacter> getChartFoodCharacterDal,
            Repository<ChartGoodsType> getChartGoodsTypeDal,
            Repository<ChartGoodsKind> getChartGoodsKindDal,
            Repository<ChartGoodsTrendByYear> getChartGoodsTrendByYearDal,
            Repository<ChartGoodsTypeTrendByYear> getChartGoodsTypeTrendByYearDal,
            Repository<ChartGoodsKindTrendByYear> getChartGoodsKindTrendByYearDal)
        {
            _getChartFoodCharacterDal = getChartFoodCharacterDal;
            _getChartGoodsTypeDal = getChartGoodsTypeDal;
            _getChartGoodsKindDal = getChartGoodsKindDal;
            _getChartGoodsTrendByYearDal = getChartGoodsTrendByYearDal;
            _getChartGoodsTypeTrendByYearDal = getChartGoodsTypeTrendByYearDal;
            _getChartGoodsKindTrendByYearDal = getChartGoodsKindTrendByYearDal;
        }

        public List<ChartFoodCharacterResultSet> GetChartFoodCharacter()
        {
            StoredProc<ChartFoodCharacter> getChartFoodCharacter = new StoredProc<ChartFoodCharacter>(typeof(ChartFoodCharacterResultSet));
            ResultsList results = _getChartFoodCharacterDal.CallStoredProc(getChartFoodCharacter, null);
            var r1 = results.ToList<ChartFoodCharacterResultSet>();
            return r1;
        }

        public List<ChartGoodsTypeResultSet> GetChartGoodsType()
        {
            StoredProc<ChartGoodsType> getChartGoodsType = new StoredProc<ChartGoodsType>(typeof(ChartGoodsTypeResultSet));
            ResultsList results = _getChartGoodsTypeDal.CallStoredProc(getChartGoodsType, null);
            var r1 = results.ToList<ChartGoodsTypeResultSet>();
            return r1;
        }

        public List<ChartGoodsKindResultSet> GetChartGoodsKind()
        {
            StoredProc<ChartGoodsKind> getChartGoodsKind = new StoredProc<ChartGoodsKind>(typeof(ChartGoodsKindResultSet));
            ResultsList results = _getChartGoodsKindDal.CallStoredProc(getChartGoodsKind, null);
            var r1 = results.ToList<ChartGoodsKindResultSet>();
            return r1;
        }

        public List<ChartGoodsTrendByYearResultSet> GetChartGoodsTrendByYear(int inoutType, string startDate, string endDate)
        {
            StoredProc<ChartGoodsTrendByYear> getChartGoodsTrendByYear = new StoredProc<ChartGoodsTrendByYear>(typeof(ChartGoodsTrendByYearResultSet));
            ChartGoodsTrendByYear pams1 = new ChartGoodsTrendByYear();
            pams1.flag = inoutType;
            pams1.begin_time = startDate;
            pams1.end_time = endDate;
            ResultsList results = _getChartGoodsTrendByYearDal.CallStoredProc(getChartGoodsTrendByYear, pams1);
            var r1 = results.ToList<ChartGoodsTrendByYearResultSet>();
            return r1;
        }

        public List<ChartGoodsTypeTrendByYearResultSet> GetChartGoodsTypeTrendByYear(int inoutType,int goodsType, string startDate, string endDate)
        {
            StoredProc<ChartGoodsTypeTrendByYear> getChartGoodsTypeTrendByYear = new StoredProc<ChartGoodsTypeTrendByYear>(typeof(ChartGoodsTypeTrendByYearResultSet));
            ChartGoodsTypeTrendByYear pams1 = new ChartGoodsTypeTrendByYear();
            pams1.flag = inoutType;
            pams1.goods_type = goodsType;
            pams1.begin_time = startDate;
            pams1.end_time = endDate;
            ResultsList results = _getChartGoodsTypeTrendByYearDal.CallStoredProc(getChartGoodsTypeTrendByYear, pams1);
            var r1 = results.ToList<ChartGoodsTypeTrendByYearResultSet>();
            return r1;
        }

        public List<ChartGoodsKindTrendByYearResultSet> GetChartGoodsKindTrendByYear(int inoutKind, int goodsKind, string startDate, string endDate)
        {
            StoredProc<ChartGoodsKindTrendByYear> getChartGoodsKindTrendByYear = new StoredProc<ChartGoodsKindTrendByYear>(typeof(ChartGoodsKindTrendByYearResultSet));
            ChartGoodsKindTrendByYear pams1 = new ChartGoodsKindTrendByYear();
            pams1.flag = inoutKind;
            pams1.goods_kind = goodsKind;
            pams1.begin_time = startDate;
            pams1.end_time = endDate;
            ResultsList results = _getChartGoodsKindTrendByYearDal.CallStoredProc(getChartGoodsKindTrendByYear, pams1);
            var r1 = results.ToList<ChartGoodsKindTrendByYearResultSet>();
            return r1;
        }
    }
}
