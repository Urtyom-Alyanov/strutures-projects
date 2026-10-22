using System.Text;

using AiEngine.Native.Interop;

namespace AiEngine.Native;

public enum Status : byte
{
    Ok = 0,
    NullPointer = 1,
    InvalidArgument = 2,
    DimensionMismatch = 3,
    InvalidStructure = 4,
    Panic = 100
}

public class AiEngineException(uint errorCode, string message) : Exception($"[Error {errorCode}] {message}")
{
    public Status ErrorCode { get; } = (Status)errorCode;

    /// <summary>
    /// Проверка кода статука, полученного из Rust-бибилиотеки
    /// </summary>
    public static unsafe void CheckResult(uint statusCode)
    {
        if (statusCode == 0) return;

        byte[] buf = new byte[1024];
        uint len;
        fixed (byte* pBuf = buf)
        {
            len = NativeMethods.ai_last_error(pBuf, (uint)buf.Length);
        }

        string errorMsg = len > 0
            ? Encoding.UTF8.GetString(buf, 0, (int)Math.Min(len, buf.Length))
            : "Unknown error";

        throw new AiEngineException(statusCode, errorMsg);
    }
}