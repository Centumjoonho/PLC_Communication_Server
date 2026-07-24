using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
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
        private const int PlcReadTimeoutMs = 400;
        // PLC 데이터를 읽는 메서드
        public async Task<PlcData> ReadPlcData(PlcMaster master)
        {

            byte[] frame = MakeReadFrame(master);

            using (TcpClient client = new TcpClient())
            {

                Task connectTask = client.ConnectAsync(master.PlcIp, master.PlcPort);

                if (await Task.WhenAny(connectTask, Task.Delay(PlcConnectTimeoutMs)) != connectTask)
                {
                    throw new Exception("PLC 연결 시간이 초과되었습니다.");
                }
                await connectTask;

                using (NetworkStream stream = client.GetStream())
                {

                    Task writeTask = stream.WriteAsync(frame, 0, frame.Length);

                    if (await Task.WhenAny(writeTask, Task.Delay(PlcWriteTimeoutMs)) != writeTask)
                    {
                        throw new Exception("PLC 쓰기 시간이 초과되었습니다.");
                    }
                    
                    await writeTask;

                    // receive = read  : 도착 프레임 길이 점검 추가
                    ///////////////////////////////////////////////////////////////////////

                    byte[] buffer = new byte[100];

                    int totalLength = 0;
                    int requiredLength = 13;

                    Task timeoutTask = Task.Delay(PlcReadTimeoutMs);

                    while (totalLength < requiredLength)
                    {
                        Task<int> readTask = stream.ReadAsync(buffer, totalLength, requiredLength - totalLength);

                        if (await Task.WhenAny(readTask, timeoutTask) != readTask)
                        {
                            throw new TimeoutException("PLC 응답 전체를 읽는 시간이 초과 되었습니다.");
                        }

                        int readLength = await readTask;

                        if (readLength == 0)
                        {
                            throw new Exception("PLC가 응답 전송 중 연결을 종료했습니다");
                        }

                        totalLength += readLength;
                    }

                    return MakeData(master, buffer, totalLength);

                    ///////////////////////////////////////////////////////////////////////
                }

            }
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
            byte[] header = new byte[]
            {
                0x00, 0x00,   // Transaction ID
                0x00, 0x00,   // Protocol ID
                0x00, 0x06,   // Length
                0x01,         // Unit ID
                0x03          // Function Code
            };
            // 메모리 주소 100 이상도 반영
            ushort addressValue = Convert.ToUInt16(address, 16);
            byte[] memoryAddress = new byte[]
            {
                (byte)(addressValue >> 8),
                (byte)(addressValue & 0xFF)
            };
            byte[] memorySize = new byte[] { 0x00, 0x02 };
            byte[] frame = new byte[12];

            // 예시 : header의 0번부터 frame의 0번 위치에 header.Length개 만큼 복사해라
            Buffer.BlockCopy(header, 0, frame, 0, header.Length);
            Buffer.BlockCopy(memoryAddress, 0, frame, header.Length, memoryAddress.Length);
            Buffer.BlockCopy(memorySize, 0, frame, header.Length + memoryAddress.Length, memorySize.Length);

            return frame;
        }
    }
}
