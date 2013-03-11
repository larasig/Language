using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Lang.Pattern;

namespace Ditw.Test.Lang.Pattern
{
    public static class BuiltInExpressions
    {

        #region  与 ... 的关系
        internal static AndExpr RELATIONSHIP_WITH = new AndExpr(
            null,
            BuiltinPatterns.TOGETHER_WITH,
            LitExpr.FromString("的关系")
            );
        #endregion

        #region Test Acquisition
        private static String[] _STR_ACQUIRE = new String[]
        {
    "收购",
    "买下",
    "购得",
    "购入",
    "买入",
    "洽购",
        };
        private static String[] _STR_PARTIAL_ASSET = new String[]
        {
    "股份",
    "股票",
    "股权",
    "普通股",
    "部门",
    "流通股",
    "专利",
    "业务",
    "品牌",
    "项目",
        };
        internal static LitExpr _ACQUIRE = LitExpr.FromStrings(_STR_ACQUIRE);
        internal static NotExpr _NOT_PARTIAL_ASSET = new NotExpr(LitExpr.FromStrings(_STR_PARTIAL_ASSET));

        internal static AndExpr BEING_ACQUIRED = new AndExpr(
            null,
            LitExpr.FromString("被"),
            BuiltinPatterns.NOT_CLAUSE_SEPARATOR,
            _ACQUIRE
            );
        internal static AndExpr DUI_ACQUIRED = new AndExpr(
            null,
            LitExpr.FromString("对"),
            BuiltinPatterns.NOT_CLAUSE_SEPARATOR,
            _ACQUIRE
            );
        #endregion

        #region pay attention
        private static String[] _STR_PAY = new String[]
        {
            "Pay",
            "pay",
            "Paying",
            "paying",
            "Pays",
            "pays",
            "Paid",
            "paid",
        };
        internal static LitExpr PAY = LitExpr.FromStrings(_STR_PAY);
        internal static LitExpr ATTENTION = LitExpr.FromStrings(new String[] { "attention", "Attention" });

        internal static AndExpr PAY_ATTENTION = new AndExpr(
            null,
            PAY,
            ATTENTION
            );
        internal static AndExprN PAY_ATTENTION_1 = new AndExprN(
            PAY_ATTENTION,
            1
            );
        internal static AndExpr ATTENTION_PAY = new AndExpr(
            null,
            ATTENTION,
            PAY
            );
        #endregion
    }
}
