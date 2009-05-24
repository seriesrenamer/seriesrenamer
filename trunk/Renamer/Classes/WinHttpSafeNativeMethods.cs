using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Renamer.Classes
{
    /// <summary>
    /// Some network function for faster automatic proxy detection
    /// </summary>
    static class WinHttpSafeNativeMethods
    {

        internal static IEnumerable<Uri> GetProxiesForUrl(
                Uri requestUrl) {
            return GetProxiesForUrl(requestUrl, string.Empty);
        }

        internal static IEnumerable<Uri> GetProxiesForUrl(
                Uri requestUrl, string userAgent) {

            IntPtr hHttpSession = IntPtr.Zero;
            string[] proxyList = null;
            ;

            try {
                hHttpSession = WinHttpOpen(userAgent,
                        AccessType.NoProxy, null, null, 0);

                if (hHttpSession != IntPtr.Zero) {

                    AutoProxyOptions autoProxyOptions = new AutoProxyOptions();
                    autoProxyOptions.Flags = AccessType.AutoDetect;
                    autoProxyOptions.AutoLogonIfChallenged = true;
                    autoProxyOptions.AutoDetectFlags =
                            AutoDetectType.Dhcp | AutoDetectType.DnsA;

                    ProxyInfo proxyInfo = new ProxyInfo();

                    if (WinHttpGetProxyForUrl(hHttpSession,
                            requestUrl.ToString(), ref autoProxyOptions, ref proxyInfo)) {
                        if (!string.IsNullOrEmpty(proxyInfo.Proxy)) {
                            proxyList = proxyInfo.Proxy.Split(';', ' ');
                        }
                    }
                }
            }
            catch (System.DllNotFoundException) {
                // winhttp.dll is not found. 
            }
            catch (System.EntryPointNotFoundException) {
                // A method within winhttp.dll is not found. 
            }
            finally {
                if (hHttpSession != IntPtr.Zero) {
                    WinHttpCloseHandle(hHttpSession);
                    hHttpSession = IntPtr.Zero;
                }
            }

            if (proxyList != null && proxyList.Length > 0) {
                Uri proxyUrl;
                foreach (string address in proxyList) {
                    if (TryCreateUrlFromPartialAddress(address, out proxyUrl)) {
                        yield return proxyUrl;
                    }
                }
            }
        }

        /// <summary>
        /// Some network function for faster automatic proxy detection
        /// </summary>
        private static bool TryCreateUrlFromPartialAddress(string address, out Uri url) {
            address = address.Trim();

            if (string.IsNullOrEmpty(address)) {
                url = null;
                return false;
            }

            try {
                if (address.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        address.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) {
                    url = new Uri(address);
                }
                else if (address.StartsWith("//", StringComparison.Ordinal)) {
                    url = new Uri("http:" + address);
                }
                else {
                    url = new Uri("http://" + address);
                }
                return true;
            }
            catch (UriFormatException) {
                url = null;
            }
            return false;
        }

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr WinHttpOpen(
                string userAgent,
                AccessType accessType,
                string proxyName,
                string proxyBypass,
                int flags);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WinHttpGetProxyForUrl(
                IntPtr hSession,
                string url,
                [In] ref AutoProxyOptions autoProxyOptions,
                [In, Out] ref ProxyInfo proxyInfo);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WinHttpCloseHandle(IntPtr httpSession);

        private enum AccessType
        {
            NoProxy = 1,
            AutoDetect = 1,
            AutoProxyConfigUrl = 2
        }

        [Flags]
        private enum AutoDetectType
        {
            Dhcp = 1,
            DnsA = 2,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct AutoProxyOptions
        {

            internal AccessType Flags;

            internal AutoDetectType AutoDetectFlags;

            [MarshalAs(UnmanagedType.LPTStr)]
            internal string AutoConfigUrl;

            private IntPtr lpvReserved;

            private int dwReserved;

            internal bool AutoLogonIfChallenged;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct ProxyInfo
        {

            internal AccessType dwAccessType;

            [MarshalAs(UnmanagedType.LPTStr)]
            internal string Proxy;

            [MarshalAs(UnmanagedType.LPTStr)]
            internal string ProxyBypass;
        }
    }
}
