# Amaryllis

Amaryllis State Behavior System is a Unity-friendly state/action framework.

## Unity Package Manager

Amaryllis can be installed as a Unity Package Manager package from this repository:

```json
"com.puzikgames.amaryllis": "https://github.com/denkacn/StateBehaviorSystem.git?path=/Assets/Amaryllis"
```

For local development, add it from disk with the package path:

```text
D:/MyProjects/StateBehaviorSystem/Assets/Amaryllis
```

Required dependency:
- `com.cysharp.unitask`

Optional dependencies:
- Odin Inspector enables `Amaryllis.EntityActions` and `Amaryllis.Utils`.
- DOTween is also required for `Amaryllis.EntityActions`.

Assemblies:
- `Amaryllis.Runtime` - core runtime, persistence, networking contracts, and debug events.
- `Amaryllis.Editor` - editor-only state debug window.
- `Amaryllis.EntityActions` - optional ready-made actions, compiled only when `ODIN_INSPECTOR` and `DOTWEEN` are available.
- `Amaryllis.Utils` - optional local helpers, compiled only when `ODIN_INSPECTOR` is available.

## Module Boundaries

Core:
- `States` - state graph, state lifecycle, transitions.
- `Actions` - action contracts, action execution, conditions, shared base classes.
- `Entities` - entity identity, entity registry, state-owning entity helpers.
- `Logs` - small logging facade.

Integrations and examples:
- `EntityActions` - reusable Unity/gameplay actions built on top of core.
- `Networks` - network authority checks and transport bridge components.
- `Persistence` - save/load helpers for state snapshots.
- `Utils` and `TestScene` - local test/demo helpers.

Keep game-specific behavior in `EntityActions` or project-specific assemblies. Core code should depend on contracts such as `IEntity`, `IStatesObject`, `IRunAction`, `ICharacterActionTarget`, and transport interfaces rather than concrete game managers.

## State Lifecycle

Typical initialization:

1. `IStatesObject.Init()` starts `InitAsync`.
2. `StatesObjectBase` builds the state cache.
3. The state graph is validated.
4. The start state is entered.
5. The active state runs `PreInit`.
6. The active state runs `Init`.
7. `OnInitHandler` is invoked.

Typical execution:

1. `IStatesObject.Exec(entity)` is called.
2. Reentrancy guard rejects the call if another execution or transition is active.
3. State conditions are checked, unless condition checks are explicitly disabled.
4. If conditions fail, `ConditionFail` actions run and `OnConditionFailHandler` is invoked.
5. If conditions pass, `Exec` actions run.
6. `OnExecHandler` is invoked.
7. If the state was not changed by an action, `NextStateId` is entered.

Typical transition:

1. `MoveToStateByIdAsync(stateId)` is called.
2. Missing state ids are logged as errors and ignored.
3. The old state's cancellation token is canceled.
4. The old state runs `Discard`.
5. The old state runs `PostDiscard`.
6. The new state runs `PreInit`.
7. The new state runs `Init`.
8. `OnStateChangedHandler` is invoked.

Cancellation:
- Every state/action call receives a `CancellationToken`.
- Delays use `UniTask.Delay(..., cancellationToken)`.
- State transitions cancel pending work from the previous state.
- Object destruction cancels work through `GetCancellationTokenOnDestroy`.

## State Graph Validation

`StatesObjectBase` validates the graph on `OnValidate` and during initialization.

Validation checks:
- At least one state exists.
- `StateId` values are unique.
- `_startState` exists.
- Each `NextStateId` exists, unless it is `-1`.

## Action Results

`IRunAction.Run` returns `RunActionResult`:

- `Success` - action executed successfully.
- `Skipped` - action was disabled or blocked by its run conditions.
- `Failed` - action ran and reported failure or threw an exception.
- `Canceled` - action was canceled by lifecycle/state cancellation.

Action chains treat only `Failed` and `Canceled` as chain failure. `Skipped` is not an error.

## Entity Registry

`EntitiesManager` stores entities by id and exposes:

- `Add(entity)`
- `Remove(entityId)`
- `Get(entityId)`
- `TryGet(entityId, out entity)`
- `Get()` for the first available entity, or `null` if empty
- `Clear()`

Duplicate ids replace the previous entry and log through `AmaryllisLog`.

## Saving And Loading States

Every `IStatesObject` exposes:

- `SaveId` - stable id used as the save key.
- `CaptureSnapshot()` - captures the current state id.
- `RestoreSnapshotAsync(snapshot)` - initializes the state object if needed and moves it to the saved state.

`StatesObjectBase` resolves `SaveId` in this order:

1. Explicit `_saveId` from the inspector.
2. Parent `IEntity.Id`, when available.
3. Transform path fallback.

For production saves, prefer explicit `_saveId` or entity ids. Transform paths are convenient for prototypes but can change when hierarchy names change.

`StatesJsonSerializer` provides scene-level JSON serialization only:

- `CaptureJson()` - captures all scene `IStatesObject` instances and returns JSON.
- `ApplyJsonAsync(json)` - restores matching state objects from JSON.
- `CaptureSceneSnapshot()` - returns the snapshot object before serialization.
- `ApplySceneSnapshotAsync(snapshot)` - restores from a snapshot object.

Amaryllis does not write saves to disk, `PlayerPrefs`, cloud, profiles, or databases. The game layer owns storage. Use `CaptureJson()` when you want to save and pass that string to your own save system. Use `ApplyJsonAsync(json)` after your game layer loads the string back.

## Network Layer

The network layer is split into two concepts.

Authority:
- `INetworkAuthorityProvider` reports current role.
- `NetworkAuthorityProviderRegistry.Set(provider)` installs a provider.
- Without a provider, Amaryllis uses offline/local-master behavior.
- `NetworkRunActionCondition` uses the registered provider for `Client` and `Master` checks.

Transport:
- `INetworkStatesObjectTransport` sends/receives state execution events.
- `INetworkEntityTransport` sends/receives entity creation events.
- `PunNetworkStatesObjectSynchronizer` and `PunNetworkEntitySynchronizer` are compatibility bridge components. They no longer contain Photon RPC logic directly; assign a `MonoBehaviour` implementing the relevant transport interface.

To integrate Photon, Mirror, Netcode for GameObjects, or a custom backend:

1. Implement `INetworkAuthorityProvider`.
2. Register it with `NetworkAuthorityProviderRegistry.Set`.
3. Implement `INetworkStatesObjectTransport` and/or `INetworkEntityTransport`.
4. Assign the transport component to the synchronizer's `_transportBehaviour`.
