using Android.Bluetooth;
using System.IO;
using System.Text;

namespace MyScout.Android
{
    public class ScoutConnection : BluetoothConnection
    {
        // Variables/Constants
        protected BluetoothServerSocket serverSocket;

        // Constructors
        public ScoutConnection(BluetoothAdapter adapter)
        {
            serverSocket = adapter.ListenUsingRfcommWithServiceRecord(
                "MyScout_Scout", MyScoutUUID);
        }

        // Methods
        protected override bool Connect()
        {
            // Accept any incoming connection requests
            while (socket == null)
            {
                try
                {
                    socket = serverSocket.Accept();
                    reader = new ExtendedBinaryReader(socket.InputStream);
                    writer = new ExtendedBinaryWriter(socket.OutputStream);
                    return true;
                }
                catch
                {
                    // Attempt to close the socket which failed to connect
                    try
                    {
                        CloseSocket();
                    }
                    catch { }

                    // Connection will be re-attempted on next loop if connectAttempts
                    // does not exceed the maximum allowed number of attempts.
                    if (++connectAttempts > MaxConnectAttempts)
                    {
                        // TODO: Display error
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void UpdateLoop()
        {
            // TODO
        }
    }
}