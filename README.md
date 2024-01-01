# com.lurking-ninja.player-loop
A simple helper package to use Unity's custom player loop. The usage is simple:
## Installation
You can choose manually installing the package or from GitHub source
### Using direct git source code
Use the Package Manager's ```+/Add package from git URL``` function.
The URL you should use is this:
```
https://github.com/LurkingNinja/com.lurking-ninja.player-loop.git?path=Packages/com.lurking-ninja.player-loop
```
### Manual install
1. Download the latest ```.zip``` package from the [Release](https://github.com/LurkingNinja/com.lurking-ninja.player-loop/releases) section.
2. Unpack the ```.zip``` file into your project's ```Packages``` folder.
3. Open your project and check if it is imported properly.
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