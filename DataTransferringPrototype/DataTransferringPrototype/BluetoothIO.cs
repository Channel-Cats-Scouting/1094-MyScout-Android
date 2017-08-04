using Android.Bluetooth;
using Android.OS;
using Java.Lang;
using Java.Util;
using System.IO;

namespace DataTransferringPrototype
{
    public class BluetoothIO
    {
        // Variables/Constants
        public static MainActivity MainActivityInstance = null;
        public static BluetoothAdapter Adapter = null;

        protected static UUID myUUID =
            UUID.FromString("fa87c0d0-afac-11de-8a39-0800200c9a66");

        protected const string socketName = "BluetoothPrototype";
        protected const int bufferSize = 2048;

        // Methods
        public static void Host(BluetoothDevice device)
        {
            var hostThread = new HostThread(device);
            hostThread.Start();
        }

        public static void Join()
        {
            var joinThread = new ClientThread(Adapter);
            joinThread.Start();
        }

        protected static void Log(string str)
        {
            #if DEBUG
            Android.Util.Log.Debug("XamarinPrototype", str);
            #endif

            MainActivityInstance.RunOnUiThread(() =>
            {
                MainActivity.ProgressLbl.Text = str;
            });
        }

        protected static void ChangeProgress(int currentValue, int maxValue)
        {
            ChangeProgress((int)(((float)currentValue / maxValue) * 100));
        }

        protected static void ChangeProgress(int value)
        {
            MainActivityInstance.RunOnUiThread(() =>
            {
                MainActivity.ProgressBar.Progress = value;
            });
        }

        // Other
        protected class HostThread : Thread
        {
            // Variables/Constants
            protected BluetoothSocket socket = null;

            // Constructors
            public HostThread(BluetoothDevice device)
            {
                socket = device.CreateRfcommSocketToServiceRecord(myUUID);
            }

            // Methods
            public override void Run()
            {
                // Connect if not already connected
                if (!socket.IsConnected)
                {
                    Log("Connecting...");
                    // TODO: Cancel discovery as it apparently slows down a connection

                    try
                    {
                        socket.Connect();
                        Log("Connected!");
                    }
                    catch (Exception ex)
                    {
                        Log($"ERROR: {ex.Message}");
                    }
                }

                // Read data from file
                string filePath = Path.Combine(
                    Environment.ExternalStorageDirectory.Path, "MyScout", "test.png");

                var data = File.ReadAllBytes(filePath);
                int bytes = 0, chunkIndex = 0;

                // Send file Length
                // TODO: Check if file length is greater than uint max range, and if so, error.
                int dataLength = data.Length;
                socket.OutputStream.WriteByte((byte)((dataLength & 0xFF000000) >> 24));
                socket.OutputStream.WriteByte((byte)((dataLength & 0x00FF0000) >> 16));
                socket.OutputStream.WriteByte((byte)((dataLength & 0x0000FF00) >> 8));
                socket.OutputStream.WriteByte((byte)((dataLength & 0x000000FF)));
                Log($"Sent file length ({dataLength})");

                // Send data one chunk at a time
                while (bytes < dataLength)
                {
                    int size = ((bytes + bufferSize) <= dataLength) ?
                        bufferSize : dataLength - bytes;

                    socket.OutputStream.Write(data, bytes, size);

                    bytes += size;
                    Log($"Sent Chunk #{++chunkIndex} ({bytes}/{dataLength})");
                    ChangeProgress(bytes, dataLength);
                }

                Log($"Done! Sent {bytes} bytes.");

                // Close socket
                socket.Close();
            }
        }

        protected class ClientThread : Thread
        {
            // Variables/Constants
            protected BluetoothServerSocket serverSocket = null;
            protected BluetoothSocket socket = null;

            // Constructors
            public ClientThread(BluetoothAdapter adapter)
            {
                serverSocket = adapter.ListenUsingRfcommWithServiceRecord(
                    socketName, myUUID);
            }

            // Methods
            public override void Run()
            {
                while (socket == null)
                {
                    Log("Looking for sockets to accept...");
                    socket = serverSocket.Accept();

                    // TODO: If the socket is not accepted because the tablet
                    // doesn't like me (just like most people) then handle it or whatever.
                }

                Log("Socket accepted!");

                // Get file size
                int dataSize = -1;
                Log("Waiting for file size...");

                while (dataSize < 1)
                {
                    int len1 = socket.InputStream.ReadByte();
                    int len2 = socket.InputStream.ReadByte();
                    int len3 = socket.InputStream.ReadByte();
                    int len4 = socket.InputStream.ReadByte();

                    if (len1 == -1 && len2 == -1 && len3 == -1 && len4 == -1)
                        continue;

                    dataSize = ((len1 << 24) | (len2 << 16) | (len3 << 8) | len4);
                    Log($"Got file size: {dataSize}");
                }

                // Get data one chunk at a time
                var data = new byte[dataSize];
                int bytes = 0;

                while (bytes < dataSize)
                {
                    int size = ((dataSize - bytes) >= bufferSize) ?
                        bufferSize : dataSize - bytes;

                    int received = socket.InputStream.Read(data, bytes, size);

                    bytes += received;
                    Log($"Received {bytes}/{dataSize}");
                    ChangeProgress(bytes, dataSize);
                }

                // Write data to device
                string filePath = Path.Combine(
                    Environment.ExternalStorageDirectory.Path, "MyScout", "test.png");

                File.WriteAllBytes(filePath, data);
                Log($"Done! Received {bytes} bytes.");

                // Close socket
                socket.Close();
            }
        }
    }
}