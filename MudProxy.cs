using static telnet_proxy.TelnetProxyBroker;

namespace telnet_proxy
{
    internal class MudProxy : ITelnetProxyInterceptor
    {
        public int Intercept(ref byte[] buffer, Direction direction, int bytes)
        {
            return bytes;
            // Just some sample code
            ////if (direction == Direction.Outbound)
            ////{
            ////    return bytes;
            ////}

            ////var test = System.Text.Encoding.UTF8.GetString(buffer.Take(bytes).ToArray());
            ////if (!test.Contains("Bank of Godfrey"))
            ////{
            ////    return bytes;
            ////}

            ////var parts = test.Split("Bank of Godfrey").ToList();
            ////parts.Insert(1, "Bank of Aylen the Magnificent");
            ////var newBuffer = System.Text.Encoding.UTF8.GetBytes(string.Join(string.Empty, parts));
            ////for (int i = 0; i < newBuffer.Length; i++)
            ////{
            ////    buffer[i] = newBuffer[i];
            ////}

            ////return newBuffer.Length;
        }
    }
}
