# Complex Rust FFI Example

The stated goal from [round one][orig-repo]:

> An example project that shows the use of Rust's FFI capabilities to handle memory- or performance-senstivie work in Rust while the remainder of the application is written in Unity.

This follow-up to that project shows how to use more complex data structures (e.g. classes, structs) to share more sophisticated state between Unity and Rust. We're still using a Node server to manage connections, and leaning on Rust for networking, but this time multiple clients are connected, each creating a "sparkle" who's position is synchronized across all clients.

For more information, see the full [write-up][blog-post].

## Interesting files

|File|Description|
|---|---|
| `node/server.js` | Handles managing client state and broadcasting incoming messages to all connected clients. |
| `rust/src/lib.rs` | Defines the Rust-provided behaviour, sending events to the server based on FFI calls. |
| `unity/Assets/Plugins/FFI.cs` | C# bindings to the Rust library, as well as a few tricks to make those bindings simpler, e.g. handling floats. |
| `unity/Assets/SpawnNewSparkles.cs` | Unity Behaviour that consumes Rust-provided events and manages the Transforms for the different sparkles. |
| `unity/Assets/BroadcastMousePosition.cs` | Unity Behaviour that calls to Rust when the mouse button is down. |

## TODO

Looking for ways to extend this further? Here are some ideas (with varying levels of hand-holding):

- [ ] Don't broadcast redundant state updates.
- [ ] Broadcast the last-known position of all existing clients to new clients.
- [ ] There are still some `unwrap()` calls left in Rust for simplicity. Replace them with a different error handling strategy.
- [ ] If the Unity app loses focus and a lot of events come in, it will get behind and "catch up" once it regains focus. Fix that.

[orig-repo]: https://github.com/testdouble/rust-ffi-example
[blog-post]:
