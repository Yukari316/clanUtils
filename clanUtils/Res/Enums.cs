namespace clanUtils.Res
{
    internal enum AttackType
    {
        /// <summary>
        /// 非法刀
        /// </summary>
        Illeage = -1,
        /// <summary>
        /// 普通刀
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 尾刀
        /// </summary>
        Final = 1,
        /// <summary>
        /// 过度伤害
        /// </summary>
        FinalOutOfRange = 2,
        /// <summary>
        /// 补时刀
        /// </summary>
        Compensate = 3,
        /// <summary>
        /// 掉刀
        /// </summary>
        Offline = 4,
        /// <summary>
        /// 补时刀击杀
        /// </summary>
        CompensateKill = 5
    }

    internal enum FlagType
    {
        /// <summary>
        /// 未知成员
        /// </summary>
        UnknownMember = -1,
        /// <summary>
        /// 空闲
        /// </summary>
        IDLE = 0,
        /// <summary>
        /// 出刀中
        /// </summary>
        EnGage = 1,
        /// <summary>
        /// 在树上
        /// </summary>
        OnTree = 3
    }

    #region 区服标识
    internal enum Server
    {
        CN = 0,
        TW = 1,
        JP = 2
    }
    #endregion
}
