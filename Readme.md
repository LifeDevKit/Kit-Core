# Install
 - [Verdaccio](https://shlifedev.tistory.com/69) 가 설정 되어있어야 한다.

# 스코프 설정
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
