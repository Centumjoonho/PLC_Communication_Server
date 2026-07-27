using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Api
{
    public class ApiServer
    {
      // 서버 시작 / 정지가 동시에 실행 될 때
      // tcpListener, cancellationTokenSource , tcpListenTask 상태를 보호 
      private readonly object stateLock = new object();

        private TcpListener tcpListener;
        private CancellationTokenSource cancellationTokenSource;
        private Task tcpListenTask;

        // 실제 TcpListener 존재 여부를 기준으로 서버 실행 상태 반환
        public bool IsRunning
        {
            get
            {
                lock (stateLock)
                {
                    return tcpListener != null;
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
                if (tcpListener != null)
                {

                    return true;
                }



                try
                {
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
        private async Task ListenLoopAsync(TcpListener runningTcpListener, CancellationToken cancellationToken)
        {
            try
            {
              
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 클라이언트가 접속할 때까지 비동기로 대기
                        TcpClient client =await runningTcpListener.AcceptTcpClientAsync();

                        // 현재 단계에서는 접속만 확인하고 종료
                        // 이후 ApiHandler 연결 시 요청 처리 함수 호출
                        client.Close();
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
                    catch (SocketException)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        // 일시적인 소켓 오류라면 잠시 후 다시 접속 대기
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // CancellationToken에 의한 정상적인 서버 정지
            }
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
            catch (Exception)
            {
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
