using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Plc
{
    public class PlcReader
    {
        // 네트워크 상황에 따라 조정 가능  PLC 연결, 읽기, 쓰기 시간 초과를 총 1초로 설정
        private const int PlcConnectTimeoutMs = 300;
        private const int PlcWriteTimeoutMs = 300;
        private const int PlcReadTimeoutMs = 1000;

        // 현재 PLC Frame에서 읽는 Register 개수
        private const int RegisterCount = 2;

        // Register 2개이므로 응답 데이터는 4바이트
        private const int ExpectedDataByteCount = RegisterCount * 2;

        // PLC 와 TCP 연결을 생성 
        public async Task<TcpClient> ConnectPlcAsync(PlcMaster master , CancellationToken cancellationToken) 
        {

            if (master == null)
            {
                throw new ArgumentNullException(nameof(master));
            }

            if (string.IsNullOrWhiteSpace(master.PlcIp))
            {
                throw new ArgumentException(
                    "PLC IP가 없습니다.",
                    nameof(master));
            }

            if (master.PlcPort <= 0 ||
                master.PlcPort > 65535)
            {
                throw new ArgumentException(
                    "PLC Port가 올바르지 않습니다.",
                    nameof(master));
            }

            TcpClient client = new TcpClient();

            try
            {
                Task connectTask = client.ConnectAsync(master.PlcIp, master.PlcPort);

                Task timeoutTask = Task.Delay(PlcConnectTimeoutMs, cancellationToken);

                if(await Task.WhenAny(connectTask, timeoutTask) != connectTask)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    throw new TimeoutException("PLC 연결 시간이 초과되었습니다.");
                }

                await connectTask;

                return client;
            }
            catch 
            {
                client.Close();
                throw;
            }

        }
        // 이미 연결된 TcpClient를 이용하여 Modbus 읽기 요청을 보내고 PLC 응답을 받음
        public async Task<PlcData> ReadPlcDataAsync( PlcMaster plcMaster, TcpClient client , CancellationToken cancellationToken)
        {
            if(plcMaster == null)
            {
                throw new ArgumentNullException(nameof(plcMaster));
            }
            if(client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (!client.Connected)
            {
                throw new InvalidOperationException("PLC가 연결되어 있지 않습니다.");
            }
            cancellationToken.ThrowIfCancellationRequested();

            // PLC에 보낼 Modbus TCP 읽기 요청 생성
            byte[] frame = MakeReadFrame(plcMaster);

            // 연결할때 생성한 TcpClient의 통신 Stream 사용 
            NetworkStream stream = client.GetStream();

            // 읽기 요청 프레임 전송
            Task writeTask = stream.WriteAsync(frame, 0, frame.Length, cancellationToken);
            Task writeTimeoutTask = Task.Delay(PlcWriteTimeoutMs, cancellationToken);


            if (await Task.WhenAny(writeTask, writeTimeoutTask) != writeTask)
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw new TimeoutException("PLC 쓰기 시간이 초과되었습니다.");
            }
            
            await writeTask;

            byte[] buffer = new byte[260];

            //Header와 Data를 모두 받는데 적용할 제한시간 
            Task readTimeOutTask = Task.Delay(PlcReadTimeoutMs, cancellationToken);

            // MBAP Header 7바이트 + Function Code 1바이트 + Byte Count 1바이트
            int totalLength = await ReadExactAsync(stream, buffer, 0, 9, readTimeOutTask, cancellationToken);

            // Function Code와 데이터 길이 확인
            int dataByteCount = ValidateResponseHeader(buffer, totalLength);

            // 실제로 받아야 하는 전체 응답 길이
            int requiredLength = 9 + dataByteCount;

            // Header 이후의 실제 데이터 수신
            totalLength = await ReadExactAsync(stream, buffer, totalLength, requiredLength, readTimeOutTask, cancellationToken);

            return MakeData(plcMaster, buffer, totalLength);
        }
        
        // PLC 데이터를 읽는 메서드
        public async Task<PlcData> ReadPlcData(PlcMaster master , CancellationToken cancellationToken = default(CancellationToken))
        {

            if (master == null)
            {
                throw new ArgumentNullException(nameof(master));
            }

            if (string.IsNullOrWhiteSpace(master.PlcIp))
            {
                throw new ArgumentException("PLC IP가 없습니다.", nameof(master));
            }

            if (master.PlcPort <= 0 || master.PlcPort > 65535)
            {
                throw new ArgumentException("PLC Port가 올바르지 않습니다.", nameof(master));
            }

            cancellationToken.ThrowIfCancellationRequested();

            byte[] frame = MakeReadFrame(master);

            using (TcpClient client = new TcpClient())
            {

                Task connectTask = client.ConnectAsync(master.PlcIp, master.PlcPort);
                Task connectTimeoutTask = Task.Delay(PlcConnectTimeoutMs, cancellationToken);

                if (await Task.WhenAny(connectTask, connectTimeoutTask) != connectTask)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw new TimeoutException("PLC 연결 시간이 초과되었습니다.");
                }
                // ConnectAsync 내부에서 발생한 SocketException 확인
                await connectTask;

                using (NetworkStream stream = client.GetStream())
                {

                    Task writeTask = stream.WriteAsync(frame, 0, frame.Length, cancellationToken);
                    Task writeTimeoutTask = Task.Delay(PlcWriteTimeoutMs, cancellationToken);


                    if (await Task.WhenAny(writeTask, writeTimeoutTask) != writeTask)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        throw new TimeoutException("PLC 쓰기 시간이 초과되었습니다.");
                    }
                    // WriteAsync 내부 예외 확인
                    await writeTask;

                    byte[] buffer = new byte[260];

                    // PLC 응답 전체에 하나의 읽기 제한시간 적용
                    Task readTimeoutTask = Task.Delay(PlcReadTimeoutMs, cancellationToken);

                    // MBAP Header 7바이트 + Function Code 1바이트 + Byte Count 1바이트
                    int totalLength = await ReadExactAsync(stream, buffer, 0, 9, readTimeoutTask, cancellationToken);

                    // Function Code와 데이터 길이 확인
                    int dataByteCount = ValidateResponseHeader(buffer, totalLength);

                    // 실제로 받아야 하는 전체 응답 길이
                    int requiredLength = 9 + dataByteCount;

                    totalLength = await ReadExactAsync(stream, buffer, totalLength, requiredLength, readTimeoutTask, cancellationToken);


                    return MakeData(master, buffer, totalLength);

                }

            }
        }
        // requiredLength만큼 응답을 전부 받을 때까지 반복해서 읽음
        private async Task<int> ReadExactAsync(NetworkStream stream,byte[] buffer,int currentLength,int requiredLength,Task timeoutTask,CancellationToken cancellationToken)
        {
            int totalLength = currentLength;

            while (totalLength < requiredLength)
            {
                Task<int> readTask = stream.ReadAsync(
                    buffer,
                    totalLength,
                    requiredLength - totalLength,
                    cancellationToken);

                if (await Task.WhenAny(readTask, timeoutTask) != readTask)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    throw new TimeoutException("PLC 응답 전체를 읽는 시간이 초과되었습니다.");
                }

                int readLength = await readTask;

                if (readLength == 0)
                {
                    throw new Exception("PLC가 응답 전송 중 연결을 종료했습니다.");
                }

                totalLength += readLength;
            }

            return totalLength;
        }

        // 수신한 Modbus TCP 응답 Header 확인
        private int ValidateResponseHeader(byte[] buffer, int length)
        {
            if (buffer == null || length < 9)
            {
                throw new Exception("PLC 응답 Header가 너무 짧습니다.");
            }

            // 요청 Frame의 Transaction ID가 0x0000이므로 응답도 같은 값이어야 함
            if (buffer[0] != 0x00 || buffer[1] != 0x00)
            {
                throw new Exception("PLC Transaction ID가 올바르지 않습니다.");
            }

            // Modbus TCP의 Protocol ID는 0x0000
            if (buffer[2] != 0x00 || buffer[3] != 0x00)
            {
                throw new Exception("PLC Protocol ID가 올바르지 않습니다.");
            }

            // 현재 요청 Frame에서 Unit ID는 0x01
            if (buffer[6] != 0x01)
            {
                throw new Exception("PLC Unit ID가 올바르지 않습니다.");
            }

            // Function Code의 최상위 Bit가 1이면 Modbus 예외 응답
            if ((buffer[7] & 0x80) != 0)
            {
                throw new Exception("PLC 예외 응답을 수신했습니다. Code : " + buffer[8]);
            }

            // 현재 읽기 요청은 Function Code 0x03
            if (buffer[7] != 0x03)
            {
                throw new Exception("PLC Function Code가 올바르지 않습니다.");
            }

            int dataByteCount = buffer[8];

            if (dataByteCount != ExpectedDataByteCount)
            {
                throw new Exception("PLC 응답 데이터 길이가 올바르지 않습니다. Byte Count : " + dataByteCount);
            }

            // MBAP Length는 Unit ID, Function Code, Byte Count, Data 길이의 합
            int responseLength = (buffer[4] << 8) | buffer[5];
            int expectedResponseLength = 3 + dataByteCount;

            if (responseLength != expectedResponseLength)
            {
                throw new Exception("PLC MBAP Length가 올바르지 않습니다.");
            }

            return dataByteCount;
        }
        // 수신 프레임에서 data 부분만 추출
        private PlcData MakeData(PlcMaster master, byte[] buffer, int length)
        {
            if (length <= 12)
            {
                throw new Exception("PLC 응답 데이터가 너무 짧습니다");
            }

            // 송신 프레임의 마지막 값 : data -> 2진수로 추출
            string bits = Convert.ToString(buffer[12], 2).PadLeft(8, '0');

            return new PlcData
            {
                PlcCode = master.PlcCode,
                PlcName = master.PlcName,
                PlcIp = master.PlcIp,
                PlcPort = master.PlcPort,
                MemoryAddress = master.MemoryAddress,
                ReceiveData = bits,
                Status = bits == "00000001" ? "RUN" : "STOP",
                ReceiveTime = DateTime.Now
            };

        }
        // PLC 읽기 프레임 생성
        private byte[] MakeReadFrame(PlcMaster master)
        {
            string address = string.IsNullOrWhiteSpace(master.MemoryAddress) ? "30" : master.MemoryAddress;

            ushort addressValue;

            if (!ushort.TryParse(address, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out addressValue))
            {
                throw new ArgumentException("PLC Memory Address가 올바른 16진수가 아닙니다.", nameof(master));
            }

            byte[] header = new byte[]
            {
                0x00, 0x00,   // Transaction ID
                0x00, 0x00,   // Protocol ID
                0x00, 0x06,   // Length
                0x01,         // Unit ID
                0x03          // Function Code
            };
           
            byte[] memoryAddress = new byte[]
            {
                (byte)(addressValue >> 8),
                (byte)(addressValue & 0xFF)
            };

            byte[] memorySize = new byte[]
            {
                (byte)(RegisterCount >> 8),
                (byte)(RegisterCount & 0xFF)
            };
            byte[] frame = new byte[12];

            // 예시 : header의 0번부터 frame의 0번 위치에 header.Length개 만큼 복사해라
            Buffer.BlockCopy(header, 0, frame, 0, header.Length);
            Buffer.BlockCopy(memoryAddress, 0, frame, header.Length, memoryAddress.Length);
            Buffer.BlockCopy(memorySize, 0, frame, header.Length + memoryAddress.Length, memorySize.Length);

            return frame;
        }
    }
}
