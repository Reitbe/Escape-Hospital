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





## 🖥️ 본인 개발 내용

### 벽 옮기기 동작
```csharp
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

 플레이어 임의의 맵 구조 변경 시스템은 벽 옮기기 동작, 전방 Raycast 판별, 정보 제어 기능으로 구성됩니다.

 벽 옮기기 동작은 특정 아이템을 보유한 상태에서 이동 가능한 벽의 위치를 변경하는 기능입니다. 이는 벽을 들어 올리는 과정과 고정하는 과정으로 구분됩니다.

 이 기능을 사용하는 동안 벽 오브젝트는 플레이어 카메라의 하위 오브젝트로 지정됩니다. 이는 벽이 플레이어의 시야와 함께 움직이도록 하기 위함입니다. 벽을 고정하면 원래 부모 오브젝트의 하위로 복구됩니다. 

<br>

### 플레이어 행동 및 정보 제어
 플레이어 행동 및 정보 제어는 캐릭터의 전방 Raycast 정보를 기반으로 동작합니다. 각 부분은 개별 클래스로 구성되어 있습니다.

```csharp
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

 Raycast 판별 클래스에서는 프레임마다 광선 충돌 / 충돌 상태 전환 / 충돌 오브젝트와 부모 오브젝트 / 충돌 오브젝트 전환을 확인합니다. 

 상태 및 오브젝트 전환 여부를 확인하는 이유는 정보의 변경이 필요한 때만 정보 제어 클래스의 함수를 호출하기 위함입니다. 해당 클래스에서는 이때 필요한 정보의 갱신을 담당합니다.

 맵에는 옮길 수 있는 벽과 옮길 수 없는 벽이 배치되어 있으며, 옮길 수 있는 벽은 태그가 지정된 빈 오브젝트의 하위에 배치되어 있습니다. 이를 인식하기 위하여 부모 오브젝트 태그 확인 과정을 포함했습니다.

```csharp
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
 정보 제어 클래스는 Raycast 판별 클래스의 멤버들을 바탕으로 외곽선과 안내 텍스트 활성화 / 텍스트 정보 업데이트 기능을 실행합니다. 이때 광선의 충돌 전환 여부와 충돌 오브젝트 변경 사항을 참고하여 정보 갱신을 진행합니다.

 행동 제어 클래스는 Raycast 판별 클래스의 멤버들과 플레이어 입력을 바탕으로 벽 옮기기 / 기능 활성화 / 아이템 픽업 / 망치 오브젝트 파괴 기능을 실행합니다.
 
 <br>

### 사운드 매니저
```csharp
// 사운드 매니저 일부(SoundManager.cs)
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

 맵 곳곳에서 사용되는 사운드 리소스를 통합 관리하기 위한 사운드 매니저입니다. 설계 목적상 추가적인 인스턴스 생성이 없는 싱글톤 패턴이 적합하다고 판단하여 이를 적용했습니다. 

 단독 재생과 랜덤성을 기준으로 함수를 분리하였습니다. 

- 발소리 - switch 문을 사용하여 캐릭터별로 구분된 오디오 풀에서 랜덤하게 소리를 재생합니다.
- 좀비 소리 - 보유 중인 오디오 플레이어가 비어 있다면, 랜덤 간격으로 소리를 재생합니다. 모든 플레이어가 재생 중이라면 효과음은 무시됩니다.
- 단발성 효과음 - 보유 중인 오디오 플레이어가 비어 있다면, 오디오 풀에서 해당 소스를 찾아 재생합니다. 모든 플레이어가 재생 중이라면 효과음은 무시됩니다.

<br>

### 애니메이션

 Unity의 애니메이션 상태 머신을 사용하여 플레이어와 NPC의 애니메이션을 적용하였습니다. 또한, 애니메이션 재생 속도를 이동 속도와 연동하여 보다 자연스러운 움직임을 만들었습니다.

<br>


## 📜 주요 기능

### 플레이어 임의의 맵 구조 변경 시스템
![맵구조 변경 시스템.png](/ReadMeSource/MovingWall.png)
 맵 내부에는 플레이어 임의로 위치 변경이 가능한 벽들이 존재합니다. 이 벽들을 옮겨서 새로운 길을 개척하거나, 적대적 NPC의 접근을 차단하는 용도로 사용할 수 있습니다.

 본 기능은 맵 내에 존재하는 망치 오브젝트의 습득을 통해 활성화됩니다. 기능 활성화 상태에서 옮길 수 있는 벽을 바라보면 사용 가능 횟수와 외곽선이 표시됩니다. 

 사용 방법은 기능 활성화 상태에서 이동이 가능한 벽을 바라보고 키를 입력하는 것입니다. 키를 누르면 벽을 들어 올리고, 키를 놓으면 해당 위치에 벽이 고정됩니다.

 <br>

### 적대적 NPC
![적대적 NPC.png](/ReadMeSource/NPC.png)

 맵 내부를 순찰하는 3종류의 적대적 NPC가 존재합니다. 이들은 플레이어를 발견할 시, 추적 및 공격을 진행합니다.

 적대적 NPC는 일정 시간마다 맵 내의 랜덤 공간으로 텔레포트를 진행합니다. 따라서 옮길 수 있는 벽으로 접근을 방해할 수는 있으나 격리하는 것은 불가능합니다.

 <br>

### 퍼즐 및 단서

여러 공간은 자물쇠나 잠금장치가 적용된 문으로 막혀있습니다. 맵 내 곳곳에 위치한 단서와 열쇠들로 이들을 해결할 수 있습니다. 인게임 스토리 또한 퍼즐 요소를 해결하며 진행됩니다.

<br>

## 🕹️ 인게임 영상

[![티저 및 설명 영상](/ReadMeSource/thumbnail.png)](https://youtu.be/zAkzgQ9c_IA?si=z_7aVDKqbU_w4EFW)

<br>


## 💡 성장 경험
### 의사소통은 자세하고 구체적으로

맵 구조 변경 시스템을 제작하는 과정에서 맵 제작을 담당한 팀원과 의사소통 오류가 있었습니다. 기존의 벽을 옮기는 기능이 Raycast에 닿은 첫 번째 물체에만 적용되다 보니, 벽 전체가 아닌 일부만 움직이는 문제가 발생했습니다.

 문제는 곧 팀 회의에서 공유되었습니다. 해결책으로써 벽을 하나의 오브젝트로 합치자는 의견이 나왔고, 모두 동의하여 작업을 진행했습니다. 다음 회의에서 서로의 결과물을 공유했을 때, 제가 생각했던 것과는 구현 방식이 다름을 깨달았습니다. 하나의 부모 오브젝트 아래 벽의 구성요소들을 넣는 방식으로 구현하였으며, 기존의 문제는 해결되지 않았습니다. 

 결국 벽을 옮기는 기능을 크게 수정했습니다. 다행히 일정은 지킬 수 있었지만 꽤나 바쁜 시간을 보내야 했습니다. 이를 경험하며 보다 구체적으로 요구 사항을 전달하고, 지속적으로 과정을 공유해야 함을 깨달을 수 있었습니다.

<br>

### 객체지향 원칙을 지키며 개발할 것

위의 기능 수정을 진행한 주된 이유는 빠른 기능 구현에 집중하는 과정에서 객체지향 원칙을 충분히 고려하지 못했기 때문입니다. 캐릭터의 행동을 관리하는 클래스에 여러 부가 기능들이 복잡하게 얽히면서, 문제가 발생했을 때 이를 신속하게 해결하는 데 어려움이 있었습니다.

 이후, 리팩토링을 통해 Raycast 판별, 행동 제어, 정보제어를 담당하는 클래스들로 기능을 분리했습니다. 그 과정에서 기능 분리의 필요성을 절감했으며, 객체지향 원칙을 준수하려 노력하게 되는 계기가 되었습니다.
