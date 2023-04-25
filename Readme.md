Private upm 서버와 연결되어있는 개인 게임개발 툴킷, 계속 업데이트 예정 유료 에셋과의 종속성 제거를 하기전

현재 종속성 (추후 제거해함)
- ALine
- Shapes
- Job/Burst (이건 유니티 내장이라 괜찮음)


# Install
 - 종속성을 위해 서버설정 및 클라이언트측 로그인이 되어있어야한다.
 - [Verdaccio](https://shlifedev.tistory.com/69) 서버가 열려 있어야한다.

## 개인 패키지 서버에 로그인
```
   > openupm login -r http://package.shlife.dev:4873 -u id -p password -e email --always-auth=true
```

## 개인 패키지 서버에서 검색하기
 아쉽지만 이는 npm을 사용해야한다.
```
   > npm -r http://package.shlife.dev:4873 search 검색어
```


 
# 프로젝트에서 사용하기
스코프 레지스트리를 등록해야한다. 하지 않으면 종속성을 다운로드 할 수 없음.
 ```
"scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.openupm
      ]
    },
    {
     "name": "custom server",
      "url": "http://package.shlife.dev:4873",
      "scopes": [
        "com",
        "com.shlifedev"
      ]
    }
  ]
```

# 향후 계획
 현재는 클라우드 디스크에 파일이 저장되는데 storage를 백업해두어야 할듯
