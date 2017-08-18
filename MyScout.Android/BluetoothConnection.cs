using Android.Bluetooth;
using Java.Lang;
using Java.Util;
using System;

namespace MyScout.Android
{
    public class BluetoothConnection : Thread
    {
        // Variables/Constants
        public static readonly UUID MyScoutUUID = // Literally just a randomly-generated string
            UUID.FromString("51dc3703-9458-4216-b2a3-a16455e6fdb5");

        protected BluetoothSocket socket;
        protected ExtendedBinaryReader reader;
        protected ExtendedBinaryWriter writer;
        protected int connectAttempts = 0;
        protected bool active = false;
        private bool threadClosed = false;

        public const int MaxConnectAttempts = 8;

        // Methods
        public sealed override void Start()
        {
            active = true;
            base.Start();
        }

        public void Close()
        {
            // Set active to false so the thread terminates on the next loop
            active = false;

            // Freeze until thread terminates
            while (!threadClosed) { }

            // Close everything
            CloseSocket();
        }

        protected void CloseSocket()
        {
            if (reader != null)
                reader.Close();

            if (writer != null)
                writer.Close();

            if (socket != null)
                socket.Close();
        }

        public override void Run()
        {
            // Update loop
            while (active)
            {
                // Attempt to connect if not already connected
                // and return if attempt is unsuccessful.
                if (socket == null || !socket.IsConnected)
                {
                    if (!Connect()) return;
                }

                // Call the update loop method
                UpdateLoop();
            }
        }

        protected virtual bool Connect()
        {
            // Ask to establish a connection with the other device
            if (socket == null) return false;
            while (!socket.IsConnected)
            {
                // TODO: Cancel discovery as it slows down a connection

                try
                {
                    socket.Connect();
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

        protected virtual void UpdateLoop()
        {
            throw new NotImplementedException();
        }
    }
}