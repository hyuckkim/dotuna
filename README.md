# DoTuna
[참치 인터넷 어장 아카이브](https://archive.tunaground.net/)의 [json 형식 파일](https://mega.nz/folder/COpUVSxY#AEbhRcjb2lzLQ0K9t0n9ng/folder/Pb43BLDK)의 원하는 일부만을 추출해 html 폴더를 만듭니다.  
(실행 파일이 있는 디렉터리에 Result 폴더가 생성됩니다)

## 사용 방법
1. json 형식 파일들을 받아서 압축을 풀어야 합니다. 압축을 풀어서 폴더 안에 index.json이 있어야 합니다.
2. dotuna.exe에서 폴더를 엽니다.

## 생성되는 파일 파일 소개
index.html에 문서 목록이 정리되어 있고, 나머지 파일들은 {id}.html로 생성됩니다.  
{id}, {title}, {name}, {created}, {updated}, {size}가 실제 이름으로 대체됩니다.
하위 폴더로 data가 생기고, data 폴더 안에는 레스들에서 사용중인 이미지들이 저장됩니다.

추가로 이 파일들은 다음과 같은 특징이 있습니다:
 - html 파일들의 생김새는 아카이브와 대략 비슷하지만 완전히 일치하는 것은 아닙니다.
 - 레스 번호를 누르면 그 레스로 가는 링크에 연결됩니다.

## 별 의미는 없는 링크글
https://bbs2.tunaground.net/trace/tuna/4394  
https://gall.dcinside.com/aa/16858

## 실행 사진
![image](https://github.com/user-attachments/assets/b3d17cdd-bea4-4723-ac11-eefd7493194d)

## todo
[v] 링크
[v] 이미지
[] 이미지 복사 중 메시지 띄우기
[] 파일 5등분 할 수 있게?
[] index.json 여러개 이어붙이기?
