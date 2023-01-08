using System.Text;
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
            ////    return Task.FromResult(bytes);
            ////}

            ////var test = Encoding.UTF8.GetString(buffer.Take(bytes).ToArray());
            ////if (!test.Contains("Bank of Godfrey"))
            ////{
            ////    return Task.FromResult(bytes);
            ////}

            ////test.Replace("Bank of Godfrey", "Bank of Aylen The Magnificent");
            ////var stringBuilder = new StringBuilder();
            ////var parts = test.Split("Bank of Godfrey").ToList();
            ////parts.Insert(1, "Bank of Aylen the Magnificent");
            ////var newBuffer = Encoding.UTF8.GetBytes(string.Join(string.Empty, parts));
            ////for (int i = 0; i < newBuffer.Length; i++)
            ////{
            ////    buffer[i] = newBuffer[i];
            ////}

            ////return Task.FromResult(newBuffer.Length);
        }
    }
}
