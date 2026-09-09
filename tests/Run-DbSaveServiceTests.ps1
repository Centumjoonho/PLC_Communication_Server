$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = Join-Path $repoRoot 'RO_Server_Rebuild_2'
$compiler = 'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\Roslyn\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    throw 'Visual Studio Roslyn csc.exe 경로를 이 PC에 맞게 수정하세요.'
}
$testOutput = Join-Path ([System.IO.Path]::GetTempPath()) ('RO_DbQueueTests_' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testOutput | Out-Null
$testExe = Join-Path $testOutput 'QueueTests.exe'
& $compiler /nologo /target:exe /langversion:7.3 "/out:$testExe" /r:System.dll /r:System.Core.dll /r:System.Web.Extensions.dll `
    (Join-Path $PSScriptRoot 'QueueTests.cs') `
    (Join-Path $projectRoot 'Services\DbSaveService.cs') `
    (Join-Path $projectRoot 'Services\RunRateService.cs') `
    (Join-Path $projectRoot 'Models\PlcData.cs')
if ($LASTEXITCODE -ne 0) { throw '테스트 컴파일 실패' }
& $testExe (Join-Path $testOutput 'data')
if ($LASTEXITCODE -ne 0) { throw "테스트 실패. 진단 파일: $testOutput" }
Write-Output "검증 완료. 진단 파일: $testOutput"
