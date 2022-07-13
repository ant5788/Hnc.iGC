namespace Hnc.iGC
{
    public interface IDongleValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ValidationException">Dongle Validation fail</exception>
        void Validate();
    }
}
