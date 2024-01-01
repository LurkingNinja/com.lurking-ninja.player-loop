# com.lurking-ninja.player-loop
A simple helper package to use Unity's custom player loop. The usage is simple:
## Installation
Use the Package Manager's ```+/Install package from git URL``` function.
The URL you should use is this: 
```
https://github.com/LurkingNinja/com.lurking-ninja.player-loop.git?path=Packages/com.lurking-ninja.player-loop
```
## Usage
```csharp
using LurkingNinja.PlayerloopManagement;
using UnityEngine;

public class TestPlayerloop : IUpdate
{
    [RuntimeInitializeOnLoadMethod]
    private static void Awake()
    {
        var testPlayerloop = new TestPlayerloop();
        Playerloop.AddListener(testPlayerloop);
    }

    public void OnUpdate() => Debug.Log("Update");
}
```
You can always use the following type of player-loop entries: 
- EarlyUpdate 
- FixedUpdate
- PreUpdate
- Update
- PreLateUpdate
- PostLateUpdate

The corresponding interface's name is the same but starts with an "I" as standard, and the callback method starts with the "On" prefix.
If you want to register the same class into multiple callbacks use the standard typecasting:
```csharp
using LurkingNinja.PlayerloopManagement;
using UnityEngine;

public class TestPlayerloop : IUpdate, IPostLateUpdate
{
    [RuntimeInitializeOnLoadMethod]
    private static void Awake()
    {
        var testPlayerloop = new TestPlayerloop();
        Playerloop.AddListener((IUpdate)testPlayerloop);
        Playerloop.AddListener((IPostLateUpdate)testPlayerloop);
    }

    public void OnUpdate() => Debug.Log("Update");
    
    public void OnPostLateUpdate() => Debug.Log("PostLateUpdate");
}
```
It is possible to keep MonoBehaviour, the Awake or Start method and just register for Update and the rest of the repeated callbacks.
