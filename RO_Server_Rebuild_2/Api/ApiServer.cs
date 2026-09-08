using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RO_Server_Rebuild_2.Api
{
    public class ApiServer
    {
        // 서버 시작 / 정지가 동시에 실행 될 때
        // tcpListener, cancellationTokenSource , tcpListenTask 상태를 보호 
        private readonly object stateLock = new object();
        private readonly object clientLock = new object();

        private readonly List<TcpClient> clientList = new List<TcpClient>();
        private readonly List<Task> clientTaskList = new List<Task>();

        // API 요청에 맞는 응답을 만들어주는 객체
        private readonly ApiHandler apiHandler;

        private TcpListener tcpListener;
        private CancellationTokenSource cancellationTokenSource;
        private Task tcpListenTask;

        // 클라이언트로부터 다음 데이터가 도착하기까지 기다리는 최대 시간
        private const int ClientReadTimeoutMs = 3000;
        // 비정상적으로 큰 HTTP Header가 메모리에 계속 쌓이는 것을 방지
        private const int MaxHeaderSize = 16 * 1024;
        // JSON 본문의 최대 크기를 1MB로 제한
        private const int MaxBodySize = 1024 * 1024;
        
        private const int MaxConcurrentClientCount = 20;
        private readonly SemaphoreSlim clientSemaphore = new SemaphoreSlim(MaxConcurrentClientCount, MaxConcurrentClientCount);

        public ApiServer(ApiHandler apiHandler)
        {
            if(apiHandler == null)
            {
                throw new ArgumentNullException(nameof(apiHandler));
            }

            this.apiHandler = apiHandler;
        }

        // 실제 TcpListener 존재 여부를 기준으로 서버 실행 상태 반환
        // Listener와 접속 대기 Task가 모두 실행 중일 때만 true 반환
        public bool IsRunning
        {
            get
            {
                lock (stateLock)
                {
                    // Listener가 없으면 서버가 실행되지 않은 상태
                    if (tcpListener == null)
                    {
                        return false;
                    }
                    // 접속 대기 Task가 생성되지 않았다면 정상 실행 상태가 아니므로 false 반환
                    if(tcpListenTask == null)
                    {
                        return false;
                    }
                    
                    // ListenLoopAsync가 종료 되었다면 서버가 정상 실행중인 상태가 아님
                    if(tcpListenTask.IsCompleted)
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

        public bool StartListening(int port , out string errorMessage)
        {
            errorMessage = string.Empty;

            if(port<=0 || port > 65535)
            {
                errorMessage = "API 서버 Port는 1부터 65535 사이로 설정하세요.";

                return false;
            }

            lock (stateLock)
            {

                try
                {
                    // Listener가 존재한다면 Listen Task 상태 확인
                    if(tcpListener != null)
                    {
                        // ListenLoopAsync가 아직 실행 중이라면 중복 시작하지 않음
                        if(tcpListenTask != null && !tcpListenTask.IsCompleted)
                        {
                            return true;
                        }

                        //Listner 만 남아 있고  Listen Task가 끝난 비정상 상태 정리
                        tcpListener.Stop();
                        tcpListener = null;
                    }

                    // 이전 CancellationTokenSource 정리
                    if (cancellationTokenSource != null)
                    {
                        cancellationTokenSource.Cancel();
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = null;
                    }

                    tcpListenTask = null;

                    cancellationTokenSource = new CancellationTokenSource();

                    tcpListener = new TcpListener(IPAddress.Any, port);

                    tcpListener.Start();

                    CancellationToken cancellationToken = cancellationTokenSource.Token;

                    tcpListenTask =  ListenLoopAsync(tcpListener, cancellationToken);

                    return true;
                }
                catch (Exception ex)
                {
                    if (tcpListener != null)
                    {
                        try
                        {
                            tcpListener.Stop();
                        }
                        catch
                        {
                        }

                        tcpListener = null;
                    }

                    if (cancellationTokenSource != null)
                    {
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = null;
                    }

                    tcpListenTask = null;

                    errorMessage = "API 서버 시작 실패 : " + ex.Message;

                    return false;
                }
        }   }
        // 클라이언트 접속을 기다리고 요청 처리 작업을 시작
        private async Task ListenLoopAsync(TcpListener runningTcpListener, CancellationToken cancellationToken)
        {
            try
            {
              
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 클라이언트가 접속할 때까지 비동기로 대기
                        TcpClient client = await runningTcpListener.AcceptTcpClientAsync();

                        // 접속한 직후 서버 정지가 요청 되었다면 처리하지 않고 종료
                        if(cancellationToken.IsCancellationRequested)
                        {
                            client.Close();
                            
                            break;
                        }

                        client.NoDelay = true;

                        // 서버 정지 시 접속중인 클라이언트를 닫을 수 있도록 보관
                        lock (clientLock)
                        {
                            clientList.Add(client);
                        }

                        // 클라언트 요청 처리는 별도 비동기 작업으로 진행
                        Task clientTask = HandleClientAsync(client, cancellationToken);

                        lock (clientLock)
                        {
                            clientTaskList.Add(clientTask);
                        }

                        // 클라이언트 처리가 끝나면 완료된 Task를 제거
                        _= clientTask.ContinueWith(completedTask =>
                        {
                            lock (clientLock)
                            {
                                clientTaskList.Remove(completedTask);
                            }

                        },TaskScheduler.Default);
                    }
                    catch (ObjectDisposedException)
                    {
                        // 서버 정지 과정에서 TcpListener.Stop()이 호출된 경우
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        throw;
                    }
                    catch (SocketException ex)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        
                        LogService.Error("[API][ACCEPT][FAIL] " + ex.Message);

                        // 일시적인 소켓 오류라면 잠시 후 다시 접속 대기
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // CancellationToken에 의한 정상적인 서버 정지
            }
            catch (Exception ex)
            {
                LogService.Error("[API][ACCEPT_LOOP][EXCEPTION] " + ex.Message);
            }
        }
        // 접속한 클라이언트 1개의 API 요청과 응답 처리
        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            // HTTP 요청 읽기
            // JSON을 ApiMessage로 변환
            // apiHandler.CreateResponse() 호출
            // 응답 JSON 전송
            // clientList에서 제거하고 소켓 종료

            string clientIp = string.Empty;
            bool semaphoreEntered = false;

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            try
            {
                // 동시에 최대 20개 Client만 요청 처리
                await clientSemaphore.WaitAsync(cancellationToken);
                
                semaphoreEntered = true;

                // 접속한 클라이언트 IP 확인
                IPEndPoint remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;

                if (remoteEndPoint != null)
                {
                    clientIp = remoteEndPoint.Address.ToString();
                }

                // HTTP 요청에서 JSON 본문 읽기
                string requestBody = await ReadHttpBodyAsync(client,cancellationToken);

                // JSON 본문을 ApiMessage 객체로 변환
                ApiMessage request = jsonSerializer.Deserialize<ApiMessage>(requestBody);

                // 요청 종류와 API Key를 확인하여 응답 생성
                ApiMessage response = apiHandler.CreateResponse(request, clientIp);

                // 응답 객체를 JSON 문자열로 변환
                string responseJson = jsonSerializer.Serialize(response);

                // 클라이언트에 HTTP 응답 전송
                await WriteHttpResponseAsync(client , responseJson , cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // API 서버 정지 요청에 따른 정상 종료
            }
            catch (ObjectDisposedException)
            {
                // 서버 정지 과정에서 클라이언트가 닫힌 정상 상황
            }
            catch (Exception ex)
            {
                LogService.Error(
                    "[API][REQUEST][FAIL] " +
                    "ClientIp: " + clientIp + " / " +
                    ex.Message);

                try
                {
                    // 내부 예외 내용은 클라이언트에게 그대로 노출하지 않음
                    ApiMessage errorResponse =
                        apiHandler.CreateErrorResponse(
                            "API 요청 처리 중 오류가 발생했습니다.");

                    string errorJson =
                        jsonSerializer.Serialize(errorResponse);

                    await WriteHttpResponseAsync(
                        client,
                        errorJson,
                        cancellationToken);
                }
                catch
                {
                    // 클라이언트 연결까지 끊어진 경우 응답 전송 불가
                }
            }
            finally
            {
                // 서버가 관리하는 접속 클라이언트 목록에서 제거
                lock (clientLock)
                {
                    clientList.Remove(client);
                }

                try
                {
                    client.Close();
                }
                catch
                {
                }
                // 실제로 자리를 받은 경우에만 반환
                if (semaphoreEntered)
                {
                    clientSemaphore.Release();
                }
            }
        }
        // JSON 결과를 HTTP 응답으로 전송
        private async Task WriteHttpResponseAsync( TcpClient client, string responseJson, CancellationToken cancellationToken)
        {
            NetworkStream stream = client.GetStream();

            byte[] bodyBytes = Encoding.UTF8.GetBytes(responseJson ?? string.Empty);

            string header =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: application/json; charset=utf-8\r\n" +
                "Content-Length: " + bodyBytes.Length + "\r\n" +
                "Connection: close\r\n" +
                "\r\n";

            byte[] headerBytes = Encoding.ASCII.GetBytes(header);

            await stream.WriteAsync(headerBytes, 0, headerBytes.Length, cancellationToken);
            await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        // HTTP 요청에서 JSON 본문 읽기
        private async Task<string> ReadHttpBodyAsync(TcpClient client, CancellationToken cancellationToken)
        {
            NetworkStream stream = client.GetStream();

            // 네트워크에서 한 번에 읽을 임시 공간
            byte[] buffer = new byte[8192];

            // 지금까지 받은 전체 HTTP 요청 데이터 : 계속 추가
            byte[] receivedBytes = null;
            
            int headerEnd = -1;
            
            //HTTP header가 여러번 나뉘어 들어올 수 있으므로 누적해서 읽음
            using (MemoryStream requestStream = new MemoryStream())
            {
                while (headerEnd < 0)
                {
                    // 클라이언트가 처음 보낸 데이터 읽기
                    int readLength = await ReadStreamWithTimeoutAsync(stream, buffer, buffer.Length, cancellationToken);

                    if (readLength <= 0)
                    {
                        throw new IOException("API 요청 Header 수신 중 연결이 종료되었습니다.");
                    }

                    // 받은 내용을 기존 수신 데이터 뒤에 추가
                    requestStream.Write(buffer, 0, readLength);

                    // 받은 전체 데이터 배열로 셋팅
                    receivedBytes = requestStream.ToArray();

                    // HTTP Header는 영문 문자이므로 ASCII로 확인
                    string requestText = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);

                    // Header와 Body 사이의 빈 줄 위치 확인
                    headerEnd = requestText.IndexOf("\r\n\r\n", StringComparison.Ordinal);

                    // Header 끝을 아직 찾지 못했는데 제한 크기를 넘은 경우
                    if (headerEnd < 0 && requestStream.Length > MaxHeaderSize)
                    {
                        throw new InvalidOperationException("HTTP 요청 Header가 너무 큽니다.");
                    }
                }

                // Header 끝을 찾았더라도 Header 자체가 제한 크기를 넘으면 거부
                if (headerEnd > MaxHeaderSize)
                {
                    throw new InvalidOperationException("HTTP 요청 Header가 너무 큽니다.");
                }

                string headerText = Encoding.ASCII.GetString( receivedBytes, 0, headerEnd);
                
                int contentLength = GetContentLength(headerText);

                    if (contentLength <= 0)
                    {
                        throw new InvalidOperationException("HTTP Content-Length가 올바르지 않습니다.");
                    }

                    if (contentLength > MaxBodySize)
                    {
                        throw new InvalidOperationException("API 요청 본문이 너무 큽니다.");
                    }

                    // Header 끝의 \r\n\r\n 다음부터 Body가 시작
                    int bodyStartIndex = headerEnd + 4;

                    using (MemoryStream bodyStream = new MemoryStream())
                    {
                    // Header를 읽는 과정에서 Body까지 함께 들어온 경우 먼저 저장
                    int receivedBodyLength = receivedBytes.Length - bodyStartIndex;

                    if (receivedBodyLength > 0)
                    {
                        int saveLength = Math.Min(receivedBodyLength, contentLength);

                        bodyStream.Write(receivedBytes, bodyStartIndex, saveLength);
                    }

                    // 아직 받지 못한 Body가 있으면 Content-Length만큼 계속 읽음
                    while (bodyStream.Length < contentLength)
                    {
                        int remainingLength = contentLength - (int)bodyStream.Length;
                            
                        int readSize = Math.Min(buffer.Length, remainingLength);

                        int readLength = await ReadStreamWithTimeoutAsync(stream, buffer, readSize, cancellationToken);

                        if (readLength <= 0)
                        {
                            throw new IOException("API 요청 본문 수신 중 연결이 종료되었습니다.");
                        }

                        bodyStream.Write(buffer, 0, readLength);
                    }

                        byte[] bodyBytes = bodyStream.ToArray();

                        return Encoding.UTF8.GetString(bodyBytes, 0, contentLength);
                    }

            }

        }
        // HTTP Header에서 Content-Length 값 찾기
        private int GetContentLength(string headerText)
        {
            string[] headerLines = headerText.Split(new[] { "\r\n" }, StringSplitOptions.None);

            foreach (string headerLine in headerLines)
            {
                if (!headerLine.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string lengthText = headerLine.Substring("Content-Length:".Length).Trim();
                int contentLength;

                if (int.TryParse(lengthText, out contentLength))
                {
                    return contentLength;
                }
            }

            return -1;
        }
        // 클라이언트가 데이터를 보내지 않은 상태로 연결만 계속 유지하는 것을 방지 (3초 유지)
        private async Task<int> ReadStreamWithTimeoutAsync(NetworkStream stream,byte[] buffer,int readSize, CancellationToken cancellationToken)
        {
            Task<int> readTask = stream.ReadAsync(buffer, 0, readSize, cancellationToken);

            Task timeoutTask = Task.Delay(ClientReadTimeoutMs, cancellationToken);

            Task completedTask = await Task.WhenAny(readTask, timeoutTask);

            if (completedTask != readTask)
            {
                cancellationToken.ThrowIfCancellationRequested();

                throw new TimeoutException("API 요청 데이터 수신 시간이 초과되었습니다.");
            }

            return await readTask;
        }

        // API 서버 정지
        public async Task<bool> StopListeningAsync()
        {
            TcpListener runningTcpListener;
            CancellationTokenSource runningTokenSource;
            Task runningTcpListenTask;

            lock (stateLock)
            {
                // 이미 서버가 정지된 상태라면 성공으로 처리
                if (tcpListener == null)
                {
                    return true;
                }

                // 정리할 서버 리소스를 지역 변수에 보관
                runningTcpListener = tcpListener;
                runningTokenSource = cancellationTokenSource;
                runningTcpListenTask = tcpListenTask;

                // 중복 정지 요청이 같은 객체를 다시 정리하지 않도록 필드 초기화
                tcpListener = null;
                cancellationTokenSource = null;
                tcpListenTask = null;
            }

            try
            {
                // ListenLoopAsync 반복문에 정지 요청
                if (runningTokenSource != null)
                {
                    runningTokenSource.Cancel();
                }

                // AcceptTcpClientAsync 접속 대기를 실제로 종료
                runningTcpListener.Stop();

                // 접속 대기 Task가 완전히 끝날 때까지 기다림
                if (runningTcpListenTask != null)
                {
                    await runningTcpListenTask;
                }

                TcpClient[] runningClientArray;
                Task[] runningClientTaskArray;

                // 현재 처리 중인 클라이언트와 Task 복사
                lock (clientLock)
                {
                    runningClientArray = clientList.ToArray();
                    runningClientTaskArray = clientTaskList.ToArray();

                    clientList.Clear();
                    clientTaskList.Clear();
                }

                // 요청 처리 중인 클라이언트 연결 종료
                foreach (TcpClient client in runningClientArray)
                {
                    try
                    {
                        client.Close();
                    }
                    catch
                    {
                    }
                }

                // 클라이언트 요청 처리 Task 종료 대기
                if (runningClientTaskArray.Length > 0)
                {
                    await Task.WhenAll(runningClientTaskArray);
                }

                return true;
            }
            catch (ObjectDisposedException)
            {
                // 이미 Listener가 정리된 경우 정지 성공으로 처리
                return true;
            }
            catch (OperationCanceledException)
            {
                // 취소 토큰으로 종료된 정상 상황
                return true;
            }
            catch (Exception ex)
            {
                LogService.Error("[API_SERVER][STOP][FAIL] " + ex.Message);

                return false;
            }
            finally
            {
                if (runningTokenSource != null)
                {
                    runningTokenSource.Dispose();
                }
            }
        }
    }
}
