# 실행 화면 캡처 가이드

포트폴리오 README에 사용할 **실제 WinForms 실행 화면**을 이 폴더에 저장합니다.

현재 저장소에는 실행 화면 원본이 없으므로 임의로 생성한 화면을 사용하지 않습니다. 아래 기준으로 로컬 Windows PC에서 캡처한 뒤 지정된 파일명으로 추가하세요.

## 필수 화면

| 파일명 | 캡처할 화면 | 보여줘야 할 내용 |
|---|---|---|
| `server-dashboard.png` | Server 메인 | API Server 상태, PLC 수집 상태, DB 저장 상태 |
| `server-plc-register.png` | PLC 등록 | PLC 코드·명칭·IP·Port·메모리 주소·사용 여부 항목 |
| `server-log-viewer.png` | 로그 화면 | 정상 수집 로그와 특정 PLC 통신 실패가 분리되는 모습 |
| `server-admin-settings.png` | DB/API 설정 | 설정 항목의 종류만 보이도록 캡처하고 실제 값은 마스킹 |

## 장애 격리 화면 권장 구성

포트폴리오에서 핵심 설계를 전달하려면 다음 상태를 한 화면 또는 연속 캡처로 보여주는 것이 좋습니다.

1. 여러 PLC가 정상 수집 중인 상태
2. 테스트 PLC 한 대만 연결 실패 상태
3. 나머지 PLC의 수집과 DB 저장이 계속 진행되는 로그
4. Client가 재연결된 뒤 최신 데이터가 다시 조회되는 상태

## 권장 캡처 조건

- Windows 디스플레이 배율 100%
- 프로그램 창 크기 1600 x 900 이상
- 테스트용 PLC 또는 Simulator와 샘플 데이터 사용
- 로그 시각이 보이도록 하되 실제 운영 데이터는 제거
- 마우스 커서와 다른 프로그램 창은 화면에서 제외
- PNG 형식으로 저장
- 이미지마다 불필요한 여백을 제거하되 프로그램 전체 구조는 보이도록 유지

## 공개 전 반드시 제거할 정보

- 실제 PLC, DB 및 Server IP
- API Key와 DB 비밀번호
- 실제 공장명, 공정명과 설비 식별번호
- 사용자 계정, 로컬 Windows 경로와 사내 공유 경로
- 생산량, 가동 실적, 장애 이력 등 현장 데이터

공개용 캡처에는 `192.0.2.x` 테스트 주소, `YOUR_API_KEY`, `plc_monitoring`과 같은 샘플값을 사용하세요.

## README 삽입 예시

이미지를 이 폴더에 추가한 뒤 Server README의 실행 화면 섹션에 다음 형식으로 연결합니다.

```markdown
### Server 상태 및 PLC 수집

![PLC 수집 Server 메인 화면](docs/screenshots/server-dashboard.png)

### PLC 통신 및 오류 로그

![PLC별 통신 로그](docs/screenshots/server-log-viewer.png)
```
