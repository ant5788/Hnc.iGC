namespace Hnc.iGC.Worker
{
    /// <summary>
    /// 运行状态
    /// </summary>
    public enum RunStates
    {
        RESET = 0, // 复位
        STOP,       // 自动运行停止状态 
        HOLD,       //自动运行暂停状态  进给保持
        START,      //自动运行启动状态  循环启动
        MSTR,       //手动数值指令启动状态
    }
}
