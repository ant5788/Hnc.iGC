using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Hnc.iGC
{
    public class ViKeyValidator : IDongleValidator
    {
        #region Define

        // 错误代码
        const long VIKEY_SUCCESS = 0x00000000; //成功
        const long VIKEY_ERROR_NO_VIKEY = 0x80000001; //没有找到ViKey加密锁
        const long VIKEY_ERROR_INVALID_PASSWORD = 0x80000002; //密码错误
        const long VIKEY_ERROR_NEED_FIND = 0x80000003; //请先查找加密锁
        const long VIKEY_ERROR_INVALID_INDEX = 0x80000004; //无效的句柄
        const long VIKEY_ERROR_INVALID_VALUE = 0x80000005; //数值错误
        const long VIKEY_ERROR_INVALID_KEY = 0x80000006; //秘钥无效
        const long VIKEY_ERROR_GET_VALUE = 0x80000007; //读取信息错误
        const long VIKEY_ERROR_SET_VALUE = 0x80000008; //设置信息错误
        const long VIKEY_ERROR_NO_CHANCE = 0x80000009; //没有机会
        const long VIKEY_ERROR_NO_TAUTHORITY = 0x8000000A; //权限不足
        const long VIKEY_ERROR_INVALID_ADDR_OR_SIZE = 0x8000000B; //地址或长度错误
        const long VIKEY_ERROR_RANDOM = 0x8000000C; //获取随机数错误
        const long VIKEY_ERROR_SEED = 0x8000000D; //获取种子错误
        const long VIKEY_ERROR_CONNECTION = 0x8000000E; //通信错误
        const long VIKEY_ERROR_CALCULATE = 0x8000000F; //算法或计算错误
        const long VIKEY_ERROR_MODULE = 0x80000010; //计数器错误
        const long VIKEY_ERROR_GENERATE_NEW_PASSWORD = 0x80000011; //产生密码错误
        const long VIKEY_ERROR_ENCRYPT_FAILED = 0x80000012; //加密数据错误
        const long VIKEY_ERROR_DECRYPT_FAILED = 0x80000013; //解密数据错误
        const long VIKEY_ERROR_ALREADY_LOCKED = 0x80000014; //ViKey加密锁已经被锁定
        const long VIKEY_ERROR_UNKNOWN_COMMAND = 0x80000015; //无效的命令
        const long VIKEY_ERROR_UNKNOWN_ERROR = 0xFFFFFFFF; //未知错误

        //ViKey加密狗类型  VikeyGetType返回值代表的类型
        const uint ViKeyAPP = 0;   //实用型加密狗ViKeyAPP
        const uint ViKeySTD = 1;   //标准型加密狗ViKeySTD
        const uint ViKeyNET = 2;   //网络型加密狗ViKeyNET
        const uint ViKeyPRO = 3;   //专业型加密狗ViKeyPRO     
        const uint ViKeyWEB = 4;   //身份认证型加密狗ViKeyWEB
        const uint ViKeyTIME = 5;  //时间型加密狗ViKeyTIME

        // 函数引用声明
        [DllImport("ViKey")]
        internal static extern uint VikeyFind(ref uint pdwCount);
        [DllImport("ViKey")]
        internal static extern uint VikeyGetHID(ushort Index, ref uint pdwHID);
        [DllImport("ViKey")]
        internal static extern uint VikeyGetType(ushort Index, ref uint pType);
        [DllImport("ViKey")]
        internal static extern uint VikeyUserLogin(ushort Index, Byte[] pUserPassword);
        [DllImport("ViKey")]
        internal static extern uint VikeyAdminLogin(ushort Index, Byte[] pAdminPassword);
        [DllImport("ViKey")]
        internal static extern uint VikeyResetPassword(ushort Index, Byte[] pNewUserPassword, Byte[] pNewAdminPassword);
        [DllImport("ViKey")]
        internal static extern uint VikeyLogoff(ushort Index);
        [DllImport("ViKey")]
        internal static extern uint VikeyReadData(ushort Index, ushort pStartAddress, ushort pBufferLength, Byte[] pBuffer);
        [DllImport("ViKey")]
        internal static extern uint VikeyWriteData(ushort Index, ushort pStartAddress, ushort pBufferLength, Byte[] pBuffer);
        [DllImport("ViKey")]
        internal static extern uint ViKeyRandom(ushort Index, ref ushort pReturn1, ref ushort pReturn2, ref ushort pReturn3, ref ushort pReturn4);
        [DllImport("ViKey")]
        internal static extern uint VikeySeed(ushort Index, ref uint pSeed, ref ushort pReturn1, ref ushort pReturn2, ref ushort pReturn3, ref ushort pReturn4);
        [DllImport("ViKey")]
        internal static extern uint VikeySetSoftIDString(ushort Index, Byte[] SoftIDString);
        [DllImport("ViKey")]
        internal static extern uint VikeyGetSoftIDString(ushort Index, Byte[] SoftIDString);
        [DllImport("ViKey")]
        internal static extern uint ViKeySetModule(ushort Index, ushort ModelueIndex, ushort pValue, ushort pDecrease);
        [DllImport("ViKey")]
        internal static extern uint ViKeyCheckModule(ushort Index, ushort ModelueIndex, ref ushort pIsZero, ref ushort pCanDecrase);
        [DllImport("ViKey")]
        internal static extern uint ViKeyDecraseModule(ushort Index, ushort ModelueIndex);

        [DllImport("ViKey")]
        internal static extern uint VikeySetPtroductName(ushort Index, Byte[] szName);
        [DllImport("ViKey")]
        internal static extern uint VikeyGetPtroductName(ushort Index, Byte[] szName);
        [DllImport("ViKey")]
        internal static extern uint VikeyGetTime(ushort Index, Byte[] pTime);

        #endregion

        private const string SoftIDString = "Hnc_iGC_";
        private const string DeviceName = "_iGC_Key";
        private const string DefaultUserPassword = "BDE2CXrc";

        private static void ThrowIfError(uint returnCode)
        {
            if (returnCode != 0)
            {
                throw new ValidationException($"加密狗错误，错误码: 0x{returnCode:X4}");
            }
        }

        public void Validate()
        {
            uint Count = 0;
            ThrowIfError(VikeyFind(ref Count));
            for (ushort index = 0; index < Count; index++)
            {
#if DEBUG1
                uint HID = 0;
                ThrowIfError(VikeyGetHID(index, ref HID));
                Console.WriteLine($"[仅在调试时显示] 加密狗HID: {HID}");
#endif

                ThrowIfError(VikeyUserLogin(index, Encoding.Default.GetBytes(DefaultUserPassword)));

                var softIDBuffer = new byte[32];
                ThrowIfError(VikeyGetSoftIDString(index, softIDBuffer));
                var softIDString = Encoding.ASCII.GetString(softIDBuffer).TrimEnd('\0');
                if (softIDString != SoftIDString)
                {
                    throw new ValidationException($"加密狗软件ID错误: {softIDString}");
                }

                var szProductName = new byte[32];
                ThrowIfError(VikeyGetPtroductName(index, szProductName));
                var deviceName = Encoding.Unicode.GetString(szProductName).TrimEnd('\0');
                if (deviceName != DeviceName)
                {
                    throw new ValidationException($"加密狗设备名称错误: {deviceName}");
                }

                ThrowIfError(VikeyLogoff(index));
            }
        }

    }
}
