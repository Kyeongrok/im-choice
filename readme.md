# im-choice
Windows에서 IME를 바꿔주는 앱 입니다. VsCode, NeoVim등에서 설정을 통해 Esc키를 눌렀을 때 IME를 변경하도록 할 때 사용합니다. im-select.exe 가 잘 작동 하지 않아서 생성한 프로젝트 입니다.

## 사용방법
1. 오른쪽 Release에서 최신 릴리즈를 받고 압축을 풉니다.
2. HOME디렉토리 또는 원하는 위치에 im-choice.exe 로 파일을 이동 합니다.

### 명령어

입력기 변경  
![image](https://github.com/user-attachments/assets/2c664a6c-fe7c-4070-b4ff-6c596bfa8e25)

```shell
./im-choice.exe {im}
```

설치된 입력기 목록 보기
```shell
./im-choice.exe -l
```

### 주요 입력기
- 한글 412
- 영문 409


## 빌드
```shell
dotnet build -c Release
```

### 단일 파일로 빌드
```shell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishReadyToRun=true
```

### c:\im-choice에 빌드

```shell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o c:\im-choice
```
