![게임 배너.png](/ReadMeSource/banner.png)

- 태그: 1인칭, 생존 호러
- 사용 기술: Github, Unity 3D, VisualStudio
- 팀 구성: 프로그래머 3명(맵&인벤토리 / NPC / 플레이어)
- 플랫폼: PC
- 작업 기간: 2023년 3월 1일 → 2023년 7월 30일
<br>


## ❗개요
- **Unity 3D를 사용한 PC 기반의 1인칭 생존 호러 게임**
- 플레이어 임의의 맵 구조 변경 시스템(벽뜯기)을 도입하여 일반적인 방탈출 생존 호러 게임보다 플레이의 자유도를 높이는 것을 목표로 개발했습니다.
- 전남대학교 2023 소프트웨어중심대학사업단 성과 발표회 동상 수상
<br>


## 📜 주요 기능

### 플레이어 임의의 맵 구조 변경 시스템
![맵구조 변경 시스템.png](/ReadMeSource/MovingWall.png)
- 맵 내부에는 플레이어 임의로 위치 변경이 가능한 벽들이 존재합니다. 이 벽들을 옮겨서 새로운 길을 개척하거나, 적대적 NPC의 접근을 차단하는 용도로 사용할 수 있습니다.
- 본 기능은 맵 내에 존재하는 망치 오브젝트의 습득을 통해 활성화됩니다. 기능 활성화 상태에서 옮길 수 있는 벽을 바라보면 사용 가능 횟수와 외곽선이 표시됩니다.
- 사용 방법은 기능 활성화 상태에서 이동이 가능한 벽을 바라보고 키를 입력하는 것입니다. 키를 누르면 벽을 들어 올리고, 키를 놓으면 해당 위치에 벽이 고정됩니다.

### 적대적 NPC
![적대적 NPC.png](/ReadMeSource/NPC.png)
- 맵 내부를 순찰하는 3종류의 적대적 NPC가 존재합니다. 이들은 플레이어를 발견할 시, 추적 및 공격을 진행합니다.
- 적대적 NPC는 일정 시간마다 맵 내의 랜덤한 공간으로 텔레포트를 진행합니다. 따라서 옮길 수 있는 벽으로 접근을 방해할 수는 있으나 격리하는 것은 불가능합니다.

### 퍼즐 및 단서

- 여러 공간들은 자물쇠나 잠금장치가 적용된 문으로 막혀있습니다. 맵 내 곳곳에 위치한 단서와 열쇠들로 이들을 해결할 수 있습니다.
- 인게임 스토리 또한 퍼즐 요소를 해결하며 진행됩니다.
<br>


## 🖥️ 본인 개발 내용

### 플레이어 임의의 맵 구조 변경 시스템
```cs
// 시스템 중 벽을 들어올리는 기능(ActionContoller.cs)
private void PutUpWall()
{
  if (IsMoveWallActivated)
  {
    if (_raycastInfo.PresentObject.CompareTag("MovingWall"))
    {
      IsMovingWall = true;
      // 캐릭터가 보유한 망치 오브젝트를 들어올리는 동작
      _hammer.HammerUP(); 
      // 사운드 재생
      _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "PutUpWall");
      // 플레이어 속도 변화
      previousSpeed = thePlayerControl.ApplySpeed; 
      thePlayerControl.ApplySpeed = 0.5f;
      // 벽을 고정할 때, 다시 hierarchy를 복원하기 위한 정보
      _selectedWall = _raycastInfo.PresentObject;
      _selectedWallParent = _raycastInfo.PresentObjectParent;
      // 옮기고자 하는 벽이 주변 요소와 충돌하지 않도록 처리
      _selectedWallChildren = _selectedWall.GetComponentsInChildren<Transform>();
      foreach (Transform child in _selectedWallChildren) 
      {
        if (child.GetComponent<Collider>() == null)
        {
          child.gameObject.AddComponent<BoxCollider>();
        }
        child.GetComponent<Collider>().isTrigger = true;
      }
      // 플레이어의 시선과 벽이 함께 이동하도록 hierarchy 설정
      _selectedWall.transform.SetParent(_tempWallStorage.transform);
    }
  }
}
```
- 옮길 수 있는 벽을 들어 올리는 과정과 고정하는 과정으로 나누어 함수를 분리했습니다.
- 들어 올린 벽은 플레이어와 함께 이동할 수 있어야 하며, 플레이어의 시야를 따라 위치가 조정되어야 합니다. 그렇기에 벽을 기존의 부모 오브젝트로부터 분리하여 플레이어의 하위 오브젝트로 위계를 변경하였습니다. 벽을 고정할 때 원 상태로 복구됩니다.


### 플레이어 행동 및 정보 제어
Raycast 판별, 정보 제어, 행동 제어를 각각 담당하는 클래스들로 구성됩니다.

```cs
// Raycast 결과 판별 과정 일부(RaycastInfo.cs)
// 광선과 충돌한 오브젝트가 벽 이동과 관련되어 있는지 판별
private void RaycastHitObject()
{
	PreviousObject = PresentObject;
	PresentObject = HitInfo.transform.gameObject;

	// 부모 오브젝트가 존재하지 않는 경우
	if (PresentObject.transform.parent == null)
	{
  	PresentObjectParent = null;
	}
	// 부모 오브젝트는 존재하지만 벽 이동과 관련이 없는 경우
	else if (PresentObject.transform.parent.gameObject.CompareTag("Untagged"))
	{
		PresentObjectParent = PresentObject.transform.parent.gameObject;
	}
	// 부모 오브젝트가 존재하며 벽 이동과 관련이 있는 경우
	else
	{
    PresentObject = PresentObject.transform.parent.gameObject;
		if (PresentObject.transform.parent != null)
		{
			PresentObjectParent = PresentObject.transform.parent.gameObject;
		}
	}
}
```

- Raycast 판별 클래스에서는 프레임마다 광선 출동 / 충돌 상태 전환 / 충돌 오브젝트와 부모 오브젝트 / 충돌 오브젝트 전환을 확인합니다.
- 상태 및 오브젝트 전환을 확인하는 이유는 정보의 변경이 필요할 때만 정보 제어 클래스의 함수를 호출하기 위함입니다.
- 움직일 수 있는 벽은 태그가 지정된 빈 오브젝트 아래 벽의 구성 요소를 배치한 형태입니다. 그렇기에 부모 오브젝트 확인 과정이 포함되어 있습니다.

```cs
// 정보 제어 과정 일부(InformationController.cs)
// 조건에 맞는 정보를 띄우기 위한 판별 과정
void Update()
{
	// 광선의 충돌 여부가 변경되었을 때
	if (_raycastInfo.IsRaycastHitChanged)
	{
		// 광선이 (충돌X -> 충돌O)로 변경되었을 때
		if (_raycastInfo.IsPresentRaycastHit)
		{
  		_isInfoControl = true;
			InfoControl();
		}
		// 광선이 (충돌O -> 충돌X) 변경되었을 때
		else
		{
			_isInfoControl = false;
			InfoDisappear(); 
			if (_raycastInfo.PreviousObject.GetComponent<Outline>() != null)
			{
  			OutlineDisappear(_raycastInfo.PreviousObject);
			}
	  	_raycastInfo.PreviousObject = null;
		}
	}

	// 광선이 새로 충돌하거나, 충돌중인 상태에서 물체가 변경되었을 때 호출
	if (_isInfoControl && _raycastInfo.IsRaycastHitObjectChanged)
	{
  	InfoControl();
	}
}
```
![정보제어.png](/ReadMeSource/InfoControl.png)
- 정보 제어 클래스는 Raycast 판별 클래스의 멤버들을 바탕으로 외곽선과 안태 텍스트 활성화 / 텍스트 정보 업데이트 기능을 실행합니다.
- 행동 제어 클래스는 Raycast 판별 클래스의 멤버들과 플레이어 입력을 바탕으로 벽 옮기기 / 기능 활성화 / 아이템 픽업 / 망치 오브젝트 파괴 기능을 실행합니다.

### 사운드 매니저

```cs
// 사운드 매니저 일부(SoundManager.cs)
// 싱글톤
private void Awake()
{
  if (Instance == null)
  {
    Instance = this;
    DontDestroyOnLoad(gameObject);
  }
  else
  {
    Destroy(this.gameObject);
  }
}
```

- 맵 곳곳에 존재하는 사운드 요소를 통합 관리하기 위한 사운드 매니저입니다. 싱글톤 패턴을 사용하여 접근성을 높였으며 용이한 리소스 제어가 가능합니다.
- 단독 재생과 랜덤성을 기준으로 함수를 분리하였습니다.
    - 발소리 - switch 문을 사용하여 캐릭터별로 구분된 오디오 풀에서 랜덤하게 소리를 재생합니다.
    - 좀비 소리 - 보유 중인 오디오 플레이어가 비어있다면, 랜덤한 간격으로 소리를 재생합니다. 모든 플레이어가 재생 중이라면 효과음은 무시됩니다.
    - 단발성 효과음 - 보유 중인 오디오 플레이어가 비어있다면, 오디오 풀에서 해당 소스를 찾아 재생합니다. 모든 플레이어가 재생 중이라면 효과음은 무시됩니다.

### 애니메이션

- Unity의 애니메이션 상태 머신을 사용하여 플레이어와 NPC의 애니메이션을 적용하였습니다.
- 애니메이션 재생 속도를 이동 속도와 연동하여 보다 자연스러운 움직임을 만들었습니다.
<br>


## 🕹️ 인게임 영상

[![티저 및 설명 영상](/ReadMeSource/thumbnail.png)](https://youtu.be/zAkzgQ9c_IA?si=z_7aVDKqbU_w4EFW)
<br>


## 💡 성장 경험
### 의사소통은 자세하고 구체적으로

맵 구조 변경 시스템을 제작하는 과정에서 맵 제작을 담당한 팀원과 의사소통 오류가 있었습니다. 기존 환경에서 옮길 수 있는 벽은 여러 요소들로 구성되었으며, 벽을 옮기는 기능은 Raycast에 닿은 첫 번째 물체에만 적용되었습니다. 따라서 기능 사용 시 벽 전체가 아닌 일부 요소만 옮겨진다는 문제가 발생했습니다.

 이 문제는 팀 회의에서 공유되었습니다. 벽을 하나의 오브젝트로 합치기로 결정하고 맵을 제작한 팀원이 이 작업을 담당했습니다. 다음 회의에서 서로의 결과물을 공유했을 때, 제가 생각했던 것과는 구현 방식이 다름을 깨달았습니다. 하나의 부모 오브젝트 아래 벽의 구성요소들을 넣는 방식으로 구현되었기 때문입니다. 기존의 문제는 해결되지 않았습니다. 

 결국 벽을 옮기는 기능을 크게 수정했습니다. 다행히 일정은 지킬 수 있었지만 꽤나 타이트한 시간을 보내야 했습니다. 이를 경험하며 보다 구체적으로 요구 사항을 전달하고, 지속적으로 과정을 공유해야 함을 알 수 있었습니다.

### 객체지향 원칙을 지키며 개발할 것

 앞서 말한 기능 수정에 관한 이야기입니다. 기존 구조를 크게 바꿔야 했던 이유는 객체지향 원칙을 지키는 것보다 빠른 기능 구현에 무게를 두고 개발을 진행했기 때문입니다. 편의를 위해 하나의 클래스에 조금씩 부가 기능이 추가되었고, 여러 기능이 얽혀있는 상황에서 문제가 발생하자 대응에 어려움을 겪게 된 것입니다.

 수정된 구조를 만들 때는 역할과 책임에 따라 리팩토링을 진행하였으며, Raycast 판별, 행동 제어, 정보제어를 담당하는 별도의 클래스가 생성되었습니다. 방만한 개발이 어떠한 결과를 불러오는지 경험할 수 있었으며, 이후 객체지향 원칙을 지켜가며 프로젝트를 진행하는 계기가 되었습니다.
